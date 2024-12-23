using System;
using System.Collections.Generic;
using Lexy.Poc.Core.Parser;

namespace Lexy.Poc.Core.Language
{
    public class TableHeader : Node
    {
        public string Name { get; }
        public string Type { get; }

        public TableHeader(string name, string type)
        {
            Name = name;
            Type = type;
        }

        public static TableHeader Parse(string name, string typeName)
        {
            return new TableHeader(name, typeName);
        }

        protected override IEnumerable<INode> GetChildren()
        {
            yield break;
        }

        protected override void Validate(IParserContext context)
        {
        }
    }
}