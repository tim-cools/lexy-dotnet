using Lexy.Compiler.Parser;

namespace Lexy.Compiler.Language.VariableTypes;

public abstract class VariableDeclarationType : Node
{
    public VariableType VariableType { get; protected set; }

    protected VariableDeclarationType(SourceReference reference) : base(reference)
    {
    }


    protected abstract VariableType ValidateVariableType(IValidationContext context);

    protected override void Validate(IValidationContext context)
    {
        VariableType = ValidateVariableType(context);
    }
}