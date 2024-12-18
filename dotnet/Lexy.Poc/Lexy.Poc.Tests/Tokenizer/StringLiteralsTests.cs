using NUnit.Framework;

namespace Lexy.Poc.Tokenizer
{
    public class StringLiteralsTests
    {
        [Test]
        public void TestQuotedLiteral()
        {
            TestContext
                .TestLine(@"   ""This is a quoted literal""")
                .ValidateTokens()
                    .Count(1)
                    .QuotedString(0, "This is a quoted literal")
                .Assert();
        }

        [Test]
        public void TestStringLiteral()
        {
            TestContext
                .TestLine(@"   ThisIsAStringLiteral")
                .ValidateTokens()
                    .Count(1)
                    .StringLiteral(0, "ThisIsAStringLiteral")
                .Assert();
        }
    }
}