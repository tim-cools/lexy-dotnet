namespace Lexy.RunTime;

public interface IExecutionContext
{
    void SetFileName(string fileName);
    void LogVariables(string message, int? lineNumber, object variablesObject);
    ExecutionLogEntry LogLine(string message, int? lineNumber, LogVariables variables);
    void LogChild(string message);
    void OpenScope(string message, int lineNumber);
    void CloseScope();
    void UseLastNodeAsScope();
    void RevertToParentScope();
}