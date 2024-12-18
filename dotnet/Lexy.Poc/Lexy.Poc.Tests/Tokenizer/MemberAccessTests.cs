using Lexy.Poc.Core.Parser;
using NUnit.Framework;

namespace Lexy.Poc.Tokenizer
{
    public class MemberAccessTests
    {
        [Test]
        public void TestTableHeader()
        {
            TestContext
                .TestLine(@"    Source.Member")
                .ValidateTokens()
                    .Count(1)
                    .Type<MemberAccessLiteral>(0)
                    .MemberAccess(0, "Source.Member")
                .Assert();
        }
    }
}