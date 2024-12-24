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
        private readonly IList<LogEntry> logEntries = new List<LogEntry>();
        private int failedMessages;
        private IRootNode currentNode;

        public bool HasErrors() => failedMessages > 0;
        public bool HasRootErrors() => logEntries.Any(entry => entry.IsError && entry.Node == null);

        public ParserLogger(ILogger<ParserLogger> logger)
        {
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public void LogInfo(string message) => logger.LogInformation(message);

        public void Log(SourceReference reference, string message)
        {
            if (reference == null) throw new ArgumentNullException(nameof(reference));
            if (message == null) throw new ArgumentNullException(nameof(message));

            logger.LogDebug("{Reference}: {Message}", reference, message);
            logEntries.Add(new LogEntry(currentNode, false, $"{reference}: {message}"));
        }

        public void Fail(SourceReference reference, string message)
        {
            if (reference == null) throw new ArgumentNullException(nameof(reference));
            if (message == null) throw new ArgumentNullException(nameof(message));

            failedMessages++;

            logger.LogError("{Reference}: ERROR - {Message}", reference, message);
            logEntries.Add(new LogEntry(currentNode, true, $"{reference}: ERROR - {message}"));
        }

        public void Fail(INode node, SourceReference reference, string message)
        {
            if (reference == null) throw new ArgumentNullException(nameof(reference));
            if (message == null) throw new ArgumentNullException(nameof(message));

            failedMessages++;

            logger.LogError("{Reference}: ERROR - {Message}", reference, message);
            logEntries.Add(new LogEntry(node, true, $"{reference}: ERROR - {message}"));
        }

        public void LogNodes(IEnumerable<INode> nodes)
        {
            if (!logger.IsEnabled(LogLevel.Debug)) return;

            var nodeLogger = new NodesLogger();
            nodeLogger.Log(nodes);

            logger.LogDebug("Parsed nodes: " + nodeLogger.ToString());
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

        public void Reset() => currentNode = null;

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

    public class NodesLogger
    {
        private readonly StringBuilder builder = new StringBuilder();
        private int indent;

        public void Log(IEnumerable<INode> nodes)
        {
            foreach (var node in nodes)
            {
                Log(node);
            }
        }

        private void Log(INode node)
        {
            builder.Append(new string(' ', indent));

            if (node is IRootNode rootNode)
            {
                builder.AppendLine($"{rootNode.GetType().Name}: {rootNode.NodeName}");
            }
            else
            {
                builder.AppendLine(node == null ? "<null>" : node?.GetType().Name);
            }

            if (node == null) return;

            var children = node.GetChildren();

            indent += 2;
            Log(children);
            indent -= 2;
        }

        public override string ToString() => builder.ToString();
    }

    public class SourceReference
    {
        private readonly int? lineNumber;
        private readonly int? characterNumber;

        public SourceFile File { get; }

        public SourceReference(SourceFile file, int? lineNumber, int? characterNumber)
        {
            File = file ?? throw new ArgumentNullException(nameof(file));
            this.characterNumber = characterNumber;
            this.lineNumber = lineNumber;
        }

        public override string ToString() => $"{File.FileName}({lineNumber}, {characterNumber})";
    }
}