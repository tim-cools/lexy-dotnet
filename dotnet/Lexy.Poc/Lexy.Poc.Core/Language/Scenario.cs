using System;
using Lexy.Poc.Core.Parser;

namespace Lexy.Poc.Core.Language
{
    public class Scenario : RootComponent
    {
        public Comments Comments { get; } = new Comments();
        public ScenarioName Name { get; } = new ScenarioName();
        public ScenarioFunctionName FunctionName { get; } = new ScenarioFunctionName();
        public ScenarioParameters Parameters { get; } = new ScenarioParameters();
        public ScenarioResults Results { get; } = new ScenarioResults();
        public ScenarioExpectError ExpectError { get; } = new ScenarioExpectError();
        public ScenarioTable Table { get; } = new ScenarioTable();

        public override string TokenName => Name.Value;

        private Scenario(string name)
        {
            Name.ParseName(name);
        }

        internal static Scenario Parse(ComponentName name)
        {
            return new Scenario(name.Parameter);
        }

        public override IComponent Parse(ParserContext parserContext)
        {
            var line = parserContext.CurrentLine;
            if (line.IsTokenType<CommentToken>(0))
            {
                return Comments;
            }

            var name = line.TokenValue(0);
            if (!line.IsTokenType<KeywordToken>(0))
            {
                throw new InvalidOperationException($"Invalid token type '{name}': {line.TokenType<KeywordToken>(0)} {line}");
            }

            return name switch
            {
                TokenNames.Function => FunctionName.Parse(parserContext),
                TokenNames.Parameters => Parameters,
                TokenNames.Results => Results,
                TokenNames.Table => Table,
                TokenNames.Comment => Comments,
                TokenNames.ExpectError => ExpectError.Parse(parserContext),
                _ => throw new InvalidOperationException($"Invalid token '{name}'. {line}")
            };
        }
    }
}
