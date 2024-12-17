using System.Collections.Generic;
using Lexy.Poc.Core.Parser;

namespace Lexy.Poc.Core.Language
{
    public class EnumDefinition : RootComponent
    {
        public Comments Comments { get; } = new Comments();
        public EnumName Name { get; } = new EnumName();

        public IList<AssignmentDefinition> Assignments { get; } = new List<AssignmentDefinition>();
        public override string TokenName => Name.Value;

        private EnumDefinition(string name)
        {
            Name.ParseName(name);
        }

        internal static EnumDefinition Parse(ComponentName name)
        {
            return new EnumDefinition(name.Parameter);
        }

        public override IComponent Parse(ParserContext parserContext)
        {
            var line = parserContext.CurrentLine;
            if (line.IsEmpty()) return this;

            if (line.IsComment())
            {
                Comments.Parse(parserContext);
            }
            else
            {
                var assignment = AssignmentDefinition.Parse(line);
                Assignments.Add(assignment);
            }
            return this;
        }
    }
}