using System;

namespace Lexy.RunTime;

public class LogVariable
{
    public LogVariableType Type { get; }
    public object Value { get; }

    public LogVariables LogVariables => Type == LogVariableType.LogVariables ? Value as LogVariables : null;

    public LogVariable(object value, LogVariableType type)
    {
        Value = value;
        Type = type;
    }

    public override string ToString()
    {
        return Value?.ToString();
    }
}