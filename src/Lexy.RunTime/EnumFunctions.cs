namespace Lexy.RunTime;

public static class EnumFunctions
{
    private const string Prefix = "Enum__";

    public static string Format(object value)
    {
        return $"{value.GetType().Name[Prefix.Length..]}.{value}";
    }
}