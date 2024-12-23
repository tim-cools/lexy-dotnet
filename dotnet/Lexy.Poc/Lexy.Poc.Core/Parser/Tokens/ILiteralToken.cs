namespace Lexy.Poc.Core.Parser.Tokens
{
    public interface ILiteralToken : IToken
    {
        string Value { get; }
    }
}