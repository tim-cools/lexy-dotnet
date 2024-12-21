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
            public string ComponentName { get; }
            public bool IsError { get; }
            public string Message { get; }

            public LogEntry(string componentName, bool isError, string message)
            {
                ComponentName = componentName;
                IsError = isError;
                Message = message;
            }

            public override string ToString() => Message;
        }

        private readonly ILogger<ParserContext> logger;
        private readonly ISourceCodeDocument sourceCodeDocument;
        private readonly IList<LogEntry> logEntries = new List<LogEntry>();
        private int failedMessages = 0;

        public bool HasErrors() => failedMessages > 0;

        public ParserLogger(ILogger<ParserContext> logger, ISourceCodeDocument sourceCodeDocument)
        {
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
            this.sourceCodeDocument = sourceCodeDocument ?? throw new ArgumentNullException(nameof(sourceCodeDocument));
        }

        public void Log(string message, string componentName)
        {
            var item = $"{sourceCodeDocument.CurrentLine?.Index + 1}: {message}";

            logger.LogDebug(item);
            logEntries.Add(new LogEntry(componentName, false, item));
        }

        public void Fail(string message, string componentName)
        {
            failedMessages++;
            var item = $"{sourceCodeDocument.CurrentLine?.Index + 1}: ERROR - {message}";

            logger.LogError(item);
            logEntries.Add(new LogEntry(componentName, true, item));
        }

        public bool HasErrorMessage(string expectedError)
        {
            return logEntries.Any(message => message.IsError && message.Message.StartsWith(expectedError));
        }

        public string FormatMessages()
        {
            return
                $"{string.Join(Environment.NewLine, logEntries)}{Environment.NewLine}------------- Lexy Source Code{Environment.NewLine}{FormatCode()}";
        }

        public bool ComponentHasErrors(IRootComponent component)
        {
            if (component == null) throw new ArgumentNullException(nameof(component));

            return logEntries.Any(message => message.IsError && message.ComponentName == component.ComponentName);
        }

        public string[] ComponentFailedMessages(IRootComponent component)
        {
            return logEntries.Where(entry => entry.IsError && entry.ComponentName == component.ComponentName)
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

        private string FormatCode()
        {
            var builder = new StringBuilder();
            foreach (var line in sourceCodeDocument.Code)
            {
                builder.AppendLine(line.ToString());
            }
            return builder.ToString();
        }
    }
}