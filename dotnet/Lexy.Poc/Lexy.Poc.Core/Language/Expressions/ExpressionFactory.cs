using System;
using System.Collections.Generic;
using Lexy.Poc.Core.Parser;

namespace Lexy.Poc.Core.Language.Expressions
{
    public static class ExpressionFactory
    {
        private static IDictionary<Func<TokenList, bool>, Func<IParserContext, Line, TokenList, Expression>> factories =
            new Dictionary<Func<TokenList, bool>, Func<IParserContext, Line, TokenList, Expression>>()
            {
                { AssignmentExpression.IsValid, AssignmentExpression.Parse },
                { VariableExpression.IsValid, VariableExpression.Parse },
                { LiteralExpression.IsValid, LiteralExpression.Parse },
                { MemberAccessExpression.IsValid, MemberAccessExpression.Parse },

            };
        public static Expression Parse(IParserContext context, TokenList tokens)
        {
            var sourceLine = context.CurrentLine;

            foreach (var factory in factories)
            {
                if (factory.Key(tokens))
                {
                    return factory.Value(context, sourceLine, tokens);
                }
            }

            throw new InvalidOperationException("Invalid expression: " + context.CurrentLine);
        }
    }
}