using System;
using System.Collections.Generic;
using Lexy.Compiler.Language.VariableTypes;
using Lexy.Compiler.Parser;

namespace Lexy.Compiler.Language.Expressions.Functions;

public abstract class SingleArgumentFunction : FunctionCallExpression
{
    private readonly VariableType argumentType;
    private readonly VariableType resultType;

    protected abstract string FunctionHelp { get; }

    public Expression ValueExpression { get; }

    protected SingleArgumentFunction(string functionName, Expression valueExpression, ExpressionSource source,
        VariableType argumentType, VariableType resultType)
        : base(functionName, source)
    {
        ValueExpression = valueExpression ?? throw new ArgumentNullException(nameof(valueExpression));
        this.argumentType = argumentType ?? throw new ArgumentNullException(nameof(argumentType));
        this.resultType = resultType ?? throw new ArgumentNullException(nameof(resultType));
    }

    public override IEnumerable<INode> GetChildren()
    {
        yield return ValueExpression;
    }

    protected override void Validate(IValidationContext context)
    {
        context.ValidateType(ValueExpression, 1, "Value", argumentType, Reference, FunctionHelp);
    }

    public override VariableType DeriveType(IValidationContext context)
    {
        return resultType;
    }
}