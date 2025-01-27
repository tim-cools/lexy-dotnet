using System.Collections.Generic;

namespace Lexy.RunTime;

public class ExecutionLogEntry
{
    private List<ExecutionLogEntry> entries = new();
    private readonly IVariablesLogger variablesLogger;

    public string FileName { get; }
    public int? LineNumber { get; }
    public string Message { get; }
    public LogVariables ReadVariables { get; }
    public LogVariables WriteVariables { get; private set; }
    public IReadOnlyList<ExecutionLogEntry> Entries => entries;

    public ExecutionLogEntry(IVariablesLogger variablesLogger, string fileName, int? lineNumber, string message,
        LogVariables variables)
    {
        FileName = fileName;
        LineNumber = lineNumber;
        Message = message;
        this.variablesLogger = variablesLogger;
        ReadVariables = variables;
    }

    public void AddEntry(ExecutionLogEntry entry)
    {
        entries.Add(entry);
    }

    public void AddWriteVariables(LogVariables variables)
    {
        WriteVariables = variables;
        variablesLogger.LogVariable(WriteVariables);
    }

    public override string ToString()
    {
        return $"{LineNumber}:{Message}";
    }
}