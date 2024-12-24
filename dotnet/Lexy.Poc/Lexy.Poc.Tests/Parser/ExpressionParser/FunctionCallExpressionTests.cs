using Lexy.Poc.Core.Language.Expressions;
using NUnit.Framework;
using Shouldly;

namespace Lexy.Poc.Parser.ExpressionParser
{
    public class FunctionCallExpressionTests : ScopedServicesTestFixture
    {
        [Test]
        public void FunctionCallExpression()
        {
            var expression = this.ParseExpression("func(y)");
            expression.ValidateOfType<FunctionCallExpression>(functionCallExpression =>
            {
                functionCallExpression.FunctionName.ShouldBe("func");
                functionCallExpression.Arguments.Length.ShouldBe(1);
                functionCallExpression.Arguments[0].ValidateVariableExpression("y");
            });
        }

        [Test]
        public void NestedParenthesizedExpression()
        {
            var expression = this.ParseExpression("func(5 * (3 + A))");
            expression.ValidateOfType<FunctionCallExpression>(functionCall =>
            {
                functionCall.FunctionName.ShouldBe("func");

                functionCall.Arguments.Length.ShouldBe(1);
                functionCall.Arguments[0].ValidateOfType<BinaryExpression>(multiplication =>
                    multiplication.Right.ValidateOfType<ParenthesizedExpression>(inner =>
                        inner.Expression.ValidateOfType<BinaryExpression>(addition =>
                            addition.Operator.ShouldBe(ExpressionOperator.Addition))));
            });
        }

        [Test]
        public void InvalidParenthesizedExpression()
        {
            this.ParseExpressionExpectException(
                "func(A",
                "(FunctionCallExpression) No closing parentheses found.");
        }


        [Test]
        public void InvalidNestedParenthesizedExpression()
        {
            this.ParseExpressionExpectException(
                "func(5 * (3 + A)",
            "(FunctionCallExpression) No closing parentheses found.");
        }
    }
}