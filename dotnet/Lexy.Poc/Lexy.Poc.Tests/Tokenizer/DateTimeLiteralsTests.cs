using NUnit.Framework;

namespace Lexy.Poc.Tokenizer
{
    public class DateTimeLiteralsTests
    {
        [Test]
        public void TestQuotedLiteral()
        {
            TestContext
                .TestLine(@"   OutDateTime = d""2024/12/16 13:26:55""")
                .ValidateTokens()
                .Count(3)
                .DateTime(2, 2024, 12, 16, 13, 26, 55)
                .Assert();
        }
    }
}