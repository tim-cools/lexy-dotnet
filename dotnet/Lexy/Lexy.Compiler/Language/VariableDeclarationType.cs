
using Lexy.Compiler.Parser;

namespace Lexy.Compiler.Language
{
    public abstract class VariableDeclarationType
    {
        public static VariableDeclarationType Parse(string type)
        {
            if (TypeNames.Contains(type))
            {
                return new PrimitiveVariableDeclarationType(type);
            }

            return new CustomVariableDeclarationType(type);
        }

        public abstract VariableType CreateVariableType(IValidationContext context);
    }
}