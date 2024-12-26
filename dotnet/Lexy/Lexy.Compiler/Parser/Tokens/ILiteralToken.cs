
using Lexy.Compiler.Language;

namespace Lexy.Compiler.Parser.Tokens
{
    public interface ILiteralToken : IToken
    {
        string Value { get; }

        VariableType DeriveType(IValidationContext context);
    }
}