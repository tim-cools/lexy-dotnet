using System;
using System.Collections.Generic;
using Lexy.Poc.Core.Parser;

namespace Lexy.Poc.Core.Language
{
    public interface INode
    {
        void ValidateTree(IParserContext context);
    }

    public interface IParsableNode : INode
    {
        IParsableNode Parse(IParserContext context);
    }

    public abstract class ParsableNode : Node, IParsableNode
    {
        public abstract IParsableNode Parse(IParserContext context);
    }

    public abstract class Node : INode
    {
        public virtual void ValidateTree(IParserContext context)
        {
            Validate(context);

            var children = GetChildren();
            foreach (var child in children)
            {
                if (child == null)
                {
                    throw new InvalidOperationException($"({GetType().Name}) Child is null");
                }
                child.ValidateTree(context);
            }
        }

        protected abstract IEnumerable<INode> GetChildren();

        protected abstract void Validate(IParserContext context);
    }
}