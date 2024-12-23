using System;
using System.Collections.Generic;
using System.Linq;
using Lexy.Poc.Core.Language;
using Microsoft.Extensions.Logging;

namespace Lexy.Poc.Core.Parser
{
    public class ParserLogger : IParserLogger
    {
        private class LogEntry
        {
            public INode Node { get; }
            public bool IsError { get; }
            public string Message { get; }

            public LogEntry(INode node, bool isError, string message)
            {
                Node = node;
                IsError = isError;
                Message = message;
            }

            public override string ToString() => Message;
        }

        private readonly ILogger<ParserLogger> logger;
        private readonly ISourceCodeDocument sourceCodeDocument;
        private readonly IList<LogEntry> logEntries = new List<LogEntry>();
        private int failedMessages;
        private IRootNode currentNode;

        public bool HasErrors() => failedMessages > 0;
        public bool HasRootErrors() => logEntries.Any(entry => entry.IsError && entry.Node == null);

        public ParserLogger(ILogger<ParserLogger> logger, ISourceCodeDocument sourceCodeDocument)
        {
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
            this.sourceCodeDocument = sourceCodeDocument ?? throw new ArgumentNullException(nameof(sourceCodeDocument));
        }

        public void LogInfo(string message) => logger.LogInformation(message);

        public void Log(string message)
        {
            if (sourceCodeDocument.CurrentLine != null)
            {
                var lineIndex = sourceCodeDocument.CurrentLine?.Index + 1;
                logger.LogDebug("{LineIndex}: {Message}", lineIndex, message);
                logEntries.Add(new LogEntry(currentNode, false, $"{lineIndex}: {message}"));
            }
            else
            {
                logger.LogDebug("{CurrentNode}: {Message}", currentNode?.NodeName, message);
                logEntries.Add(new LogEntry(currentNode, false, $"{currentNode?.NodeName}: {message}"));
            }
        }

        public void Fail(string message)
        {
            failedMessages++;

            if (sourceCodeDocument.CurrentLine != null)
            {
                var lineIndex = sourceCodeDocument.CurrentLine?.Index + 1;
                logger.LogError("{LineIndex}: ERROR - {Message}", lineIndex, message);
                logEntries.Add(new LogEntry(currentNode, true, $"{lineIndex}: ERROR - {message}"));
            }
            else
            {
                logger.LogError("{CurrentNode}: ERROR - {Message}", currentNode?.NodeName, message);
                logEntries.Add(new LogEntry(currentNode, true, $"{currentNode?.NodeName}: ERROR - {message}"));
            }
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

        public void SetCurrentNode(IRootNode node)
        {
            currentNode = node ?? throw new ArgumentNullException(nameof(node));
        }

        public bool NodeHasErrors(IRootNode node)
        {
            if (node == null) throw new ArgumentNullException(nameof(node));

            return logEntries.Any(message => message.IsError && message.Node == node);
        }

        public string[] NodeFailedMessages(IRootNode node)
        {
            return logEntries.Where(entry => entry.IsError && entry.Node == node)
                .Select(entry => entry.Message)
                .ToArray();
        }

        public string[] FailedRootMessages()
        {
            return logEntries.Where(entry => entry.IsError && entry.Node == null)
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