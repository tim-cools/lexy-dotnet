using System;
using Lexy.RunTime;

namespace Lexy.Compiler.Language.Scenarios;

public static class VariablePathParser
{
    public static VariablePath Parse(string name)
    {
        if (string.IsNullOrEmpty(name)) throw new ArgumentNullException(nameof(name));

        var parts = name.Split(".");
        return new VariablePath(parts);
    }

    public static VariablePathParseResult Parse(string[] parts)
    {
        var variableReference = new VariablePath(parts);
        return VariablePathParseResult.Success(variableReference);
    }
}