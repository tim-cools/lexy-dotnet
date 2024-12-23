

namespace Lexy.Poc.Core.Language
{
    public abstract class RootNode : ParsableNode, IRootNode
    {
        public abstract string NodeName { get; }
    }
}