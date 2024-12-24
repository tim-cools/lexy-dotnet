namespace Lexy.Poc.Core.Parser
{
    public interface IFunctionCodeContext
    {
        void RegisterVariableAndVerifyUnique(SourceReference reference, string variableName, VariableType variableType);
        void EnsureVariableExists(SourceReference reference, string variableName);
        bool Contains(string name);

        VariableType GetVariableType(string variableName);
    }
}