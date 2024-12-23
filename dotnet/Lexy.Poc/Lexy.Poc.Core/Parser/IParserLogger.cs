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

        bool ComponentHasErrors(IRootComponent component);

        string[] ComponentFailedMessages(IRootComponent component);
        string[] FailedRootMessages();
        string[] FailedMessages();

        void AssertNoErrors();
        void SetCurrentComponent(IRootComponent component);
    }
}