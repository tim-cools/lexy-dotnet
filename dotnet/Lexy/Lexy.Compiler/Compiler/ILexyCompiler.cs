using System.Collections.Generic;
using Lexy.Compiler.Language;

namespace Lexy.Compiler.Compiler;

public interface ILexyCompiler
{
    CompilerResult Compile(IEnumerable<IRootNode> nodes);
}