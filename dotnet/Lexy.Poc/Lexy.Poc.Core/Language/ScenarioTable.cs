using System.Collections.Generic;
using Lexy.Poc.Core.Parser;

namespace Lexy.Poc.Core.Language
{
    public class ScenarioTable : ParsableNode
    {
        public TableHeaders Headers { get; private set; }
        public IList<TableRow> Rows { get; } = new List<TableRow>();

        public ScenarioTable(SourceReference reference) : base(reference)
        {
        }

        public override IParsableNode Parse(IParserContext context)
        {
            var line = context.CurrentLine;

            if (line.IsEmpty()) return this;

            if (Headers == null)
            {
                Headers = TableHeaders.Parse(context);
                return this;
            }

            var row = TableRow.Parse(context);
            if (row != null)
            {
                Rows.Add(row);
            }

            return this;
        }

        protected override IEnumerable<INode> GetChildren()
        {
            if (Headers != null)
            {
                yield return Headers;
            }

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