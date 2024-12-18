using Lexy.Poc.Core.Parser;
using NUnit.Framework;
using Shouldly;

namespace Lexy.Poc.Parser
{
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
            script.Code.Lines[0].Value.ShouldBe("Result=777");
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