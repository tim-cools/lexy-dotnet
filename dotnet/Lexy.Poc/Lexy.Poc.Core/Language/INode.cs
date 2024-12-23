using Lexy.Poc.Core.Parser;

namespace Lexy.Poc.Core.Language
{
    public interface INode
    {
        void ValidateTree(IValidationContext context);
    }
}