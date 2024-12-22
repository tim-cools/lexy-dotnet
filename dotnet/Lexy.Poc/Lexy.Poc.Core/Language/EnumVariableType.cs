namespace Lexy.Poc.Core.Language
{
    public class EnumVariableType : VariableType
    {
        public string EnumName { get; }

        public EnumVariableType(string enumName)
        {
            EnumName = enumName;
        }
    }
}