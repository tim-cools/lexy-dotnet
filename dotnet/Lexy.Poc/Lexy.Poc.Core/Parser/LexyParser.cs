using System;
using System.IO;
using Lexy.Poc.Core.Language;

namespace Lexy.Poc.Core.Parser
{
    public class LexyParser : ILexyParser
    {
        private readonly IParserContext context;
        private readonly ISourceCodeDocument sourceCodeDocument;
        private readonly IParserLogger logger;

        public LexyParser(IParserContext parserContext, ISourceCodeDocument sourceCodeDocument, IParserLogger logger)
        {
            context = parserContext ?? throw new ArgumentNullException(nameof(parserContext));
            this.sourceCodeDocument = sourceCodeDocument ?? throw new ArgumentNullException(nameof(sourceCodeDocument));
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public ParserResult ParseFile(string fileName, bool throwException = true)
        {
            logger.LogInfo("Parse file: " + fileName);

            var code = File.ReadAllLines(fileName);

            return Parse(code, throwException);
        }

        public ParserResult Parse(string[] code, bool throwException = true)
        {
            if (code == null) throw new ArgumentNullException(nameof(code));

            sourceCodeDocument.SetCode(code);

            var currentIndent = 0;
            var document = new Document();
            var nodes = new ParsableNodeArray(document);

            IParsableNode currentNode = document;

            while (sourceCodeDocument.HasMoreLines())
            {
                if (!context.ProcessLine())
                {
                    currentIndent = sourceCodeDocument.CurrentLine.Indent();
                    continue;
                }

                var line = sourceCodeDocument.CurrentLine;
                if (line.IsComment() || line.IsEmpty())
                {
                    continue;
                }

                var indent = line.Indent();
                if (indent > currentIndent)
                {
                    context.Logger.Fail($"Invalid indent: {indent}");
                    continue;
                }

                currentNode = nodes.Get(indent);

                var node = ParseLine(currentNode);

                currentNode = node;
                currentIndent = indent + 1;
                nodes.Set(currentIndent, currentNode);
            }

            sourceCodeDocument.Reset();

            document.ValidateTree(context);

            if (throwException)
            {
                logger.AssertNoErrors();
            }

            return new ParserResult(context.Nodes);
        }

        private IParsableNode ParseLine(IParsableNode currentNode)
        {
            var node = currentNode.Parse(context);
            if (node == null)
            {
                throw new InvalidOperationException(
                    $"({currentNode}) Parse should return child node or itself.");
            }
            return node;
        }
    }

    public class ParsableNodeArray
    {
        private IParsableNode[] values = new IParsableNode[8];

        public ParsableNodeArray(IParsableNode rootNode)
        {
            values[0] = rootNode;
        }

        public IParsableNode Get(int indent)
        {
            var node = values[indent];
            for (var index = indent + 1; index < values.Length; index++)
            {
                if (values[index] == null) break;

                values[index] = null;
            }

            return node;
        }

        public void Set(int indent, IParsableNode node)
        {
            if (indent >= values.Length)
            {
                Array.Resize(ref values, values.Length * 2);
            }

            values[indent] = node;
        }
    }
}