using System;
using System.Collections.Generic;
using Lexy.Compiler.Language;

namespace Lexy.Compiler.DependencyGraph;

public class DependencyNode
{
    private readonly List<DependencyNode> dependencies = new();
    private readonly DependencyNode parentNode;

    public string Name { get; }
    public Type Type { get; }

    public IRootNode Node { get; }

    public IReadOnlyList<DependencyNode> Dependencies => dependencies;

    public DependencyNode(string name, Type type, IRootNode node, DependencyNode parentNode)
    {
        Name = name;
        Type = type ?? throw new ArgumentNullException(nameof(type));
        Node = node ?? throw new ArgumentNullException(nameof(node));
        this.parentNode = parentNode;
    }

    public void AddDependency(DependencyNode dependency)
    {
        dependencies.Add(dependency);
    }

    protected bool Equals(DependencyNode other)
    {
        return Name == other.Name && Equals(Type, other.Type);
    }

    public override bool Equals(object obj)
    {
        if (ReferenceEquals(null, obj)) return false;
        if (ReferenceEquals(this, obj)) return true;
        if (obj.GetType() != GetType()) return false;
        return Equals((DependencyNode)obj);
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(Name, Type);
    }

    public bool ExistsInLineage(string name, Type type)
    {
        if (Name == name && Type == type) return true;
        return parentNode != null && parentNode.ExistsInLineage(name, type);
    }
}