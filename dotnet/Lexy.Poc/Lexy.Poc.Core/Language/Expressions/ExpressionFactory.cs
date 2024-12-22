using System;
using System.Collections.Generic;
using Lexy.Poc.Core.Parser;

namespace Lexy.Poc.Core.Language.Expressions
{
    public static class ExpressionFactory
    {
        private static readonly IDictionary<Func<TokenList, bool>, Func<Line, TokenList, ParseExpressionResult>> factories =
            new Dictionary<Func<TokenList, bool>, Func<Line, TokenList, ParseExpressionResult>>()
            {
                { AssignmentExpression.IsValid, AssignmentExpression.Parse },
                { VariableExpression.IsValid, VariableExpression.Parse },
                { LiteralExpression.IsValid, LiteralExpression.Parse },
                { MemberAccessExpression.IsValid, MemberAccessExpression.Parse },
                { BinaryExpression.IsValid, BinaryExpression.Parse },
                { ParenthesizedExpression.IsValid, ParenthesizedExpression.Parse },
                { BracketedExpression.IsValid, BracketedExpression.Parse },
                { FunctionCallExpression.IsValid, FunctionCallExpression.Parse },
            };

        public static Expression Parse(TokenList tokens, Line currentLine)
        {
            foreach (var factory in factories)
            {
                if (factory.Key(tokens))
                {
                    var expressionResult = factory.Value(currentLine, tokens);
                    switch (expressionResult.Status)
                    {
                        case ParseExpressionStatus.Success:
                            return expressionResult.Expression;
                        case ParseExpressionStatus.Failed:
                            throw new InvalidOperationException($"Invalid expression: {tokens}. {expressionResult.ErrorMessage}");
                    }
                }
            }

            throw new InvalidOperationException($"Invalid expression: {tokens}");
        }
    }
}