using System;
using System.Collections.Generic;
using System.Linq;
using Lexy.Poc.Core.Parser;
using Lexy.Poc.Core.Parser.Tokens;

namespace Lexy.Poc.Core.Language.Expressions
{
    public class BinaryExpression : Expression
    {
        private class OperatorEntry
        {
            public OperatorType OperatorType { get; }
            public ExpressionOperator ExpressionOperator { get; }

            public OperatorEntry(OperatorType operatorType, ExpressionOperator expressionOperator)
            {
                OperatorType = operatorType;
                ExpressionOperator = expressionOperator;
            }
        }

        private class TokenIndex
        {
            public int Index { get; }
            public OperatorType OperatorType { get; }
            public ExpressionOperator ExpressionOperator { get; }

            public TokenIndex(int index, OperatorType operatorType, ExpressionOperator expressionOperator)
            {
                Index = index;
                OperatorType = operatorType;
                ExpressionOperator = expressionOperator;
            }
        }

        private static readonly IList<OperatorEntry> supportedOperatorsByPriority = new List<OperatorEntry>
        {
            new OperatorEntry(OperatorType.Multiplication, ExpressionOperator.Multiplication),
            new OperatorEntry(OperatorType.Division, ExpressionOperator.Division),
            new OperatorEntry(OperatorType.Modulus, ExpressionOperator.Modulus),

            new OperatorEntry(OperatorType.Addition, ExpressionOperator.Addition),
            new OperatorEntry(OperatorType.Subtraction, ExpressionOperator.Subtraction),

            new OperatorEntry(OperatorType.GreaterThan, ExpressionOperator.GreaterThan),
            new OperatorEntry(OperatorType.GreaterThanOrEqual, ExpressionOperator.GreaterThanOrEqual),
            new OperatorEntry(OperatorType.LessThan, ExpressionOperator.LessThan),
            new OperatorEntry(OperatorType.LessThanOrEqual, ExpressionOperator.LessThanOrEqual),

            new OperatorEntry(OperatorType.Equals, ExpressionOperator.Equals),
            new OperatorEntry(OperatorType.NotEqual, ExpressionOperator.NotEqual),

            new OperatorEntry(OperatorType.And, ExpressionOperator.And),
            new OperatorEntry(OperatorType.Or, ExpressionOperator.Or),
        };

        public Expression Left { get; }
        public Expression Right { get; }
        public ExpressionOperator Operator { get; }

        private BinaryExpression(Expression left, Expression right, ExpressionOperator operatorValue, ExpressionSource source) : base(source)
        {
            Left = left;
            Right = right;
            Operator = operatorValue;
        }

        public static ParseExpressionResult Parse(ExpressionSource source)
        {
            var tokens = source.Tokens;
            var supportedTokens = GetCurrentLevelSupportedTokens(tokens);
            var lowerPriorityOperation = GetLowestPriorityOperation(supportedTokens);
            if (lowerPriorityOperation == null)
            {
                return ParseExpressionResult.Invalid<BinaryExpression>("No valid Operator token found.");
            }

            var leftTokens = tokens.TokensRange(0, lowerPriorityOperation.Index - 1);
            if (leftTokens.Length == 0)
            {
                return ParseExpressionResult.Invalid<BinaryExpression>(
                    $"No tokens left from: {lowerPriorityOperation.Index} ({tokens})");
            }
            var rightTokens = tokens.TokensFrom(lowerPriorityOperation.Index + 1);
            if (rightTokens.Length == 0)
            {
                return ParseExpressionResult.Invalid<BinaryExpression>(
                    $"No tokens right from: {lowerPriorityOperation.Index} ({tokens})");
            }

            var left = ExpressionFactory.Parse(leftTokens, source.Line);
            var right = ExpressionFactory.Parse(rightTokens, source.Line);
            var operatorValue = lowerPriorityOperation.ExpressionOperator;

            var binaryExpression = new BinaryExpression(left, right, operatorValue, source);
            return ParseExpressionResult.Success(binaryExpression) ;
        }

        private static TokenIndex GetLowestPriorityOperation(IList<TokenIndex> supportedTokens)
        {
            foreach (var supportedOperator in supportedOperatorsByPriority.Reverse())
            {
                foreach (var supportedToken in supportedTokens)
                {
                    if (supportedOperator.OperatorType == supportedToken.OperatorType)
                    {
                        return supportedToken;
                    }
                }
            }

            return null;
        }

        public static bool IsValid(TokenList tokens)
        {
            var supportedTokens = GetCurrentLevelSupportedTokens(tokens);
            return supportedTokens.Count > 0;
        }

        private static IList<TokenIndex> GetCurrentLevelSupportedTokens(TokenList tokens)
        {
            if (tokens == null) throw new ArgumentNullException(nameof(tokens));

            var result = new List<TokenIndex>();
            var countParentheses = 0;
            var countBrackets = 0;
            for (var index = 0; index < tokens.Length; index++)
            {
                var token = tokens[index];
                if (!(token is OperatorToken operatorToken)) continue;

                switch (operatorToken.Type)
                {
                    case OperatorType.OpenParentheses:
                        countParentheses++;
                        break;
                    case OperatorType.CloseParentheses:
                        countParentheses--;
                        break;
                    case OperatorType.OpenBrackets:
                        countBrackets++;
                        break;
                    case OperatorType.CloseBrackets:
                        countBrackets--;
                        break;
                }

                if (countBrackets != 0 || countParentheses != 0) continue;

                var supported = IsSupported(operatorToken.Type);
                if (supported != null)
                {
                    result.Add(new TokenIndex(index, operatorToken.Type, supported.ExpressionOperator));
                }
            }
            return result;
        }

        private static OperatorEntry IsSupported(OperatorType operatorTokenType) =>
            supportedOperatorsByPriority.FirstOrDefault(entry => entry.OperatorType == operatorTokenType);


        protected override IEnumerable<INode> GetChildren()
        {
            yield return Left;
            yield return Right;
        }

        protected override void Validate(IParserContext context)
        {
        }
    }

    public class VariableDeclarationExpression : Expression
    {
        public VariableType VariableType { get; }
        public string VariableName { get; }
        public Expression Assignment { get; }

        private VariableDeclarationExpression(VariableType variableType, string variableName, Expression assignment, ExpressionSource source) : base(source)
        {
            VariableType = variableType ?? throw new ArgumentNullException(nameof(variableType));
            VariableName = variableName ?? throw new ArgumentNullException(nameof(variableName));
            Assignment = assignment;
        }

        public static ParseExpressionResult Parse(ExpressionSource source)
        {
            var tokens = source.Tokens;
            if (!IsValid(tokens))
            {
                return ParseExpressionResult.Invalid<VariableDeclarationExpression>("Invalid expression.");
            }

            var type = VariableType.Parse(tokens.TokenValue(0));
            var name = tokens.TokenValue(1);
            var assignment = tokens.Length > 3 ? ExpressionFactory.Parse(tokens.TokensFrom(3), source.Line) : null;
            var expression = new VariableDeclarationExpression(type, name, assignment, source);

            return ParseExpressionResult.Success(expression);
        }

        public static bool IsValid(TokenList tokens)
        {
            return tokens.Length == 2
                   && tokens.IsTokenType<StringLiteralToken>(0)
                   && tokens.IsTokenType<StringLiteralToken>(1)
                || tokens.Length >= 4
                   && tokens.IsTokenType<StringLiteralToken>(0)
                   && tokens.IsTokenType<StringLiteralToken>(1)
                   && tokens.OperatorToken(2, OperatorType.Assignment);
        }

        protected override IEnumerable<INode> GetChildren()
        {
            yield return Assignment;
        }

        protected override void Validate(IParserContext context)
        {
        }
    }
}