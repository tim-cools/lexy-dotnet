using Lexy.Poc.Core.Language.Expressions;
using Lexy.Poc.Core.Parser.Tokens;
using NUnit.Framework;
using Shouldly;

namespace Lexy.Poc.Parser.ExpressionParser
{
    public class AssignmentExpressionTests : ScopedServicesTestFixture
    {
        [Test]
        public void Addition()
        {
            var expression = this.ParseExpression("A = B + C");
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
            var expression = this.ParseExpression("A = B + C * 12");
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
                        multiplication.Right.ValidateOfType<LiteralExpression>(literal =>
                        {
                            literal.Literal.ValidateOfType<NumberLiteralToken>(number =>
                                number.NumberValue.ShouldBe(12m));
                        });
                    });
                });
            });
        }
    }
}