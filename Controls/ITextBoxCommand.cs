namespace Buschmann.Windows.Controls
{
    public interface ITextBoxCommand
    {
        string CommandString { get; }
        string Description { get; }
        void Execute(string commandText);
    }
}
