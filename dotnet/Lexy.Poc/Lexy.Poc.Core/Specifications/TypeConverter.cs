using System;
using Lexy.Poc.Core.Compiler;
using Lexy.Poc.Core.Language;

namespace Lexy.Poc.Core.Specifications
{
    internal static class TypeConverter
    {
        public static object Convert(CompilerResult compilerResult, string value, VariableType type)
        {
            if (type is CustomVariableType enumVariableType)
            {
                if (!compilerResult.ContainsEnum(enumVariableType.TypeName)) throw new InvalidOperationException("Known enum: " + enumVariableType.TypeName);

                var indexOfSeparator = value.IndexOf(".");
                var enumValue = value[(indexOfSeparator + 1)..];

                return Enum.Parse(compilerResult.GetEnumType(enumVariableType.TypeName), enumValue);
            }

            if (type is PrimitiveVariableType primitiveVariableType)
            {
                return primitiveVariableType.Type switch
                {
                    TypeNames.Number => decimal.Parse(value),
                    TypeNames.DateTime => DateTime.Parse(value),
                    TypeNames.Boolean => bool.Parse(value),
                    TypeNames.String => value,
                    _ => throw new InvalidOperationException($"Invalid type: {primitiveVariableType.Type}")
                };
            }

            throw new InvalidOperationException($"Invalid type: {type}");
        }
    }
}