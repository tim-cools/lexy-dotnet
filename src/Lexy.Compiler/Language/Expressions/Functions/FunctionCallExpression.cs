using System;
using System.Collections.Generic;
using System.Linq;
using Lexy.Compiler.Parser;
using Lexy.Compiler.Parser.Tokens;

namespace Lexy.Compiler.Language.Expressions.Functions;

public abstract class FunctionCallExpression : Expression
{
    public string FunctionName { get; }

    internal FunctionCallExpression(string functionName, ExpressionSource source)
        : base(source, source.CreateReference())
    {
        FunctionName = functionName ?? throw new ArgumentNullException(nameof(functionName));
    }

    public static bool IsValid(TokenList tokens)
    {
        return tokens.IsTokenType<StringLiteralToken>(0)
               && tokens.IsOperatorToken(1, OperatorType.OpenParentheses);
    }

    public override IEnumerable<VariableUsage> UsedVariables()
    {
        return GetChildren()
            .OfType<Expression>()
            .GetReadVariableUsageNodes();
    }
}