using Lexy.Compiler.Language;

namespace Lexy.Compiler.Compiler
{
    public interface ILexyCompiler
    {
        CompilerResult Compile(Nodes nodes, Function function);
    }
}