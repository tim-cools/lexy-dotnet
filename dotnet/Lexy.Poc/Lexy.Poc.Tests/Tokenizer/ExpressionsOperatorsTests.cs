using Lexy.Poc.Core.Parser;
using Lexy.Poc.Core.Parser.Tokens;
using NUnit.Framework;

namespace Lexy.Poc.Tokenizer
{
    public class ExpressionsOperatorsTests
    {
        [Test]
        public void TestAdditionOperator()
        {
            TestContext
                .TestLine(@"  X = Y + 1")
                .ValidateTokens()
                    .Count(5)
                    .StringLiteral(0, "X")
                    .Operator(1, OperatorType.Assignment)
                    .StringLiteral(2, "Y")
                    .Operator(3, OperatorType.Addition)
                    .IntLiteral(4, 1)
                .Assert();
        }

        [Test]
        public void TestSubtractionOperator()
        {
            TestContext
                .TestLine(@"  X = Y - 1")
                .ValidateTokens()
                    .Count(5)
                    .StringLiteral(0, "X")
                    .Operator(1, OperatorType.Assignment)
                    .StringLiteral(2, "Y")
                    .Operator(3, OperatorType.Subtraction)
                    .IntLiteral(4, 1)
                .Assert();
        }

        [Test]
        public void TestMultiplicationOperator()
        {
            TestContext
                .TestLine(@"  X = Y * 1")
                .ValidateTokens()
                    .Count(5)
                    .StringLiteral(0, "X")
                    .Operator(1, OperatorType.Assignment)
                    .StringLiteral(2, "Y")
                    .Operator(3, OperatorType.Multiplication)
                    .IntLiteral(4, 1)
                .Assert();
        }

        [Test]
        public void TestDivisionOperator()
        {
            TestContext
                .TestLine(@"  X = Y / 1")
                .ValidateTokens()
                    .Count(5)
                    .StringLiteral(0, "X")
                    .Operator(1, OperatorType.Assignment)
                    .StringLiteral(2, "Y")
                    .Operator(3, OperatorType.Division)
                    .IntLiteral(4, 1)
                .Assert();
        }

        [Test]
        public void TestModulusOperator()
        {
            TestContext
                .TestLine(@"  X = Y % 1")
                .ValidateTokens()
                    .Count(5)
                    .StringLiteral(0, "X")
                    .Operator(1, OperatorType.Assignment)
                    .StringLiteral(2, "Y")
                    .Operator(3, OperatorType.Modulus)
                    .IntLiteral(4, 1)
                .Assert();
        }

        [Test]
        public void TestParenthesesOperator()
        {
            TestContext
                .TestLine(@"  X = (Y)")
                .ValidateTokens()
                    .Count(5)
                    .StringLiteral(0, "X")
                    .Operator(1, OperatorType.Assignment)
                    .Operator(2, OperatorType.OpenParentheses)
                    .StringLiteral(3, "Y")
                    .Operator(4, OperatorType.CloseParentheses)
                .Assert();
        }

        [Test]
        public void TestBracketsOperator()
        {
            TestContext
                .TestLine(@"  X = A[1]")
                .ValidateTokens()
                    .Count(6)
                    .StringLiteral(0, "X")
                    .Operator(1, OperatorType.Assignment)
                    .StringLiteral(2, "A")
                    .Operator(3, OperatorType.OpenBrackets)
                    .IntLiteral(4, 1)
                    .Operator(5, OperatorType.CloseBrackets)
                .Assert();
        }

        [Test]
        public void TestLessThanOperator()
        {
            TestContext
                .TestLine(@"  X = A < 7")
                .ValidateTokens()
                    .Count(5)
                    .StringLiteral(0, "X")
                    .Operator(1, OperatorType.Assignment)
                    .StringLiteral(2, "A")
                    .Operator(3, OperatorType.LessThan)
                    .IntLiteral(4, 7)
                .Assert();
        }

        [Test]
        public void TestLessOrEqualThanOperator()
        {
            TestContext
                .TestLine(@"  X = A <= 7")
                .ValidateTokens()
                    .Count(5)
                    .StringLiteral(0, "X")
                    .Operator(1, OperatorType.Assignment)
                    .StringLiteral(2, "A")
                    .Operator(3, OperatorType.LessThanOrEqual)
                    .IntLiteral(4, 7)
                .Assert();
        }

        [Test]
        public void TestGreaterThanOperator()
        {
            TestContext
                .TestLine(@"  X = A > 7")
                .ValidateTokens()
                    .Count(5)
                    .StringLiteral(0, "X")
                    .Operator(1, OperatorType.Assignment)
                    .StringLiteral(2, "A")
                    .Operator(3, OperatorType.GreaterThan)
                    .IntLiteral(4, 7)
                .Assert();
        }

        [Test]
        public void TestGreaterOrEqualThanOperator()
        {
            TestContext
                .TestLine(@"  X = A >= 7")
                .ValidateTokens()
                    .Count(5)
                    .StringLiteral(0, "X")
                    .Operator(1, OperatorType.Assignment)
                    .StringLiteral(2, "A")
                    .Operator(3, OperatorType.GreaterThanOrEqual)
                    .IntLiteral(4, 7)
                .Assert();
        }



        [Test]
        public void TestFunctionCallOperator()
        {
            TestContext
                .TestLine(@"  X = abs(Y + 8)")
                .ValidateTokens()
                    .Count(8)
                    .StringLiteral(0, "X")
                    .Operator(1, OperatorType.Assignment)
                    .StringLiteral(2, "abs")
                    .Operator(3, OperatorType.OpenParentheses)
                    .StringLiteral(4, "Y")
                    .Operator(5, OperatorType.Addition)
                    .IntLiteral(6, 8)
                    .Operator(7, OperatorType.CloseParentheses)
                .Assert();
        }
    }
}