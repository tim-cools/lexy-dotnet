using Microsoft.Extensions.Logging;

namespace Lexy.Compiler.Compiler
{
    public class CompilerContext : ICompilerContext
    {
        public ILogger<CompilerContext> Logger { get; }

        public CompilerContext(ILogger<CompilerContext> logger)
        {
            this.Logger = logger;
        }
    }
}