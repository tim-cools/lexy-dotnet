using System;
using System.Linq;
using System.Text;

namespace Lexy.Compiler.Language;

public class VariablePath
{
    public string[] Path { get; }
    public string ParentIdentifier => Path[0];
    public bool HasChildIdentifiers => Path.Length > 1;
    public int Parts => Path.Length;

    public VariablePath(string[] variablePath)
    {
        Path = variablePath ?? throw new ArgumentNullException(nameof(variablePath));
    }

    public string FullPath() => string.Join(".", Path);
    public override string ToString() => FullPath();

    public VariablePath ChildrenReference()
    {
        var parts = Path[1..];
        return new VariablePath(parts);
    }

    public VariablePath Append(string[] path)
    {
        var newPath = Path.Concat(path).ToArray();
        return new VariablePath(newPath);
    }

    public string LastPart()
    {
        return Path[^1];
    }
}