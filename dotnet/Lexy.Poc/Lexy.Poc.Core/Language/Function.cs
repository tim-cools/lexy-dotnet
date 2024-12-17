using System;
using System.Collections.Generic;
using System.Linq;
using Lexy.Poc.Core.Parser;

namespace Lexy.Poc.Core.Language
{
    public class Function : RootComponent
    {
        private static readonly LambdaComparer<IRootComponent> componentComparer =
            new LambdaComparer<IRootComponent>((token1, token2) => token1.TokenName == token2.TokenName);

        public Comments Comments { get; } = new Comments();
        public FunctionName Name { get; } = new FunctionName();
        public FunctionParameters Parameters { get; } = new FunctionParameters();
        public FunctionResults Results { get; } = new FunctionResults();
        public FunctionCode Code { get; } = new FunctionCode();
        public FunctionIncludes Include { get; } = new FunctionIncludes();
        public override string TokenName => Name.Value;

        private Function(string name)
        {
            Name.ParseName(name);
        }

        internal static Function Parse(ComponentName name)
        {
            return new Function(name.Parameter);
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
                return InvalidTokenType("name", line, parserContext);
            }

            return name switch
            {
                TokenNames.Parameters => Parameters,
                TokenNames.Results => Results,
                TokenNames.Code => Code,
                TokenNames.Include => Include,
                 _ => InvalidToken(name, line, parserContext)
            };
        }

        private IComponent InvalidTokenType(string name, Line line, ParserContext parserContext)
        {
            var message = $"Invalid token type '{name}': {line.TokenType<KeywordToken>(0)} {line}";
            Fail(message);
            parserContext.Fail(message);
            throw new InvalidOperationException(message);
        }

        private IComponent InvalidToken(string name, Line line, ParserContext parserContext)
        {
            var message = $"Invalid token '{name}'. {line}";
            Fail(message);
            parserContext.Fail(message);
            throw new InvalidOperationException(message);
        }

        public IEnumerable<IRootComponent> GetDependencies(Components components)
        {
            var result = new List<IRootComponent>();
            AddEnumTypes(components, Parameters.Variables, result);
            AddEnumTypes(components, Results.Variables, result);
            return result.Distinct(componentComparer);
        }

        private static void AddEnumTypes(Components components, IList<VariableDefinition> variableDefinitions, List<IRootComponent> result)
        {
            foreach (var parameter in variableDefinitions)
            {
                if (!TypeNames.Exists(parameter.Type))
                {
                    var dependency = components.GetEnum(parameter.Type);
                    if (dependency == null)
                    {
                        throw new InvalidOperationException("Type or enum not found: " + parameter.Type);
                    }

                    result.Add(dependency);
                }
            }
        }
    }
}