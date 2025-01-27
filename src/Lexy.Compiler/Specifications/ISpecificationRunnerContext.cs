using System.Collections.Generic;
using Lexy.Compiler.Language.Scenarios;
using Lexy.RunTime;

namespace Lexy.Compiler.Specifications;

public interface ISpecificationRunnerContext
{
    int Failed { get; }
    IReadOnlyCollection<ISpecificationFileRunner> FileRunners { get; }

    void Fail(Scenario scenario, string message, IEnumerable<string> errors);
    void LogGlobal(string message);
    void LogTimeSpent();

    void Success(Scenario scenario, IEnumerable<ExecutionLogEntry> logging = null);
    void Add(ISpecificationFileRunner fileRunner);
    IEnumerable<IScenarioRunner> FailedScenariosRunners();
    int CountScenarios();
}