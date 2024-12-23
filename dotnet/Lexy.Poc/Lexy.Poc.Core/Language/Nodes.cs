using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace Lexy.Poc.Core.Language
{
    public class Nodes
    {
        private readonly IList<IRootNode> values = new Collection<IRootNode>();

        public int Count => values.Count;

        internal bool ContainsEnum(string enumName)
        {
            return values
                .OfType<EnumDefinition>()
                .Any(definition => definition.Name.Value == enumName);
        }

        public IRootNode GetNode(string name)
        {
            return values
                .SingleOrDefault(definition => definition.NodeName == name);
        }

        public bool Contains(string name)
        {
            return values
                .Any(definition => definition.NodeName == name);
        }

        public Function GetFunction(string name)
        {
            return values
                .OfType<Function>()
                .FirstOrDefault(function => function.Name.Value == name);
        }

        public IRootNode GetTable(string name)
        {
            return values
                .OfType<Table>()
                .FirstOrDefault(function => function.Name.Value == name);
        }

        public Function GetSingleFunction()
        {
            return values
                .OfType<Function>()
                .SingleOrDefault();
        }

        public IEnumerable<Scenario> GetScenarios() => values.OfType<Scenario>();

        public EnumDefinition GetEnum(string name)
        {
            return values
                .OfType<EnumDefinition>()
                .FirstOrDefault(enumDefinition => enumDefinition.Name.Value == name);
        }

        public void AddIfNew(IRootNode node)
        {
            if (!values.Contains(node))
            {
                values.Add(node);
            }
        }

        public INode First() => values.FirstOrDefault();

        public string MapType(string variableType)
        {
            if (ContainsEnum(variableType))
            {
                return variableType;
            }

            return variableType switch
            {
                TypeNames.String => "string",
                TypeNames.Number => "decimal",
                TypeNames.Boolean => "bool",
                TypeNames.DateTime => "System.DateTime",
                _ => throw new InvalidOperationException($"Unknown type: '{variableType}'")
            };
        }
    }
}