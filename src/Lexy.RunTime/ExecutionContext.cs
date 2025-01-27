using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text.Json;
using Microsoft.Extensions.Logging;

namespace Lexy.RunTime;

public class ExecutionContext : IExecutionContext, IVariablesLogger
{
    private record CurrentEntry(ExecutionLogEntry Scope, ExecutionLogEntry Last);

    private readonly ILogger<ExecutionContext> logger;
    private readonly List<ExecutionLogEntry> entriesValue = new();
    private readonly Stack<CurrentEntry> entriesStack = new();
    private string fileName;
    private ExecutionLogEntry currentEntryScope;
    private ExecutionLogEntry lastEntry;

    public IReadOnlyList<ExecutionLogEntry> Entries => entriesValue;

    public ExecutionContext(ILogger<ExecutionContext> logger)
    {
        this.logger = logger;
    }

    public void SetFileName(string fileName)
    {
        this.fileName = fileName;
    }

    public void LogVariables(string message, int? lineNumber, object variablesObject)
    {
        LogLine(message, lineNumber, ConvertToVariables(variablesObject));
    }

    private static LogVariables ConvertToVariables(object variablesObject)
    {
        if (variablesObject == null) return new LogVariables();

        var builder = new LogVariablesBuilder();
        var fields = variablesObject.GetType().GetFields(BindingFlags.Instance | BindingFlags.Public);
        foreach (var field in fields)
        {
            var value = field.GetValue(variablesObject);
            builder.AddVariable(field.Name, value);
        }
        return builder.Build();
    }

    public ExecutionLogEntry LogLine(string message, int? lineNumber, LogVariables variables)
    {
        var entry = new ExecutionLogEntry(this, fileName, lineNumber, message, variables);
        if (currentEntryScope == null)
        {
            entriesValue.Add(entry);
        }
        else
        {
            currentEntryScope.AddEntry(entry);
        }

        lastEntry = entry;
        var indent = Indent();
        logger.LogDebug("{Indent}{LineNumber}:' {Message}'", indent, lineNumber, message);

        if (variables != null)
        {
            LogVariablesValues(variables, indent);
        }

        return entry;
    }

    private void LogVariablesValues(LogVariables variables, string indent)
    {
        foreach (var variable in variables)
        {
            logger.LogDebug("{Indent}  - {Name}: {Value}", indent, variable.Key, JsonSerializer.Serialize(variable.Value));
        }
    }

    public void LogChild(string message)
    {
        if (lastEntry == null)
        {
            throw new InvalidOperationException("lastEntry not set");
        }

        lastEntry.AddEntry(new ExecutionLogEntry(this, this.fileName, null, message, new LogVariables()));
        var indent = Indent();
        logger.LogDebug("{Indent}  '{Message}'", indent, message);
    }

    private string Indent() => new string(' ', entriesStack.Count * 2);

    public void LogVariable(LogVariables variables)
    {
        var indent = Indent();
        LogVariablesValues(variables, indent);
    }

    public void OpenScope(string message, int lineNumber)
    {
        entriesStack.Push(new CurrentEntry(currentEntryScope, lastEntry));
        LogLine(message, lineNumber, new LogVariables());
        currentEntryScope = lastEntry;
    }

    public void CloseScope()
    {
        var entry = entriesStack.Pop();
        currentEntryScope = entry?.Scope;
        lastEntry = entry?.Last;
    }

    public void UseLastNodeAsScope()
    {
        entriesStack.Push(new CurrentEntry(currentEntryScope, lastEntry));
        currentEntryScope = lastEntry;
    }

    public void RevertToParentScope()
    {
        var entry = entriesStack.Count > 0 ? entriesStack.Pop() : null;
        currentEntryScope = entry?.Scope;
        lastEntry = entry?.Last;
    }
}