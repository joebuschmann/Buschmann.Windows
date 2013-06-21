using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;

namespace Buschmann.Windows.Controls
{
    [TemplatePart(Name = InputTextBox, Type = typeof(TextBox))]
    [TemplatePart(Name = CommandListShowButton, Type = typeof(Image))]
    [TemplatePart(Name = CommandListHideButton, Type = typeof(Image))]
    [TemplatePart(Name = CommandListPopup, Type = typeof(Popup))]
    public class CommandTextBox : Control
    {
        private const string InputTextBox = "inputTextBox";
        private const string CommandListShowButton = "commandListShowButton";
        private const string CommandListHideButton = "commandListHideButton";
        private const string CommandListPopup = "commandListPopup";

        private TextBox _textBox;
        private Image _commandListShowButton;
        private Image _commandListHideButton;
        private Popup _commandListPopup;

        public CommandTextBox()
        {
            DefaultStyleKey = typeof(CommandTextBox);
        }

        public override void OnApplyTemplate()
        {
            _textBox = Template.FindName(InputTextBox, this) as TextBox ?? new TextBox();
            _textBox.TextChanged += TextBoxTextChanged;
            _textBox.KeyDown += TextBoxKeyDown;

            _commandListShowButton = Template.FindName(CommandListShowButton, this) as Image ?? new Image();
            _commandListShowButton.MouseLeftButtonUp += (s, e) => CommandListPopupIsVisible = true;

            _commandListHideButton = Template.FindName(CommandListHideButton, this) as Image ?? new Image();
            _commandListHideButton.MouseLeftButtonUp += (s, e) => CommandListPopupIsVisible = false;

            _commandListPopup = Template.FindName(CommandListPopup, this) as Popup ?? new Popup();
            _commandListPopup.Closed += CommandListPopupClosed;

            base.OnApplyTemplate();
        }

        private void CommandListPopupClosed(object sender, EventArgs e)
        {
            Point point = Mouse.GetPosition(this);

            if (InputHitTest(point) != _commandListHideButton)
                CommandListPopupIsVisible = false;
        }

        private void TextBoxTextChanged(object sender, TextChangedEventArgs e)
        {
            if (_textBox.Text != Text)
                Text = _textBox.Text;

            UpdateActiveCommand();
        }

        private void TextBoxKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape)
                _textBox.Clear();
            else if ((e.Key == Key.Enter) && (ExecuteCommandOnEnter))
                ExecuteCommand();
        }

        private static readonly DependencyPropertyKey ActiveCommandKey =
            DependencyProperty.RegisterReadOnly("ActiveCommand", typeof (ITextBoxCommand), typeof (CommandTextBox),
                                                new PropertyMetadata(TextBoxCommand.Empty));

        public static readonly DependencyProperty ActiveCommandProperty = ActiveCommandKey.DependencyProperty;

        public ITextBoxCommand ActiveCommand
        {
            get { return (ITextBoxCommand) GetValue(ActiveCommandProperty); }
            private set { SetValue(ActiveCommandKey, value); }
        }

        private static readonly DependencyPropertyKey CommandListPopupIsVisibleKey =
            DependencyProperty.RegisterReadOnly("CommandListPopupIsVisible", typeof(bool), typeof(CommandTextBox),
                                                new PropertyMetadata(false));

        public static readonly DependencyProperty CommandListPopupIsVisibleProperty = ActiveCommandKey.DependencyProperty;

        public bool CommandListPopupIsVisible
        {
            get { return (bool)GetValue(CommandListPopupIsVisibleProperty); }
            private set { SetValue(CommandListPopupIsVisibleKey, value); }
        }

        public static readonly DependencyProperty CommandsProperty = DependencyProperty.Register(
            "Commands", typeof (IEnumerable<ITextBoxCommand>), typeof (CommandTextBox),
            new PropertyMetadata(new List<ITextBoxCommand>() { TextBoxCommand.Empty }, CommandsChanged), c => c != null);

        public IEnumerable<ITextBoxCommand> Commands
        {
            get { return (IEnumerable<ITextBoxCommand>)GetValue(CommandsProperty); }
            set { SetValue(CommandsProperty, value); }
        }

        private static void CommandsChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            CommandTextBox commandTextBox = d as CommandTextBox;

            if (commandTextBox == null)
                return;

            // Make sure there are no duplicate commands.
            var commands = ((IEnumerable<ITextBoxCommand>) e.NewValue);
            var commandStack = new Stack<ITextBoxCommand>(commands);
            
            while (commandStack.Count != 0)
            {
                ITextBoxCommand command = commandStack.Pop();

                if (commandStack.Any(c => c.CommandString == command.CommandString))
                {
                    throw new ArgumentException(string.Format("Two commands with the same command string, \"{0}\", were added.  The command text must be unique.", command.CommandString));
                }
            }

            // Set the default command as the active one.
            ITextBoxCommand defaultCommand = commands.FirstOrDefault(c => string.IsNullOrEmpty(c.CommandString)) ??
                                             TextBoxCommand.Empty;

            commandTextBox.ActiveCommand = defaultCommand;
        }

        public static DependencyProperty TextProperty = DependencyProperty.Register("Text", typeof (string),
                                                                                    typeof (CommandTextBox),
                                                                                    new PropertyMetadata("", TextChanged));

        public string Text
        {
            get { return (string) GetValue(TextProperty); }
            set { SetValue(TextProperty, value); }
        }

        private static void TextChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            CommandTextBox commandTextBox = d as CommandTextBox;

            if (commandTextBox == null)
                return;

            commandTextBox._textBox.Text = e.NewValue.ToString();
        }

        public bool ExecuteCommandOnEnter { get; set; }

        private void UpdateActiveCommand()
        {
            string text = _textBox.Text;
            ITextBoxCommand matchingCommand = FindMatchingCommand(text);

            if (matchingCommand != ActiveCommand)
                ActiveCommand = matchingCommand;
        }

        private ITextBoxCommand FindMatchingCommand(string text)
        {
            IEnumerable<ITextBoxCommand> commands = Commands;
            ITextBoxCommand matchingCommand =
                commands.FirstOrDefault(c => c.CommandString.Length > 0 && text.StartsWith(c.CommandString));

            return matchingCommand ?? commands.First(c => c.CommandString.Length == 0);
        }

        /// <summary>
        /// Used for unit tests.
        /// </summary>
        public void SetTextBoxText(string text)
        {
            _textBox.Text = text;
        }

        /// <summary>
        /// Used for unit tests.
        /// </summary>
        /// <returns></returns>
        public string GetTextBoxText()
        {
            return _textBox.Text;
        }

        public void ExecuteCommand()
        {
            ITextBoxCommand activeCommand = ActiveCommand;
            string text = _textBox.Text;
            int commandLength = activeCommand.CommandString.Length;

            if (commandLength > 0)
                text = text.Substring(commandLength, text.Length - commandLength);

            activeCommand.Execute(text);
        }
    }
}
