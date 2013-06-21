using System;

namespace Buschmann.Windows.Controls
{
    public class TextBoxCommand : ITextBoxCommand
    {
        private static readonly ITextBoxCommand _emptyCommand = new TextBoxCommand(null, null, null);
        private readonly string _commandString;
        private readonly string _description;
        private readonly Action<string> _execute;

        public TextBoxCommand(string commandString, string description, Action<string> execute)
        {
            _commandString = commandString ?? "";
            _description = description ?? "";
            _execute = execute ?? (_ => { });
        }

        public string CommandString
        {
            get { return _commandString; }
        }

        public string Description
        {
            get { return _description; }
        }

        public void Execute(string commandText)
        {
            _execute(commandText);
        }

        public static ITextBoxCommand Empty
        {
            get { return _emptyCommand; }
        }
    }
}
