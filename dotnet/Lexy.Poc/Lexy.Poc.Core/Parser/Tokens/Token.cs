using System.Text;

namespace Lexy.Poc.Core.Parser
{
    public abstract class Token
    {
        private StringBuilder valueBuilder;

        public string Value => valueBuilder.ToString();

        protected Token(char value)
        {
            valueBuilder = new StringBuilder(value.ToString());
        }

        protected Token(string value)
        {
            valueBuilder = new StringBuilder(value);
        }

        protected void AppendValue(char value)
        {
            valueBuilder.Append(value);
        }

        public abstract ParseTokenResult Parse(char value, ParserContext context);

        public abstract ParseTokenResult Finalize(ParserContext parserContext);
    }
}