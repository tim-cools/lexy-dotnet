using Lexy.Poc.Core.Parser.Tokens;
using NUnit.Framework;

namespace Lexy.Poc.Tokenizer
{
    public class KeywordTests
    {
        [Test]
        public void TestFunctionKeyword()
        {
            TestContext
                .TestLine("Function: TestSimpleReturn")
                .ValidateTokens()
                    .Count(2)
                    .Keyword(0, "Function:")
                    .StringLiteral(1, "TestSimpleReturn")
                .Assert();
        }

        [Test]
        public void TestResultKeyword()
        {
            TestContext
                .TestLine("  Results")
                .ValidateTokens()
                    .Count(1)
                    .Keyword(0, "Results")
                .Assert();
        }

        [Test]
        public void TestExpectErrorKeywordWithQuotedLiteral()
        {
            TestContext
                .TestLine(@"  ExpectError ""Invalid token 'Paraeters'""")
                .ValidateTokens()
                    .Count(2)
                    .Keyword(0, "ExpectError")
                    .QuotedString(1, "Invalid token 'Paraeters'")
                .Assert();
        }

        [Test]
        public void TestExpectErrorKeywordWithQuotedAndInvalidChar()
        {
            TestContext
                .TestLine(@"  ExpectError ""Invalid token 'Paraeters'"".")
                .ValidateTokens()
                .ExpectError(@"0: Invalid character at 41 '.'")
                .Assert();
        }

        [Test]
        public void TestAssignmentWithMemberAccess()
        {
            TestContext
                .TestLine(@"  Value = ValidateEnumKeyword.Second")
                .ValidateTokens()
                .Count(3)
                .StringLiteral(0, "Value")
                .Operator(1, OperatorType.Assignment)
                .MemberAccess(2, "ValidateEnumKeyword.Second")
                .Assert();
        }

        [Test]
        public void TestAssignmentWithDoubleMemberAccess()
        {
            TestContext
                .TestLine(@"  Value = ValidateEnumKeyword..Second")
                .ValidateTokens()
                .ExpectError("0: Invalid token 30: Unexpected character: '.'. Member accessor should be followed by member name.")
                .Assert();
        }

        [Test]
        public void TestAssignmentWithMemberAccessWithoutLastMember()
        {
            TestContext
                .TestLine(@"  Value = ValidateEnumKeyword.")
                .ValidateTokens()
                .ExpectError("0: Invalid token at end of line: InvalidToken (Unexpected end of line. Member accessor should be followed by member name.)")
                .Assert();
        }


    }

}