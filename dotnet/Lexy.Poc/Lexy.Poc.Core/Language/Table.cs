using System.Collections.Generic;
using Lexy.Poc.Core.Parser;

namespace Lexy.Poc.Core.Language
{
    public class Table : RootComponent
    {
        public TableName Name { get; } = new TableName();
        public Comments Comments { get; } = new Comments();
        public TableHeaders Headers { get; private set; }
        public IList<TableRow> Rows { get; } = new List<TableRow>();
        public override string TokenName => Name.Value;

        private Table(string name)
        {
            Name.ParseName(name);
        }

        internal static Table Parse(ComponentName name)
        {
            return new Table(name.Parameter);
        }

        public override IComponent Parse(ParserContext parserContext)
        {
            var line = parserContext.CurrentLine;
            if (line.IsComment())
            {
                Comments.Parse(parserContext);
            }
            else if (Headers == null)
            {
                Headers = TableHeaders.Parse(line);
            }
            else
            {
                Rows.Add(TableRow.Parse(line));
            }

            return this;
        }
    }
}