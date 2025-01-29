using System.Collections.Generic;

namespace Lexy.Compiler.Language.Types;

public interface ITypeDefinition : IRootNode
{
    IReadOnlyList<VariableDefinition> Variables { get; }
}