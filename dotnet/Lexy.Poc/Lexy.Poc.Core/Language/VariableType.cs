
namespace Lexy.Poc.Core.Language
{
    public abstract class VariableType
    {
        public static VariableType Parse(string type)
        {
            if (TypeNames.Contains(type))
            {
                return new PrimitiveVariableType(type);
            }

            return new CustomVariableType(type);
        }
    }
}