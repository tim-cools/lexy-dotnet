using System;
using Lexy.Poc.Core.Language.Expressions;
using Lexy.Poc.Core.Parser;
using Lexy.Poc.Core.Parser.Tokens;
using NUnit.Framework;
using Shouldly;
using Expression = Lexy.Poc.Core.Language.Expressions.Expression;

namespace Lexy.Poc.Parser
{
    public class ExpressionParseTests : ScopedServicesTestFixture
    {
        [Test]
        public void Addition()
        {
            var expressionValue = "A = B + C";

            var context = GetService<IParserContext>();
            var tokenizer = GetService<ITokenizer>();
            var line = new Line(0, expressionValue);
            line.Tokenize(tokenizer, context);

            var expression = ExpressionFactory.Parse(line.Tokens, line);
            expression.ValidateOfType<AssignmentExpression>(assignmentExpression =>
            {
                assignmentExpression.VariableName.ShouldBe("A");
                assignmentExpression.Assignment.ValidateOfType<BinaryExpression>(addition =>
                {
                    addition.Operator.ShouldBe(ExpressionOperator.Addition);
                    addition.Left.ValidateOfType<VariableExpression>(left =>
                        left.VariableName.ShouldBe("B"));
                    addition.Right.ValidateOfType<VariableExpression>(right =>
                        right.VariableName.ShouldBe("C"));
                });
            });
        }

        [Test]
        public void AdditionAndMultiplication()
        {
            var expressionValue = "A = B + C * 12";

            var context = GetService<IParserContext>();
            var tokenizer = GetService<ITokenizer>();
            var line = new Line(0, expressionValue);
            line.Tokenize(tokenizer, context);

            var expression = ExpressionFactory.Parse(line.Tokens, line);
            expression.ValidateOfType<AssignmentExpression>(assignment =>
            {
                assignment.VariableName.ShouldBe("A");
                assignment.Assignment.ValidateOfType<BinaryExpression>(addition =>
                {
                    addition.Operator.ShouldBe(ExpressionOperator.Addition);
                    addition.Left.ValidateOfType<VariableExpression>(left =>
                        left.VariableName.ShouldBe("B"));
                    addition.Right.ValidateOfType<BinaryExpression>(multiplication =>
                    {
                        multiplication.Operator.ShouldBe(ExpressionOperator.Multiplication);
                        multiplication.Left.ValidateOfType<VariableExpression>(left =>
                            left.VariableName.ShouldBe("C"));
                        multiplication.Right.ValidateOfType<LiteralExpression>(Literal =>
                        {
                            Literal.Literal.ValidateOfType<NumberLiteralToken>(number => number.NumberValue.ShouldBe(12m));
                        });
                    });
                });
            });
        }

        [Test]
        public void ParenthesizedExpression()
        {
            var expressionValue = "(A)";

            var context = GetService<IParserContext>();
            var tokenizer = GetService<ITokenizer>();
            var line = new Line(0, expressionValue);
            line.Tokenize(tokenizer, context);

            var expression = ExpressionFactory.Parse(line.Tokens, line);
            expression.ValidateOfType<ParenthesizedExpression>(assignment =>
                assignment.Expression.ValidateOfType<VariableExpression>(variable =>
                    variable.VariableName.ShouldBe("A")));
        }

        [Test]
        public void NestedParenthesizedExpression()
        {
            var expressionValue = "(5 * (3 + A))";

            var context = GetService<IParserContext>();
            var tokenizer = GetService<ITokenizer>();
            var line = new Line(0, expressionValue);
            line.Tokenize(tokenizer, context);

            var expression = ExpressionFactory.Parse(line.Tokens, line);
            expression.ValidateOfType<ParenthesizedExpression>(parenthesis =>
                parenthesis.Expression.ValidateOfType<BinaryExpression>(multiplication =>
                    multiplication.Right.ValidateOfType<ParenthesizedExpression>(inner =>
                        inner.Expression.ValidateOfType<BinaryExpression>(addition =>
                            addition.Operator.ShouldBe(ExpressionOperator.Addition)))));
        }

        [Test]
        public void InvalidParenthesizedExpression()
        {
            var expressionValue = "(A";

            var context = GetService<IParserContext>();
            var tokenizer = GetService<ITokenizer>();
            var line = new Line(0, expressionValue);
            line.Tokenize(tokenizer, context);

            TestContext.ExpectException(
                () => ExpressionFactory.Parse(line.Tokens, line),
                "(ParenthesizedExpression) No closing parentheses found.");
        }

        [Test]
        public void InvalidNestedParenthesizedExpression()
        {
            var expressionValue = "(5 * (3 + A)";

            var context = GetService<IParserContext>();
            var tokenizer = GetService<ITokenizer>();
            var line = new Line(0, expressionValue);
            line.Tokenize(tokenizer, context);

            TestContext.ExpectException(
                () =>ExpressionFactory.Parse(line.Tokens, line),
                "(ParenthesizedExpression) No closing parentheses found.");
        }
    }

    public static class ExpressionTestExtensions
    {
        public static void ValidateOfType<T>(this object value, Action<T> validate) where T : class
        {
            if (value == null) throw new ArgumentNullException(nameof(value));

            var specificValue = value as T;
            if (specificValue == null)
            {
                throw new InvalidOperationException(
                    $"Values '{value.GetType().Name}' should be of type ‘{typeof(T).Name}'");
            }

            validate(specificValue);
        }
    }
}