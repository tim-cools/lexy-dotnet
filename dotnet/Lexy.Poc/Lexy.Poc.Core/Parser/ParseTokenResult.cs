namespace Lexy.Poc.Core.Parser
{
    public class ParseTokenResult
    {
        public Token NewToken { get; }
        public TokenStatus Status { get; }

        public ParseTokenResult(TokenStatus status, Token newToken = null)
        {
            NewToken = newToken;
            Status = status;
        }
    }
}