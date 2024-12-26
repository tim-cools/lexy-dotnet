using System.Collections.Generic;
using Lexy.Compiler.Parser;

namespace Lexy.Compiler.Language.Expressions.Functions
{
    public abstract class NoArgumentFunction : BuiltInFunction
    {
        protected abstract VariableType ResultType { get; }

        protected NoArgumentFunction(SourceReference reference)
            : base(reference)
        {
        }

        public override IEnumerable<INode> GetChildren()
        {
            yield break;
        }

        protected override void Validate(IValidationContext context)
        {
        }

        public override VariableType DeriveReturnType(IValidationContext context) => ResultType;
    }
}