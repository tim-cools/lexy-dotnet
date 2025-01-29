using System;
using System.Collections.Generic;
using Lexy.Compiler.Language;

namespace Lexy.Compiler.DependencyGraph;

public static class DependencyGraphFactory
{
    public static Dependencies Create(RootNodeList rootNodes)
    {
        if (rootNodes == null) throw new ArgumentNullException(nameof(rootNodes));

        var dependencies = new Dependencies(rootNodes);
        dependencies.Build();
        return dependencies;
    }

    public static IEnumerable<IRootNode> NodeAndDependencies(IRootNodeList rootNodes, IRootNode node)
    {
        if (rootNodes == null) throw new ArgumentNullException(nameof(rootNodes));
        if (node == null) throw new ArgumentNullException(nameof(node));

        var dependencies = new Dependencies(rootNodes);
        dependencies.Build();
        return dependencies.NodeAndDependencies(node);
    }
}