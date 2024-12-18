using System;
using System.Collections.Generic;
using Lexy.Poc.Core.Parser;

namespace Lexy.Poc.Core.Language
{
    public class TableHeaders
    {
        public IList<TableHeader> Values { get; } = new List<TableHeader>();

        private TableHeaders(TableHeader[] values)
        {
            Values = values;
        }

        public static TableHeaders Parse(ParserContext context)
        {
            var index = 0;
            var validator = context.ValidateTokens<TableHeaders>();

            if (!validator.Type<TableSeparatorToken>(index).IsValid)
            {
                return null;
            }

            var headers = new List<TableHeader>();
            while (++index < context.CurrentLine.Tokens.Length)
            {
                if (!validator
                        .Type<StringLiteralToken>(index)
                        .Type<StringLiteralToken>(index + 1)
                        .Type<TableSeparatorToken>(index + 2)
                        .IsValid)
                {
                    return null;
                }

                var typeName = context.CurrentLine.TokenValue(index);
                var name = context.CurrentLine.TokenValue(++index);

                var header = TableHeader.Parse(name, typeName);
                headers.Add(header);

                ++index;
            }

            return new TableHeaders(headers.ToArray());
        }
    }
}