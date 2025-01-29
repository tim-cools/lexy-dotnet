using System;
using System.Collections.Generic;
using System.Linq;
using Lexy.Compiler.Language;

namespace Lexy.Compiler.DependencyGraph;

public class Dependencies
{
    private readonly List<IRootNode> circularReferences = new();
    private readonly List<DependencyNode> allNodes = new();
    private readonly IRootNodeList rootNodes;

    public IList<DependencyNode> Nodes { get; } = new List<DependencyNode>();
    public bool HasCircularReferences => circularReferences.Count > 0;
    public IReadOnlyList<IRootNode> CircularReferences => circularReferences;

    public Dependencies(IRootNodeList rootNodes)
    {
        this.rootNodes = rootNodes ?? throw new ArgumentNullException(nameof(rootNodes));
    }

    public void Build()
    {
        ProcessNodes(rootNodes, null);
    }

    public IEnumerable<IRootNode> NodeAndDependencies(IRootNode node)
    {
        var dependencyNode = allNodes.FirstOrDefault(each => each.Name == node.NodeName);
        if (dependencyNode == null) return new []{node};
        return new []{node}.Union(Flatten(dependencyNode.Dependencies));
    }

    private void ProcessNodes(IEnumerable<IRootNode> nodes, DependencyNode parentNode)
    {
        foreach (var node in nodes)
        {
            Nodes.Add(ProcessNode(node, parentNode));
        }
    }

    private DependencyNode ProcessNode(INode node, DependencyNode parentNode)
    {
        var dependencyNode = NewDependencyNode(node, parentNode);
        var dependencies = GetDependencies(node, dependencyNode);
        foreach (var dependency in dependencies)
        {
            dependencyNode.AddDependency(dependency);
        }
        return dependencyNode;
    }

    private DependencyNode NewDependencyNode(INode node, DependencyNode parentNode)
    {
        if (node is not IRootNode rootNode)
        {
            throw new InvalidOperationException("Node dependencies should be root nodes.");
        }

        var dependencyNode = new DependencyNode(rootNode.NodeName, node.GetType(), rootNode, parentNode);
        allNodes.Add(dependencyNode);
        return dependencyNode;
    }

    private IReadOnlyList<DependencyNode> GetDependencies(INode node, DependencyNode parentNode)
    {
        var resultDependencies = new List<DependencyNode>();
        NodesWalker.Walk(node, childNode => ProcessDependencies(parentNode, childNode, resultDependencies));
        return resultDependencies;
    }

    private void ProcessDependencies(DependencyNode parentNode, INode childNode,
        List<DependencyNode> resultDependencies)
    {
        var nodeDependencies = (childNode as IHasNodeDependencies)?.GetDependencies(rootNodes);
        if (nodeDependencies == null) return;

        foreach (var dependency in nodeDependencies)
        {
            ValidateDependency(parentNode, resultDependencies, dependency);
        }
    }

    private void ValidateDependency(DependencyNode parentNode, List<DependencyNode> resultDependencies,
        IRootNode dependency)
    {
        if (dependency == null) throw new InvalidOperationException("node.GetNodes() should never return null");

        if (parentNode != null && parentNode.ExistsInLineage(dependency.NodeName, dependency.GetType()))
        {
            if (!circularReferences.Any(reference => reference.NodeName == dependency.NodeName))
            {
                circularReferences.Add(dependency);
            }
        }
        else
        {
            if (DependencyExists(resultDependencies, dependency)) return;

            var dependencyNode = ProcessNode(dependency, parentNode);
            resultDependencies.Add(dependencyNode);
        }
    }

    private static bool DependencyExists(List<DependencyNode> resultDependencies, IRootNode dependency)
    {
        return resultDependencies.Any(any => any.Name == dependency.NodeName && any.Type == dependency.GetType());
    }

    private static IEnumerable<IRootNode> Flatten(IReadOnlyList<DependencyNode> dependencies)
    {
        var result = new List<IRootNode>();
        Flatten(result, dependencies);
        return result;
    }

    private static void Flatten(List<IRootNode> result, IReadOnlyList<DependencyNode> dependencies)
    {
        foreach (var dependency in dependencies)
        {
            result.Add(dependency.Node);
            Flatten(result, dependency.Dependencies);
        }
    }
}