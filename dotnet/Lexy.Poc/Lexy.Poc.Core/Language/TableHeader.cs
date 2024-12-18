using System;
using Lexy.Poc.Core.Parser;

namespace Lexy.Poc.Core.Language
{
    public class TableHeader
    {
        public string Name { get; }
        public Types Type { get; }

        public TableHeader(string name, Types type)
        {
            Name = name;
            Type = type;
        }

        public static TableHeader Parse(string name, string typeName)
        {
            var type = TypeNames.ConvertToType(typeName);
            return new TableHeader(name, type);
        }
    }
}