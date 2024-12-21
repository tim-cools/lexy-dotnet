using Lexy.Poc.Core.Language;

namespace Lexy.Poc.Core.Transcribe
{
    public class GeneratedClass
    {
        public IRootComponent Component { get; }
        public string ClassName { get; }
        public string FullClassName => $"{WriterCode.Namespace}.{ClassName}";

        public GeneratedClass(IRootComponent component, string className)
        {
            Component = component;
            ClassName = className;
        }
    }
}