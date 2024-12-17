using System.Collections.Generic;
using Lexy.Poc.Core.Parser;

namespace Lexy.Poc.Core.Language
{
    public abstract class RootComponent : IRootComponent
    {
        private IList<string> failedMessages { get; } = new List<string>();

        public IEnumerable<string> FailedMessages => failedMessages;

        public abstract string TokenName { get; }

        public abstract IComponent Parse(ParserContext parserContext);

        public void Fail(string message)
        {
            failedMessages.Add(message);
        }
    }
}