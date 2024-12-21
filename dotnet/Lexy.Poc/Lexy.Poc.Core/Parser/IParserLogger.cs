using Lexy.Poc.Core.Language;

namespace Lexy.Poc.Core.Parser
{
    public interface IParserLogger
    {
        void Log(string message, string componentName = null);

        void Fail(string message, string componentName = null);

        bool HasErrors();

        bool HasErrorMessage(string expectedError);

        public string FormatMessages();

        bool ComponentHasErrors(IRootComponent component);
        string[] ComponentFailedMessages(IRootComponent component);

        void AssertNoErrors();
    }
}