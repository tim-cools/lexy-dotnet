using Lexy.Compiler.Parser;

namespace Lexy.Compiler.Language;

public abstract class RootNode : ParsableNode, IRootNode
{
    public abstract string NodeName { get; }

    protected RootNode(SourceReference reference) : base(reference)
    {
    }
}