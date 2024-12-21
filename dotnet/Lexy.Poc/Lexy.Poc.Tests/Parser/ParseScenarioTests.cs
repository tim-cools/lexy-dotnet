using Lexy.Poc.Core.Parser;
using NUnit.Framework;
using Shouldly;

namespace Lexy.Poc.Parser
{
    public class ParseScenarioTests : ScopedServicesTestFixture
    {
        [Test]
        public void TestValidScenarioKeyword()
        {
            var code = @"Scenario: TestScenario";

            var parser = GetService<ILexyParser>();
            var scenario = parser.ParseScenario(code);

            scenario.Name.Value.ShouldBe("TestScenario");
        }

        [Test]
        public void TestValidScenario()
        {
            var code = @"Scenario: TestScenario
  Function TestScenarioFunction
  Parameters
    Value = 123
  Results
    Result = 456";

            var parser = GetService<ILexyParser>();
            var scenario = parser.ParseScenario(code);

            scenario.Name.Value.ShouldBe("TestScenario");
            scenario.FunctionName.Value.ShouldBe("TestScenarioFunction");
            scenario.Parameters.Assignments.Count.ShouldBe(1);
            scenario.Parameters.Assignments[0].Name.ShouldBe("Value");
            scenario.Parameters.Assignments[0].Expression.ToString().ShouldBe("123");
            scenario.Results.Assignments.Count.ShouldBe(1);
            scenario.Results.Assignments[0].Name.ShouldBe("Result");
            scenario.Results.Assignments[0].Expression.ToString().ShouldBe("456");
        }

        [Test]
        public void TestInvalidScenario()
        {
            var code = @"Scenario: TestScenario
  Functtion TestScenarioFunction
  Parameters
    Value = 123
  Results
    Result = 456";

            var parser = GetService<ILexyParser>();
            var scenario = parser.ParseScenario(code);

            var context = GetService<IParserContext>();
            context.Logger.ComponentHasErrors(scenario).ShouldBeTrue();

            var errors = context.Logger.ComponentFailedMessages(scenario);
            errors.Length.ShouldBe(1);
            errors[0].ShouldBe("2: ERROR - Invalid token 'Functtion'. Keyword expected.");
        }

        [Test]
        public void TestInvalidNumberValueScenario()
        {
            var code = @"Scenario: TestScenario
  Function TestScenarioFunction
  Parameters
    Value = 12d3
  Results
    Result = 456";

            var parser = GetService<ILexyParser>();
            var scenario = parser.ParseScenario(code);

            var context = GetService<IParserContext>();
            context.Logger.ComponentHasErrors(scenario).ShouldBeTrue();

            var errors = context.Logger.ComponentFailedMessages(scenario);
            errors.Length.ShouldBe(1);
            errors[0].ShouldBe("4: ERROR - Invalid token at 14: Invalid number token character: d");
        }
    }
}