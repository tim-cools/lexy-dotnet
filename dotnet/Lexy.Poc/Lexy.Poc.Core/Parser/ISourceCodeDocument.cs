
namespace Lexy.Poc.Core.Parser
{
    public interface ISourceCodeDocument
    {
        Line CurrentLine { get; }

        void SetCode(string[] lines);

        bool HasMoreLines();
        Line NextLine();
        void Reset();
    }
}