using System;
using System.Linq;
using Lexy.Poc.Core.Language;

namespace Lexy.Poc.Core.Parser
{
    public class ParserContext : IParserContext
    {
        private readonly IParserLogger logger;
        private readonly ITokenizer tokenizer;
        private readonly ISourceCodeDocument sourceCodeDocument;

        public Line CurrentLine => sourceCodeDocument.CurrentLine;

        public Nodes Nodes { get; } = new Nodes();
        public IParserLogger Logger => logger;

        public ParserContext(ITokenizer tokenizer, IParserLogger logger, ISourceCodeDocument sourceCodeDocument)
        {
            this.tokenizer = tokenizer ?? throw new ArgumentNullException(nameof(tokenizer));
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
            this.sourceCodeDocument = sourceCodeDocument ?? throw new ArgumentNullException(nameof(sourceCodeDocument));
        }

        public void ProcessNode(IRootNode node)
        {
            if (node == null) throw new ArgumentNullException(nameof(node));

            Nodes.AddIfNew(node);

            logger.SetCurrentNode(node);
        }

        public bool ProcessLine()
        {
            var line = sourceCodeDocument.NextLine();
            logger.Log($"'{line.Content}'");

            var success = CurrentLine.Tokenize(tokenizer, this);
            var tokenNames = string.Join(" ", CurrentLine.Tokens.Select(token => token.GetType().Name).ToArray());

            logger.Log("  Tokens: " + tokenNames);

            return success;
        }

        public TokenValidator ValidateTokens<T>()
        {
            logger.Log("  Parse: " + typeof(T).Name);
            return new TokenValidator(typeof(T).Name, this);
        }

        public TokenValidator ValidateTokens(string name)
        {
            logger.Log("  Parse: " + name);
            return new TokenValidator(name, this);
        }
    }
}