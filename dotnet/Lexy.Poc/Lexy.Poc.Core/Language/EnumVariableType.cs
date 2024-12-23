namespace Lexy.Poc.Core.Language
{
    public class CustomVariableType : VariableType
    {
        public string TypeName { get; }

        public CustomVariableType(string typeName)
        {
            TypeName = typeName;
        }

        public override string ToString()
        {
            return TypeName;
        }
    }
}