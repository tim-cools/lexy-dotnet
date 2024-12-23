using Lexy.Poc.Core.Language;

namespace Lexy.Poc.Core.Transcribe
{
    internal interface IRootTokenWriter
    {
        GeneratedClass CreateCode(IRootNode generateNode, Nodes nodes);
    }
}