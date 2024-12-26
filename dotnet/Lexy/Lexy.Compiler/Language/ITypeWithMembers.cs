namespace Lexy.Compiler.Language
{
    public interface ITypeWithMembers
    {
        VariableType MemberType(string name);
    }
}