using System;
using System.Collections.Generic;
using System.Linq;
using Lexy.Poc.Core.Parser;
using Lexy.Poc.Core.Parser.Tokens;

namespace Lexy.Poc.Core.Language.Expressions
{
    public class FunctionCallExpression : Expression
    {
        private class ArgumentTokenParseResult
        {
            public string ErrorMessage { get; }
            public bool IsSuccess { get; }
            public IEnumerable<TokenList> Result { get; }

            private ArgumentTokenParseResult(IEnumerable<TokenList> result)
            {
                Result = result;
                IsSuccess = true;
            }

            private ArgumentTokenParseResult(bool success, string errorMessage)
            {
                ErrorMessage = errorMessage;
                IsSuccess = success;
            }

            public static ArgumentTokenParseResult Success(IEnumerable<TokenList> result = null)
            {
                return new ArgumentTokenParseResult(result ?? new TokenList[]{});
            }

            public static ArgumentTokenParseResult Failed(string errorMessage)
            {
                return new ArgumentTokenParseResult(false, errorMessage);
            }
        }

        public string FunctionName { get; }
        public Expression[] Arguments { get; }

        private FunctionCallExpression(string functionName, Expression[] arguments,
            ExpressionSource source, SourceReference reference) : base(source, reference)
        {
            FunctionName = functionName ?? throw new ArgumentNullException(nameof(functionName));
            Arguments = arguments ?? throw new ArgumentNullException(nameof(arguments));
        }

        public static ParseExpressionResult Parse(ExpressionSource source)
        {
            var tokens = source.Tokens;
            if (!IsValid(tokens))
            {
                return ParseExpressionResult.Invalid<FunctionCallExpression>("Not valid.");
            }

            var matchingClosingParenthesis = ParenthesizedExpression.FindMatchingClosingParenthesis(tokens);
            if (matchingClosingParenthesis == -1)
            {
                return ParseExpressionResult.Invalid<FunctionCallExpression>("No closing parentheses found.");
            }

            var functionName = tokens.TokenValue(0);
            var innerExpressionTokens = tokens.TokensRange(2, matchingClosingParenthesis - 1);
            var argumentsTokenList = GetArgumentTokensList(innerExpressionTokens);
            if (!argumentsTokenList.IsSuccess)
            {
                return ParseExpressionResult.Invalid<FunctionCallExpression>(argumentsTokenList.ErrorMessage);
            }
            var arguments = argumentsTokenList.Result.Select(tokens =>
                ExpressionFactory.Parse(source.File, tokens, source.Line));
            var reference = source.CreateReference();

            var expression = new FunctionCallExpression(functionName, arguments.ToArray(), source, reference);
            return ParseExpressionResult.Success(expression);
        }

        private static ArgumentTokenParseResult GetArgumentTokensList(TokenList allArgumentTokens)
        {
            if (allArgumentTokens.Length == 0) return ArgumentTokenParseResult.Success();

            var result = new List<TokenList>();
            var argumentTokens = new List<Token>();

            foreach (var token in allArgumentTokens)
            {
                if (token is OperatorToken { Type: OperatorType.ArgumentSeparator })
                {
                    if (argumentTokens.Count == 0)
                    {
                        return ArgumentTokenParseResult.Failed(@"Invalid token ','. No tokens before comma.");
                    }

                    result.Add(new TokenList(argumentTokens.ToArray()));
                    argumentTokens = new List<Token>();
                }
                else
                {
                    argumentTokens.Add(token);
                }
            }

            if (argumentTokens.Count == 0)
            {
                return ArgumentTokenParseResult.Failed(@"Invalid token ','. No tokens before comma.");
            }

            result.Add(new TokenList(argumentTokens.ToArray()));

            return ArgumentTokenParseResult.Success(result);
        }


        public static bool IsValid(TokenList tokens)
        {
            return tokens.IsTokenType<StringLiteralToken>(0)
                   && tokens.OperatorToken(1, OperatorType.OpenParentheses);
        }

        public override IEnumerable<INode> GetChildren() => Arguments;

        protected override void Validate(IValidationContext context)
        {
            if (!BuiltInFunction.Contains(FunctionName))
            {
                context.Logger.Fail(Reference, $"Invalid function name: '{FunctionName}'");
            }
        }

        public override VariableType DeriveType(IValidationContext context) => BuiltInFunction.GetType(FunctionName);
    }

    public static class BuiltInFunction
    {
        private static readonly IDictionary<string, PrimitiveType> values = new Dictionary<string, PrimitiveType>
        {
            { "LOOKUP", PrimitiveType.Number }
        };

        public static bool Contains(string functionName)
        {
            return values.ContainsKey(functionName);
        }

        public static PrimitiveType GetType(string functionName)
        {
            return values.TryGetValue(functionName, out var value) ? value : null;
        }
    }
}