using Lexy.Poc.Core.Parser;
using NUnit.Framework;

namespace Lexy.Poc.Tokenizer
{
    public class TableTests
    {
        [Test]
        public void TestTableHeader()
        {
            TestContext
                .TestLine(@"  | int Value | string Result |")
                .ValidateTokens()
                    .Count(7)
                    .Type<TableSeparatorToken>(0)
                    .StringLiteral(1, "int")
                    .StringLiteral(2, "Value")
                    .Type<TableSeparatorToken>(3)
                    .StringLiteral(4, "string")
                    .StringLiteral(5, "Result")
                    .Type<TableSeparatorToken>(6)
                .Assert();
        }

        [Test]
        public void TestTableRow()
        {
            TestContext
                .TestLine(@"  | 7 | 8 |")
                .ValidateTokens()
                    .Count(5)
                    .Type<TableSeparatorToken>(0)
                    .IntLiteral(1, 7)
                    .Type<TableSeparatorToken>(2)
                    .IntLiteral(3, 8)
                    .Type<TableSeparatorToken>(4)
                .Assert();
        }

    }
}