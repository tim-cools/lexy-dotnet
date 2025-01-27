using Lexy.Compiler.Language.VariableTypes;

namespace Lexy.Compiler.Language;

public class VariableReference
{
    public VariablePath Path { get; }
    public VariableSource Source { get; }
    public VariableType RootType { get; }
    public VariableType VariableType { get; }

    public VariableReference(VariablePath path, VariableType rootType,
        VariableType variableType, VariableSource source)
    {
        Path = path;
        RootType = rootType;
        VariableType = variableType;
        Source = source;
    }

    public override string ToString()
    {
        return Path.ToString();
    }
}