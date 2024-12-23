using Lexy.Poc.Core.Language;

namespace Lexy.Poc.Core.Parser
{
    public interface IParserContext
    {
        IParserLogger Logger { get; }

        Line CurrentLine { get; }

        Nodes Nodes { get; }

        void ProcessNode(IRootNode node);

        bool ProcessLine();

        TokenValidator ValidateTokens<T>();
        TokenValidator ValidateTokens(string name);
    }
}