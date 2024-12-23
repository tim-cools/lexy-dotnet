using System;
using System.Collections.Generic;
using System.Linq;
using Lexy.Poc.Core.Parser;

namespace Lexy.Poc.Core.Language
{
    public class FunctionIncludes : ParsableNode
    {
        public IList<FunctionInclude> Definitions { get; } = new List<FunctionInclude>();

        public override IParsableNode Parse(IParserContext context)
        {
            var line = context.CurrentLine;
            if (line.IsEmpty()) return this;

            var definition = FunctionInclude.Parse(context);
            Definitions.Add(definition);
            return this;
        }

        protected override IEnumerable<INode> GetChildren() => Definitions;

        protected override void Validate(IParserContext context)
        {
        }
    }
}