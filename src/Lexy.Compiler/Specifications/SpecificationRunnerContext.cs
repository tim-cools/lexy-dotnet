using System;
using System.Collections.Generic;
using System.Linq;
using Lexy.Compiler.Infrastructure;
using Lexy.Compiler.Language.Scenarios;
using Lexy.RunTime;
using Microsoft.Extensions.Logging;

namespace Lexy.Compiler.Specifications;

public class SpecificationRunnerContext : ISpecificationRunnerContext
{
    private readonly List<ISpecificationFileRunner> fileRunners = new();
    private readonly List<SpecificationsLogEntry> logEntries = new();
    private readonly ILogger<SpecificationsRunner> logger;
    private readonly DateTime startTimestamp;

    public int Failed { get; private set; }
    public IEnumerable<SpecificationsLogEntry> LogEntries => logEntries;
    public IReadOnlyCollection<ISpecificationFileRunner> FileRunners => fileRunners;

    public SpecificationRunnerContext(ILogger<SpecificationsRunner> logger)
    {
        startTimestamp = DateTime.Now;
        this.logger = logger;
    }

    public void Fail(Scenario scenario, string message, IEnumerable<string> errors)
    {
        Failed++;

        var entry = new SpecificationsLogEntry(scenario.Reference, scenario, true,
            $"FAILED - {scenario.Name}: {message}", errors);
        logEntries.Add(entry);

        logger.LogError("- FAILED  - {ScenarioName}: {Message}", scenario.Name, message);
        errors?.ForEach(message => this.logger.LogInformation("  {Message}", message));
    }

    public void LogGlobal(string message)
    {
        var entry = new SpecificationsLogEntry(null, null, false, message);
        logEntries.Add(entry);
        logger.LogInformation("{Message}", message);
    }

    public void LogTimeSpent()
    {
        var difference = DateTime.Now.Subtract(this.startTimestamp).TotalMilliseconds;
        var message = $"Time: {difference} milliseconds";

        var entry = new SpecificationsLogEntry(null, null, false, message);
        logEntries.Add(entry);
        logger.LogInformation("Time: {Difference} milliseconds", difference);
    }

    public void Success(Scenario scenario, IEnumerable<ExecutionLogEntry> logging = null)
    {
        var entry = new SpecificationsLogEntry(scenario.Reference, scenario, false, $"SUCCESS - {scenario.Name}", null,
            logging);
        logEntries.Add(entry);
        logger.LogInformation("- SUCCESS - {ScenarioName}", scenario.Name);
    }

    public void Add(ISpecificationFileRunner fileRunner)
    {
        fileRunners.Add(fileRunner);
    }

    public IEnumerable<IScenarioRunner> FailedScenariosRunners()
    {
        return fileRunners
            .SelectMany(runner => runner.ScenarioRunners)
            .Where(scenario => scenario.Failed);
    }

    public int CountScenarios()
    {
        return FileRunners.Select(fileRunner => fileRunner.CountScenarioRunners())
            .Aggregate((value, total) => value + total);
    }
}