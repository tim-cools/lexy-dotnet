using System;
using System.Collections.Generic;
using System.Linq;
using Lexy.Poc.Core.Parser;

namespace Lexy.Poc.Core.Language
{
    public class EnumDefinition : RootNode
    {
        public EnumName Name { get; } = new EnumName();

        public override string NodeName => Name.Value;

        public IList<EnumMember> Members { get; } = new List<EnumMember>();

        private EnumDefinition(string name)
        {
            Name.ParseName(name);
        }

        internal static EnumDefinition Parse(NodeName name)
        {
            return new EnumDefinition(name.Name);
        }

        public override IParsableNode Parse(IParserContext context)
        {
            var line = context.CurrentLine;
            if (line.IsEmpty()) return this;

            if (line.IsComment())
            {
                throw new InvalidOperationException("No comments expected. Comment should be parsed by Document only.");
            }

            var member = EnumMember.Parse(context);
            if (member != null)
            {
                Members.Add(member);
            }
            return this;
        }

        protected override IEnumerable<INode> GetChildren()
        {
            yield return Name;

            foreach (var member in Members)
            {
                yield return member;
            }
        }

        protected override void Validate(IParserContext context)
        {
            DuplicateChecker.Validate(
                context,
                member => member.Name,
                member => $"Enum member name should be unique. Duplicate name: '{member.Name}'",
                Members);
        }

        public bool ContainsMember(string name)
        {
            return Members.Any(member => member.Name == name);
        }
    }
}