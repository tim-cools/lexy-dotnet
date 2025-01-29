using System.Collections.Generic;
using Lexy.Compiler.Language.VariableTypes;
using Lexy.Compiler.Parser;

namespace Lexy.Compiler.Language.Expressions.Functions;

public abstract class NoArgumentFunction : FunctionCallExpression
{
    protected abstract VariableType ResultType { get; }

    protected NoArgumentFunction(string functionName, ExpressionSource source)
        : base(functionName, source)
    {
    }

    public override IEnumerable<INode> GetChildren()
    {
        yield break;
    }

    protected override void Validate(IValidationContext context)
    {
    }

    public override VariableType DeriveType(IValidationContext context)
    {
        return ResultType;
    }
}