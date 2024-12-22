using Lexy.Poc.Core;
using Lexy.Poc.Core.Parser;
using Lexy.Poc.Core.Parser.Tokens;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;
using Shouldly;

namespace Lexy.Poc.Parser
{
    public class TokensListTests
    {
        [Test]
        public void TokensFrom()
        {
            var list = new TokenList(new []
            {
                new StringLiteralToken("123"),
                new StringLiteralToken("456"),
                new StringLiteralToken("789"),
            });

            var result = list.TokensFrom(1);
            result.Length.ShouldBe(2);
            result[0].ValidateOfType<StringLiteralToken>(value => value.Value.ShouldBe("456"));
            result[1].ValidateOfType<StringLiteralToken>(value => value.Value.ShouldBe("789"));
        }

        [Test]
        public void TokensStart()
        {
            var list = new TokenList(new []
            {
                new StringLiteralToken("123"),
                new StringLiteralToken("456"),
                new StringLiteralToken("789"),
            });

            var result = list.TokensFromStart(2);
            result.Length.ShouldBe(2);
            result[0].ValidateOfType<StringLiteralToken>(value => value.Value.ShouldBe("123"));
            result[1].ValidateOfType<StringLiteralToken>(value => value.Value.ShouldBe("456"));
        }

        [Test]
        public void TokensRange()
        {
            var list = new TokenList(new []
            {
                new StringLiteralToken("1111"),
                new StringLiteralToken("2222"),
                new StringLiteralToken("3333"),
                new StringLiteralToken("4444"),
                new StringLiteralToken("5555"),
            });

            var result = list.TokensRange(1, 3);
            result.Length.ShouldBe(3);
            result[0].ValidateOfType<StringLiteralToken>(value => value.Value.ShouldBe("2222"));
            result[1].ValidateOfType<StringLiteralToken>(value => value.Value.ShouldBe("3333"));
            result[2].ValidateOfType<StringLiteralToken>(value => value.Value.ShouldBe("4444"));
        }
    }
}