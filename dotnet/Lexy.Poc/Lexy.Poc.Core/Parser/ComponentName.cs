using System;
using System.Linq;

namespace Lexy.Poc.Core.Parser
{
    internal class ComponentName
    {
        public string Parameter { get; }
        public string Name { get; }

        private ComponentName(string name, string parameter)
        {
            Parameter = parameter;
            Name = name;
        }

        public static ComponentName Parse(Line line, ParserContext context)
        {
            var tokens = line.Tokens;

            if (!context.ValidateTokens(validator => validator
                    .Count(2)
                    .Type<KeywordToken>(0)
                    .Type<LiteralToken>(1)))
            {
                return null;
            }

            return new ComponentName(tokens[0].Value, tokens[1].Value);
        }
    }
}