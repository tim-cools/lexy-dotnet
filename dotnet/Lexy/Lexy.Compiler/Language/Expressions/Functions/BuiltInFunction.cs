using Lexy.Compiler.Parser;

namespace Lexy.Compiler.Language.Expressions.Functions
{
    public abstract class BuiltInFunction : Node
    {
        protected BuiltInFunction(SourceReference reference) : base(reference)
        {
        }

        public abstract VariableType DeriveReturnType(IValidationContext context);
    }
}