using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Buschmann.Windows.Controls;
using MbUnit.Framework;

namespace Buschmann.Windows.UnitTests
{
    [TestFixture]
    public class CommandTextBoxFixture
    {
        [Test]
        [ExpectedArgumentException("Two commands with the same command string, \"command:\", were added.  The command text must be unique.")]
        public void VerifyExceptionForDuplicateCommandStrings()
        {
            ITextBoxCommand command1 = new TextBoxCommand("command:", null, null);
            ITextBoxCommand command2 = new TextBoxCommand("command:", null, null);

            CommandTextBox commandTextBox = GetCommandTextBox(command1, command2);
        }

        [Test]
        [ExpectedArgumentException("'' is not a valid value for property 'Commands'.")]
        public void VerifyNullCommandList()
        {
            CommandTextBox commandTextBox = GetCommandTextBox();
            commandTextBox.Commands = null; // Switch back to null to fire the property changed event.
        }

        [Test]
        public void VerifyDefaultCommandIsSetWhenListIsSet()
        {
            var defaultCommand = new TextBoxCommand("", null, null);
            var nonDefaultCommand1 = new TextBoxCommand("nondefault1:", null, null);
            var nonDefaultCommand2 = new TextBoxCommand("nondefault2:", null, null);

            CommandTextBox commandTextBox = GetCommandTextBox();

            Assert.AreNotEqual(defaultCommand, commandTextBox.ActiveCommand);

            commandTextBox.Commands = new List<ITextBoxCommand> {defaultCommand, nonDefaultCommand1, nonDefaultCommand2};

            Assert.AreEqual(defaultCommand, commandTextBox.ActiveCommand);

            commandTextBox.Commands = new List<ITextBoxCommand>();

            Assert.AreEqual(TextBoxCommand.Empty, commandTextBox.ActiveCommand);
        }

        [Test]
        public void VerifyDefaultEmptyCommandIsSetWhenListIsMissingDefaultCommand()
        {
            var nonDefaultCommand1 = new TextBoxCommand("nondefault1:", null, null);
            var nonDefaultCommand2 = new TextBoxCommand("nondefault2:", null, null);

            CommandTextBox commandTextBox = GetCommandTextBox();

            Assert.AreEqual(TextBoxCommand.Empty, commandTextBox.ActiveCommand);

            commandTextBox.Commands = new List<ITextBoxCommand> { nonDefaultCommand1, nonDefaultCommand2 };

            Assert.AreEqual(TextBoxCommand.Empty, commandTextBox.ActiveCommand);
        }

        [Test]
        public void VerifyTextBoxIsUpdatedWhenTextPropertyIsUpdated()
        {
            CommandTextBox commandTextBox = GetCommandTextBox();

            commandTextBox.Text = "c";
            Assert.AreEqual("c", commandTextBox.GetTextBoxText());

            commandTextBox.Text = "cs";
            Assert.AreEqual("cs", commandTextBox.GetTextBoxText());
            
            commandTextBox.Text = "csh";
            Assert.AreEqual("csh", commandTextBox.GetTextBoxText());

            commandTextBox.Text = "csh";
            Assert.AreEqual("csh", commandTextBox.GetTextBoxText());

            commandTextBox.Text = "";
            Assert.AreEqual("", commandTextBox.GetTextBoxText());
        }

        [Test]
        public void VerifyTextPropertyIsUpdatedWhenTextBoxIsUpdated()
        {
            CommandTextBox commandTextBox = GetCommandTextBox();

            commandTextBox.SetTextBoxText("c");
            Assert.AreEqual("c", commandTextBox.Text);

            commandTextBox.SetTextBoxText("cs");
            Assert.AreEqual("cs", commandTextBox.Text);

            commandTextBox.SetTextBoxText("csh");
            Assert.AreEqual("csh", commandTextBox.Text);

            commandTextBox.SetTextBoxText("csh");
            Assert.AreEqual("csh", commandTextBox.Text);

            commandTextBox.SetTextBoxText("");
            Assert.AreEqual("", commandTextBox.Text);
        }

        [Test]
        public void VerifyDefaultCommand()
        {
            int commandCount = 0;
            string commandText = null;
            ITextBoxCommand defaultCommand = new TextBoxCommand("", null, s => { commandCount++; commandText = s; });

            CommandTextBox commandTextBox = GetCommandTextBox(defaultCommand);

            commandTextBox.ExecuteCommand();
            Assert.AreEqual(1, commandCount);
            Assert.AreEqual("", commandText);

            commandTextBox.SetTextBoxText("c");
            commandTextBox.ExecuteCommand();
            Assert.AreEqual(2, commandCount);
            Assert.AreEqual("c", commandText);

            commandTextBox.SetTextBoxText("cs");
            commandTextBox.ExecuteCommand();
            Assert.AreEqual(3, commandCount);
            Assert.AreEqual("cs", commandText);

            commandTextBox.SetTextBoxText("");
            commandTextBox.ExecuteCommand();
            Assert.AreEqual(4, commandCount);
            Assert.AreEqual("", commandText);
        }

        [Test]
        [Row(true)]
        [Row(false)]
        public void VerifyCommandSwitch(bool useTextProperty)
        {
            int command1Count = 0, command2Count = 0;
            string command1Text = null, command2Text = null;

            ITextBoxCommand command1 = new TextBoxCommand("", null, s => { command1Count++; command1Text = s; });
            ITextBoxCommand command2 = new TextBoxCommand("command2:", null, s => { command2Count++; command2Text = s; });

            CommandTextBox commandTextBox = GetCommandTextBox(command1, command2);

            Assert.AreEqual(commandTextBox.ActiveCommand, command1);

            commandTextBox.ExecuteCommand();
            Assert.AreEqual(1, command1Count);
            Assert.AreEqual("", command1Text);

            Action<string> setText = s => SetText(commandTextBox, useTextProperty, s);

            setText("command2:csh");
            Assert.AreEqual(commandTextBox.ActiveCommand, command2);

            commandTextBox.ExecuteCommand();
            Assert.AreEqual(1, command2Count);
            Assert.AreEqual("csh", command2Text);

            setText("csh");
            Assert.AreEqual(commandTextBox.ActiveCommand, command1);

            commandTextBox.ExecuteCommand();
            Assert.AreEqual(2, command1Count);
            Assert.AreEqual("csh", command1Text);
        }

        [Test]
        [Row(true)]
        [Row(false)]
        public void VerifyCorrectCommandTextAfterRemovingCommandString(bool useTextProperty)
        {
            string commandText = null;

            ITextBoxCommand command = new TextBoxCommand("command:", null, s => { commandText = s; });

            CommandTextBox commandTextBox = GetCommandTextBox(command);

            Action<string> setText = s => SetText(commandTextBox, useTextProperty, s);

            setText("command:command:");
            commandTextBox.ExecuteCommand();
            Assert.AreEqual("command:", commandText);

            setText("command:cshcommand:");
            commandTextBox.ExecuteCommand();
            Assert.AreEqual("cshcommand:", commandText);

            setText("command:ccommand:sh");
            commandTextBox.ExecuteCommand();
            Assert.AreEqual("ccommand:sh", commandText);
        }

        private void SetText(CommandTextBox commandTextBox, bool useTextProperty, string text)
        {
            if (useTextProperty)
                commandTextBox.Text = text;
            else
                commandTextBox.SetTextBoxText(text);
        }


        private CommandTextBox GetCommandTextBox(params ITextBoxCommand[] commands)
        {
            return GetCommandTextBox(commands as IEnumerable<ITextBoxCommand>);
        }

        private CommandTextBox GetCommandTextBox(IEnumerable<ITextBoxCommand> commands = null)
        {
            CommandTextBox commandTextBox = new CommandTextBox();
            commandTextBox.OnApplyTemplate();

            if (commands != null)
                commandTextBox.Commands = commands;

            return commandTextBox;
        }
    }
}
