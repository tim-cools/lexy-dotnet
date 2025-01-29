using System.Collections.Generic;
using Lexy.Compiler.Language.Expressions.Functions;

namespace Lexy.Compiler.Language.Expressions;

public static class FunctionCallExpressionParser
{
    public static ParseExpressionResult Parse(ExpressionSource source, IExpressionFactory factory)
    {
        var tokens = source.Tokens;
        if (!FunctionCallExpression.IsValid(tokens)) return ParseExpressionResult.Invalid<FunctionCallExpression>("Not valid.");

        var matchingClosingParenthesis = ParenthesizedExpression.FindMatchingClosingParenthesis(tokens);
        if (matchingClosingParenthesis == -1)
            return ParseExpressionResult.Invalid<FunctionCallExpression>("No closing parentheses found.");

        var functionName = tokens.TokenValue(0);
        var innerExpressionTokens = tokens.TokensRange(2, matchingClosingParenthesis - 1);
        var argumentsTokenList = ArgumentList.Parse(innerExpressionTokens);
        if (!argumentsTokenList.IsSuccess)
            return ParseExpressionResult.Invalid<FunctionCallExpression>(argumentsTokenList.ErrorMessage);

        var arguments = new List<Expression>();
        foreach (var argumentTokens in argumentsTokenList.Result)
        {
            var argumentExpression = factory.Parse(argumentTokens, source.Line);
            if (!argumentExpression.IsSuccess) return argumentExpression;

            arguments.Add(argumentExpression.Result);
        }

        var builtInFunctionResult = BuiltInExpressionFunctions.Parse(functionName, source, arguments);
        if (builtInFunctionResult is { IsSuccess: false })
            return ParseExpressionResult.Invalid<FunctionCallExpression>(builtInFunctionResult.ErrorMessage);

        return ParseExpressionResult.Success(builtInFunctionResult?.Result);
    }
}