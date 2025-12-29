namespace Lexy.Compiler.Language.Expressions;

public interface IHasVariableReference : INode
{
    VariableReference Variable { get; }
}