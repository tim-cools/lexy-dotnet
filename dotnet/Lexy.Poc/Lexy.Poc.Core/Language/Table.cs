using System;
using System.Collections.Generic;
using Lexy.Poc.Core.Parser;

namespace Lexy.Poc.Core.Language
{
    public class Table : RootNode
    {
        public TableName Name { get; } = new TableName();
        public TableHeaders Headers { get; private set; }
        public IList<TableRow> Rows { get; } = new List<TableRow>();
        public override string NodeName => Name.Value;

        private Table(string name)
        {
            Name.ParseName(name);
        }

        internal static Table Parse(NodeName name)
        {
            return new Table(name.Name);
        }

        public override IParsableNode Parse(IParserContext context)
        {
            var line = context.CurrentLine;
            if (line.IsEmpty()) return this;

            if (line.IsComment())
            {
                throw new InvalidOperationException("No comments expected. Comment should be parsed by Document only.");
            }

            if (IsFirstLine())
            {
                Headers = TableHeaders.Parse(context);
            }
            else
            {
                Rows.Add(TableRow.Parse(context));
            }

            return this;
        }

        private bool IsFirstLine()
        {
            return Headers == null;
        }

        protected override IEnumerable<INode> GetChildren()
        {
            yield return Headers;

            foreach (var row in Rows)
            {
                yield return row;
            }
        }

        protected override void Validate(IValidationContext context)
        {
        }
    }
}