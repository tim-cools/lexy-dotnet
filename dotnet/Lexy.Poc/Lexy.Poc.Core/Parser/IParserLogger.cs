using Lexy.Poc.Core.Language;

namespace Lexy.Poc.Core.Parser
{
    public interface IParserLogger
    {
        void LogInfo(string message);
        void Log(string message);
        void Fail(string message);

        bool HasErrors();
        bool HasRootErrors();

        bool HasErrorMessage(string expectedError);

        public string FormatMessages();

        bool NodeHasErrors(IRootNode node);

        string[] NodeFailedMessages(IRootNode node);
        string[] FailedRootMessages();

        void AssertNoErrors();
        void SetCurrentNode(IRootNode node);
    }
}