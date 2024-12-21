using System.Reflection;
using Lexy.Poc.Core.Parser;
using Lexy.Poc.Core.Transcribe;

namespace Lexy.Poc.Core.Compiler
{
    public interface IExecutionEnvironment
    {
        void CreateExecutables(Assembly assembly);

        void AddType(GeneratedClass generatedType);
        CompilerResult Result();
    }
}