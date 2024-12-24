using System.Collections.Generic;
using Lexy.Poc.Core.Language.Expressions;
using Lexy.Poc.Core.Parser;

namespace Lexy.Poc.Core.Language
{
    public class FunctionCode : ParsableNode
    {
        private readonly ExpressionList expressions;

        public IReadOnlyList<Expression> Expressions => expressions;

        public FunctionCode(SourceReference reference) : base(reference)
        {
            expressions = new ExpressionList(reference);
        }

        public override IParsableNode Parse(IParserContext context)
        {
            var line = context.CurrentLine;
            if (line.IsComment() || line.IsEmpty())
            {
                return this;
            }

            var expression = ExpressionFactory.Parse(context.SourceCode.File, line.Tokens, line);
            if (expression != null)
            {
                expressions.Add(expression, context);
            }
            return expression is IParsableNode node ? node : this;
        }

        public override IEnumerable<INode> GetChildren() => Expressions;

        protected override void Validate(IValidationContext context)
        {
        }
    }
}