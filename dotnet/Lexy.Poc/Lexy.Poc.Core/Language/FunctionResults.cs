using System;
using System.Collections.Generic;
using System.Linq;
using Lexy.Poc.Core.Parser;

namespace Lexy.Poc.Core.Language
{
    public class FunctionResults : IComponent
    {
        public IList<VariableDefinition> Variables { get; } = new List<VariableDefinition>();

        public IComponent Parse(ParserContext parserContext)
        {
            var line = parserContext.CurrentLine;

            var variableDefinition = VariableDefinition.Parse(line);
            if (variableDefinition.Default != null)
            {
                parserContext.Fail(
                    $"Result variable {variableDefinition.Name} should not have a default value. (Line:{line})");
                return null;
            }
            Variables.Add(variableDefinition);

            return this;
        }

        public string GetParameterType(string expectedName)
        {
            return Variables.FirstOrDefault(variable => variable.Name == expectedName)?.Type;
        }
    }
}