using System.Collections.Generic;
using Lexy.Poc.Core.Parser;

namespace Lexy.Poc.Core.Language
{
    public class Document : RootNode
    {
        private readonly IList<IRootNode> nodes = new List<IRootNode>();

        public override string NodeName => "Document";

        public Comments Comments { get; } = new Comments();

        public override IParsableNode Parse(IParserContext context)
        {
            var line = context.CurrentLine;

            if (line.IsEmpty()) return this;
            if (line.Tokens.IsComment()) return Comments;

            if (line.Indent() > 0)
            {
                context.Logger.Fail( $"Unexpected line: {line}");
                return this;
            }

            var tokenName = Parser.NodeName.Parse(line, context);
            if (tokenName == null) return this;

            var rootNode = tokenName.Keyword switch
            {
                null => null,
                Keywords.FunctionKeyword => Function.Create(tokenName.Name),
                Keywords.EnumKeyword => EnumDefinition.Parse(tokenName),
                Keywords.ScenarioKeyword => Scenario.Parse(tokenName),
                Keywords.TableKeyword => Table.Parse(tokenName),
                _ => InvalidNode(tokenName, context)
            };

            if (rootNode == null) return this;

            nodes.Add(rootNode);
            context.ProcessNode(rootNode);

            return rootNode;
        }

        private IRootNode InvalidNode(NodeName tokenName, IParserContext context)
        {
            var message = $"Unknown keyword: {tokenName.Keyword}";
            context.Logger.Fail(message);
            return this;
        }

        protected override IEnumerable<INode> GetChildren() => nodes;

        protected override void Validate(IValidationContext context)
        {
        }
    }
}