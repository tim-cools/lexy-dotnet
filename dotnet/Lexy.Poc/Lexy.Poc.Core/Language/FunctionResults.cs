using System;
using System.Collections.Generic;
using System.Linq;
using Lexy.Poc.Core.Parser;

namespace Lexy.Poc.Core.Language
{
    public class FunctionResults : IComponent
    {
        public IList<VariableDefinition> Variables { get; } = new List<VariableDefinition>();

        public IComponent Parse(IParserContext context)
        {
            var line = context.CurrentLine;

            var variableDefinition = VariableDefinition.Parse(context);
            if (variableDefinition?.Default != null)
            {
                context.Logger.Fail(
                    $"Result variable {variableDefinition.Name} should not have a default value. (Line:{line})", context.CurrentComponent?.ComponentName);
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