using Lexy.Poc.Core.Language;

namespace Lexy.Poc.Core.Parser
{
    public interface IParserLogger
    {
        void LogInfo(string message);

        void Log(SourceReference reference, string message);
        void Fail(SourceReference reference, string message);
        void Fail(INode node, SourceReference reference, string message);

        bool HasErrors();
        bool HasRootErrors();

        bool HasErrorMessage(string expectedError);

        public string FormatMessages();

        bool NodeHasErrors(IRootNode node);

        string[] NodeFailedMessages(IRootNode node);
        string[] FailedRootMessages();
        string[] FailedMessages();

        void AssertNoErrors();

        void SetCurrentNode(IRootNode node);
        void Reset();
    }
}