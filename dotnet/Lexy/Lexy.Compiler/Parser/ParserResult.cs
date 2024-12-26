using Lexy.Compiler.Language;

namespace Lexy.Compiler.Parser
{
    public class ParserResult
    {
        public Nodes Nodes { get; }

        public ParserResult(Nodes nodes)
        {
            Nodes = nodes;
        }
    }
}