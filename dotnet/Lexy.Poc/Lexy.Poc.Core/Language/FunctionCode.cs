using System.Collections.Generic;
using Lexy.Poc.Core.Language.Expressions;
using Lexy.Poc.Core.Parser;

namespace Lexy.Poc.Core.Language
{
    public class FunctionCode : IComponent
    {
        public IList<Expression> Expressions { get; } = new List<Expression>();

        public IComponent Parse(IParserContext context)
        {
            if (context.CurrentLine.IsComment())
            {
                return this;
            }
            if (context.CurrentLine.IsEmpty())
            {
                return this;
            }

            var valid = context.ValidateTokens<FunctionCode>()
                .CountMinimum(1)
                .IsValid;

            if (!valid) return null;

            var expression = ExpressionFactory.Parse(context, context.CurrentLine.Tokens);

            Expressions.Add(expression);
            return this;
        }
    }
}