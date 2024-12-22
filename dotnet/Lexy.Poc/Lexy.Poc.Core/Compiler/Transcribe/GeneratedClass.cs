using Lexy.Poc.Core.Language;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Lexy.Poc.Core.Transcribe
{
    public class GeneratedClass
    {
        public IRootComponent Component { get; }
        public string ClassName { get; }
        public string FullClassName => $"{WriterCode.Namespace}.{ClassName}";
        public MemberDeclarationSyntax Syntax { get; }

        public GeneratedClass(IRootComponent component, string className, MemberDeclarationSyntax syntax)
        {
            Component = component;
            ClassName = className;
            Syntax = syntax;
        }
    }
}