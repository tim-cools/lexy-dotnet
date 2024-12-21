using Lexy.Poc.Core.Language;

namespace Lexy.Poc.Core.Transcribe
{
    internal interface IRootTokenWriter
    {
        GeneratedClass CreateCode(ClassWriter classWriter, IRootComponent generateNode, Components components);
    }
}