using System;
using System.Collections.Generic;
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

            return Parse(code, Path.GetFileName(fileName), throwException);
        }

        public ParserResult Parse(string[] code, string fileName, bool throwException = true)
        {
            if (code == null) throw new ArgumentNullException(nameof(code));

            sourceCodeDocument.SetCode(code, fileName);

            var currentIndent = 0;
            var document = new Document(context.DocumentReference());
            var nodes = new ParsableNodeArray(document);

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
                    context.Logger.Fail(context.LineStartReference(), $"Invalid indent: {indent}");
                    continue;
                }

                var node = nodes.Get(indent);
                node = ParseLine(node);

                currentIndent = indent + 1;

                nodes.Set(currentIndent, node);
            }

            sourceCodeDocument.Reset();
            logger.Reset();

            var validationContext = new ValidationContext(context);
            document.ValidateTree(validationContext);

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

    public class ValidationContext : IValidationContext
    {
        private class CodeContextScope : IDisposable
        {
            private readonly Func<IFunctionCodeContext> func;

            public CodeContextScope(Func<IFunctionCodeContext> func) => this.func = func;

            public void Dispose() => func();
        }

        public IParserContext ParserContext { get; }
        public IFunctionCodeContext FunctionCodeContext { get; private set; }
        public IParserLogger Logger => ParserContext.Logger;
        public Nodes Nodes => ParserContext.Nodes;

        public ValidationContext(IParserContext context)
        {
            ParserContext = context ?? throw new ArgumentNullException(nameof(context));
        }

        public IDisposable CreateCodeContextScope()
        {
            if (FunctionCodeContext != null)
            {
                throw new InvalidOperationException("Already in a code scope. Only can enter scope once.");
            }

            FunctionCodeContext = new FunctionCodeContext(Logger);
            return new CodeContextScope(() => FunctionCodeContext = null);
        }
    }


    public class FunctionCodeContext : IFunctionCodeContext
    {
        private readonly IParserLogger logger;
        private readonly IList<string> variables = new List<string>();

        public FunctionCodeContext(IParserLogger logger)
        {
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public bool EnsureVariableUnique(INode node, string name)
        {
            if (variables.Contains(name))
            {
                logger.Fail(node.Reference, $"Duplicated variable name: '{name}'");
                return false;
            }

            variables.Add(name);
            return true;
        }

        public void EnsureVariableExists(INode node, string variableName)
        {
            if (!variables.Contains(variableName))
            {
                logger.Fail(node.Reference, $"Unknown variable name: '{variableName}'");
            }
        }
    }

    public interface IValidationContext
    {
        IParserContext ParserContext { get; }
        IParserLogger Logger { get; }
        IFunctionCodeContext FunctionCodeContext { get; }
        Nodes Nodes { get; }

        IDisposable CreateCodeContextScope();
    }

    public interface IFunctionCodeContext
    {
        bool EnsureVariableUnique(INode node, string variableName);
        void EnsureVariableExists(INode node, string variableName);
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