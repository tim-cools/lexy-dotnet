using Lexy.Compiler.Language;

namespace Lexy.Compiler.Parser;

public interface ITreeValidationVisitor
{
    void Enter(Node node);
    void Leave(Node node);
}