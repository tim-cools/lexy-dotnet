namespace Lexy.Poc.Core.Language
{
    public class PrimitiveVariableType : VariableType
    {
        public string Type { get; }

        public PrimitiveVariableType(string type)
        {
            Type = type;
        }
    }
}