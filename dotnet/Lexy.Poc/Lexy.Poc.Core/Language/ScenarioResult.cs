using System.Collections.Generic;
using Lexy.Poc.Core.Parser;

namespace Lexy.Poc.Core.Language
{
    public class ScenarioResults : IComponent
    {
        public IList<AssignmentDefinition> Assignments { get; } = new List<AssignmentDefinition>();

        public IComponent Parse(ParserContext parserContext)
        {
            var line = parserContext.CurrentLine;
            if (line.IsEmpty()) return this;

            var assignment = AssignmentDefinition.Parse(line);
            Assignments.Add(assignment);
            return this;
        }
    }
}