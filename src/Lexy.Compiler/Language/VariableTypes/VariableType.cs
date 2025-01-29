using System.Collections.Generic;

namespace Lexy.Compiler.Language.VariableTypes;

public abstract class VariableType
{
    public virtual IEnumerable<IRootNode> GetDependencies(IRootNodeList rootNodeList)
    {
        yield break;
    }
}