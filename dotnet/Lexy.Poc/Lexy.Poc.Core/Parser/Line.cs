using System;
using System.Text;

namespace Lexy.Poc.Core.Parser
{
    public class Line
    {
        private readonly string[] code;

        public int Index { get; }

        internal string Content { get; }
        private string TrimmedContent { get; }

        internal Token[] Tokens { get; private set; }

        public Line(int index, string line, string[] code)
        {
            this.code = code;
            Index = index;
            Content = line ?? throw new ArgumentNullException(nameof(line));
            TrimmedContent = line.Trim();
        }

        public int Indent()
        {
            var spaces = 0;
            var tabs = 0;

            foreach (var value in Content)
            {
                if (value == ' ')
                {
                    spaces++;
                }
                else if (value == '\t')
                {
                    tabs++;
                }
                else
                {
                    break;
                }
            }

            if (spaces > 0 && tabs > 0)
            {
                throw new InvalidOperationException(
                    "Don't mix spaces and tabs for indentations. Use 2 spaces or tabs.");
            }

            if (spaces % 2 != 0)
            {
                throw new InvalidOperationException(
                    "Wrong number of indent spaces " + spaces + ". Should be multiplication of 2.");
            }

            return tabs > 0 ? tabs : spaces / 2;
        }

        public override string ToString()
        {
            return $"Line {Index + 1}: {Content}";
        }

        public bool IsComment()
        {
            return Tokens.Length == 1 && Tokens[0] is CommentToken;
        }

        public bool IsEmpty() => Content.Length == 0;

        internal bool Tokenize(ITokenizer tokenizer, ParserContext parserContext)
        {
            Tokens = tokenizer.Tokenize(this, parserContext, out bool errors);
            return !errors;
        }

        public string TokenValue(int index)
        {
            return index >= 0 && index <= Tokens.Length - 1 ? Tokens[index].Value : null;
        }

        public Token[] TokensFrom(int index)
        {
            return Tokens[index..];
        }

        public string TokenValuesFrom(int startIndex)
        {
            var valueBuilder = new StringBuilder();
            for (int index = startIndex; index < Tokens.Length; index++)
            {
                valueBuilder.Append(Tokens[index].Value);
            }
            return valueBuilder.ToString();
        }

        public bool IsTokenType<T>(int index)
        {
            return index >= 0 && index <= Tokens.Length - 1 && Tokens[index].GetType() == typeof(T);
        }

        public Type TokenAsType<T>(int index)
        {
            return index >= 0 && index <= Tokens.Length - 1 ? Tokens[index].GetType() : null;
        }

        public T Token<T>(int index) where T : Token
        {
            return (T) Tokens[index];
        }

        public ILiteralToken LiteralToken(int index)
        {
            return index >= 0 && index <= Tokens.Length - 1 ? Tokens[index] as ILiteralToken : null;
        }
    }
}