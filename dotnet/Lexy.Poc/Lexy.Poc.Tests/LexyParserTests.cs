using System;
using Lexy.Poc.Core.Parser;
using NUnit.Framework;
using Shouldly;

namespace Lexy.Poc
{
    public class TokenizerTests
    {
        [Test]
        public void TestFunctionKeyword()
        {
            var line = new Line(1, "Function: TestSimpleReturn", Array.Empty<string>());
            var context = new ParserContext();
            context.ProcessLine(line);

            context.AssertTokens(validator => validator
                .Count(2)
                .Type<KeywordToken>(0)
                .Value(0, "Function:")
                .Type<LiteralToken>(1)
                .Value(1, "TestSimpleReturn"));
        }

        [Test]
        public void TestResultKeyword()
        {
            var line = new Line(1, "  Results", Array.Empty<string>());
            var context = new ParserContext();
            context.ProcessLine(line);

            context.AssertTokens(validator => validator
                .Count(1)
                .Type<KeywordToken>(0)
                .Value(0, "Results"));
        }

        [Test]
        public void TestParameterDeclaration()
        {
            var line = new Line(1, "    number Result", Array.Empty<string>());
            var context = new ParserContext();
            context.ProcessLine(line);

            context.AssertTokens(validator => validator
                .Count(2)
                .Type<LiteralToken>(0)
                .Value(0, "number")
                .Type<LiteralToken>(1)
                .Value(1, "Result"));
        }
    }

    public static class ParserContextExtensions
    {
        public static void AssertTokens(this ParserContext context, Action<TokenValidator> tokenValidator)
        {
            if (context == null) throw new ArgumentNullException(nameof(context));

            var validator = new TokenValidator(context);
            tokenValidator(validator);
            if (!validator.Success)
            {
                throw new InvalidOperationException(context.ErrorMessages());
            }
        }
    }

    public class LexyParserTests
    {
        [Test]
        public void TestSimpleReturn()
        {
            var code = @"Function: TestSimpleReturn
  Results
    number Result
  Code
    Result = 777";

            var parser = new LexyParser();
            var script = parser.ParseFunction(code);

            script.Name.Value.ShouldBe("TestSimpleReturn");
            script.Results.Variables.Count.ShouldBe(1);
            script.Results.Variables[0].Name.ShouldBe("Result");
            script.Results.Variables[0].Type.ShouldBe("number");
            script.Code.Lines.Count.ShouldBe(1);
            script.Code.Lines[0].ShouldBe("Result = 777");
        }

        [Test]
        public void TestFunctionKeywords()
        {
            var code = @"Function: ValidateFunctionKeywords
# Validate function keywords
  Parameters
  Results
  Code";

            var parser = new LexyParser();
            parser.ParseFunction(code);

        }
    }
}