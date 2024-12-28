using System;
using Lexy.Compiler.Parser.Tokens;
using Lexy.Poc.Parser.ExpressionParser;
using Shouldly;

namespace Lexy.Poc.Parser;

public static class TokenTestExtensions
{
    public static void ValidateStringLiteralToken(this Token token, string value)
    {
        if (token == null) throw new ArgumentNullException(nameof(token));
        token.ValidateOfType<StringLiteralToken>(actual => actual.Value.ShouldBe(value));
    }
}