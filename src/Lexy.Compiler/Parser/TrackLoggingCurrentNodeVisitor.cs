using System;
using System.Collections.Generic;
using Lexy.Compiler.Language;

namespace Lexy.Compiler.Parser;

public class TrackLoggingCurrentNodeVisitor : ITreeValidationVisitor
{
    private readonly Stack<IRootNode> nodeStack = new();
    private readonly IParserLogger logger;

    public TrackLoggingCurrentNodeVisitor(IParserLogger logger)
    {
        this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public void Enter(Node node)
    {
        if (node is not IRootNode rootNode) return;

        AddCurrentNodeToStack(rootNode);

        logger.SetCurrentNode(rootNode);
    }

    public void Leave(Node node)
    {
        if (node is not IRootNode) return;

        RemoveCurrentNodeFromStack();

        RevertToPreviousNode();
    }

    private void AddCurrentNodeToStack(IRootNode rootNode) => nodeStack.Push(rootNode);
    private void RemoveCurrentNodeFromStack() => nodeStack.Pop();

    private void RevertToPreviousNode()
    {
        if (nodeStack.TryPeek(out var previousNode))
        {
            logger.SetCurrentNode(previousNode);
        }
    }
}