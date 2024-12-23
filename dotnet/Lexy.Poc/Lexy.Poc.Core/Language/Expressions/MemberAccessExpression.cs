using System.Collections.Generic;
using Lexy.Poc.Core.Parser;
using Lexy.Poc.Core.Parser.Tokens;

namespace Lexy.Poc.Core.Language.Expressions
{
    public class MemberAccessExpression : Expression
    {
        public string Value { get; }

        private MemberAccessExpression(string value, ExpressionSource source, SourceReference reference) : base(source, reference)
        {
            Value = value;
        }

        public static ParseExpressionResult Parse(ExpressionSource source)
        {
            var tokens = source.Tokens;
            if (!IsValid(tokens))
            {
                return ParseExpressionResult.Invalid<MemberAccessExpression>("Invalid expression");
            }

            var variableName = tokens.TokenValue(0);
            var reference = source.CreateReference();

            var accessExpression = new MemberAccessExpression(variableName, source, reference);
            return ParseExpressionResult.Success(accessExpression);
        }

        public static bool IsValid(TokenList tokens)
        {
            return tokens.Length == 1
                   && tokens.IsTokenType<MemberAccessLiteral>(0);
        }

        protected override IEnumerable<INode> GetChildren()
        {
            yield break;
        }

        protected override void Validate(IValidationContext context)
        {
        }
    }
}