using Lexy.Poc.Core.Parser;

namespace Lexy.Poc.Core.Language
{
    public abstract class VariableType
    {
        public static VariableType Parse(string type, IParserContext context)
        {
            if (TypeNames.Contains(type))
            {
                return new PrimitiveVariableType(type);
            }

            return new EnumVariableType(type);
        }
    }
}