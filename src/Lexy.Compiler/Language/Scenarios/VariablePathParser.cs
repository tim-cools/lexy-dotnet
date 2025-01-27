using System;
using Lexy.Compiler.Language.Expressions;
using Lexy.Compiler.Parser.Tokens;
using Lexy.RunTime;

namespace Lexy.Compiler.Language.Scenarios;

public static class VariablePathParser
{
    public static VariablePath Parse(string name)
    {
        if (string.IsNullOrEmpty(name)) throw new ArgumentNullException(nameof(name));

        var parts = name.Split(".");
        return new VariablePath(parts);
    }

    public static VariablePathParseResult Parse(string[] parts)
    {
        var variableReference = new VariablePath(parts);
        return VariablePathParseResult.Success(variableReference);
    }

    public static VariablePathParseResult Parse(Expression expression)
    {
        return expression switch
        {
            MemberAccessExpression memberAccessExpression => Parse(memberAccessExpression),
            LiteralExpression literalExpression => Parse(literalExpression),
            IdentifierExpression literalExpression => VariablePathParseResult.Success(Parse(literalExpression.Identifier)),
            _ => VariablePathParseResult.Failed("Invalid constant value. Expected: 'Variable = ConstantValue'")
        };
    }

    private static VariablePathParseResult Parse(LiteralExpression literalExpression)
    {
        return literalExpression.Literal switch
        {
            StringLiteralToken stringLiteral => VariablePathParseResult.Success(Parse(stringLiteral.Value)),
            _ => VariablePathParseResult.Failed("Invalid expression literal. Expected: 'Variable = ConstantValue'")
        };
    }

    private static VariablePathParseResult Parse(MemberAccessExpression memberAccessExpression)
    {
        if (memberAccessExpression.MemberAccessLiteral.Parts.Length == 0)
            return VariablePathParseResult.Failed("Invalid number of variable reference parts: "
                                                       + memberAccessExpression.MemberAccessLiteral.Parts.Length);

        var variableReference = new VariablePath(memberAccessExpression.MemberAccessLiteral.Parts);
        return VariablePathParseResult.Success(variableReference);
    }
}