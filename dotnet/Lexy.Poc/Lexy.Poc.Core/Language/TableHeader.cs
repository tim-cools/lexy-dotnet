using System;
using System.Collections.Generic;
using Lexy.Poc.Core.Parser;

namespace Lexy.Poc.Core.Language
{
    public class TableHeader : Node
    {
        public string Name { get; }
        public string Type { get; }

        public TableHeader(string name, string type, SourceReference reference) : base(reference)
        {
            Name = name;
            Type = type;
        }

        public static TableHeader Parse(string name, string typeName, SourceReference reference)
        {
            return new TableHeader(name, typeName, reference);
        }

        protected override IEnumerable<INode> GetChildren()
        {
            yield break;
        }

        protected override void Validate(IValidationContext context)
        {
        }
    }
}