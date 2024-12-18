using System.Collections.Generic;
using Lexy.Poc.Core.Parser;

namespace Lexy.Poc.Core.Language
{
    public class ScenarioTable : IComponent
    {
        public TableHeaders Headers { get; private set; }
        public IList<TableRow> Rows { get; } = new List<TableRow>();

        public IComponent Parse(ParserContext context)
        {
            var line = context.CurrentLine;

            if (line.IsEmpty()) return this;

            if (Headers == null)
            {
                Headers = TableHeaders.Parse(context);
            }
            else
            {
                Rows.Add(TableRow.Parse(context));
            }

            return this;
        }
    }
}