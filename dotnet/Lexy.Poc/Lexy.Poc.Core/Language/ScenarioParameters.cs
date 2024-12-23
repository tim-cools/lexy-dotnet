using System.Collections.Generic;
using Lexy.Poc.Core.Parser;

namespace Lexy.Poc.Core.Language
{
    public class ScenarioParameters : ParsableNode
    {
        public IList<AssignmentDefinition> Assignments { get; } = new List<AssignmentDefinition>();

        public override IParsableNode Parse(IParserContext context)
        {
            var assignment = AssignmentDefinition.Parse(context);
            Assignments.Add(assignment);
            return this;
        }

        protected override IEnumerable<INode> GetChildren() => Assignments;

        protected override void Validate(IParserContext context)
        {
        }
    }
}