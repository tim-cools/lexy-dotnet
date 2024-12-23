using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Lexy.Poc.Core.Language;
using Microsoft.Extensions.Logging;

namespace Lexy.Poc.Core.Parser
{
    public class ParserLogger : IParserLogger
    {
        private class LogEntry
        {
            public IComponent Component { get; }
            public bool IsError { get; }
            public string Message { get; }

            public LogEntry(IComponent component, bool isError, string message)
            {
                Component = component;
                IsError = isError;
                Message = message;
            }

            public override string ToString() => Message;
        }

        private readonly ILogger<ParserContext> logger;
        private readonly ISourceCodeDocument sourceCodeDocument;
        private readonly IList<LogEntry> logEntries = new List<LogEntry>();
        private int failedMessages = 0;
        private IRootComponent currentComponent;

        public bool HasErrors() => failedMessages > 0;
        public bool HasRootErrors() => logEntries.Any(entry => entry.IsError && entry.Component == null);

        public ParserLogger(ILogger<ParserContext> logger, ISourceCodeDocument sourceCodeDocument)
        {
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
            this.sourceCodeDocument = sourceCodeDocument ?? throw new ArgumentNullException(nameof(sourceCodeDocument));
        }

        public void LogInfo(string message)
        {
            logger.LogInformation(message);
        }

        public void Log(string message)
        {
            var item = $"{sourceCodeDocument.CurrentLine?.Index + 1}: {message}";

            logger.LogDebug(item);
            logEntries.Add(new LogEntry(currentComponent, false, item));
        }

        public void Fail(string message)
        {
            failedMessages++;
            var item = $"{sourceCodeDocument.CurrentLine?.Index + 1}: ERROR - {message}";

            logger.LogError(item);
            logEntries.Add(new LogEntry(currentComponent, true, item));
        }

        public bool HasErrorMessage(string expectedError)
        {
            return logEntries.Any(message => message.IsError && message.Message.Contains(expectedError));
        }

        public string FormatMessages()
        {
            return
                $"{string.Join(Environment.NewLine, logEntries)}{Environment.NewLine}";
        }

        public void SetCurrentComponent(IRootComponent component)
        {
            currentComponent = component ?? throw new ArgumentNullException(nameof(component));
        }

        public bool ComponentHasErrors(IRootComponent component)
        {
            if (component == null) throw new ArgumentNullException(nameof(component));

            return logEntries.Any(message => message.IsError && message.Component == component);
        }

        public string[] ComponentFailedMessages(IRootComponent component)
        {
            return logEntries.Where(entry => entry.IsError && entry.Component == component)
                .Select(entry => entry.Message)
                .ToArray();
        }

        public string[] FailedRootMessages()
        {
            return logEntries.Where(entry => entry.IsError && entry.Component == null)
                .Select(entry => entry.Message)
                .ToArray();
        }

        public string[] FailedMessages()
        {
            return logEntries.Where(entry => entry.IsError)
                .Select(entry => entry.Message)
                .ToArray();
        }

        public void AssertNoErrors()
        {
            if (HasErrors())
            {
                throw new InvalidOperationException($"Parsing failed: {FormatMessages()}");
            }
        }
    }
}