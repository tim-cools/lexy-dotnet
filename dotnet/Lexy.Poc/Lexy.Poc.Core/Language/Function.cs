using System;
using System.Collections.Generic;
using System.Linq;
using Lexy.Poc.Core.Parser;
using Lexy.Poc.Core.Parser.Tokens;

namespace Lexy.Poc.Core.Language
{
    public class Function : RootNode
    {
        private static readonly LambdaComparer<IRootNode> NodeComparer =
            new LambdaComparer<IRootNode>((token1, token2) => token1.NodeName == token2.NodeName);

        public FunctionName Name { get; } = new FunctionName();
        public FunctionParameters Parameters { get; } = new FunctionParameters();
        public FunctionResults Results { get; } = new FunctionResults();
        public FunctionCode Code { get; } = new FunctionCode();
        public FunctionIncludes Include { get; } = new FunctionIncludes();

        public override string NodeName => Name.Value;

        private Function(string name)
        {
            Name.ParseName(name);
        }

        internal static Function Create(string name)
        {
            return new Function(name);
        }

        public override IParsableNode Parse(IParserContext context)
        {
            var line = context.CurrentLine;
            if (line.IsComment())
            {
                throw new InvalidOperationException("No comments expected. Comment should be parsed by Document only.");
            }

            var name = line.Tokens.TokenValue(0);
            if (!line.Tokens.IsTokenType<KeywordToken>(0))
            {
                return InvalidToken(name, line, context);
            }

            return name switch
            {
                Keywords.Parameters => Parameters,
                Keywords.Results => Results,
                Keywords.Code => Code,
                Keywords.Include => Include,
                 _ => InvalidToken(name, line, context)
            };
        }

        private IParsableNode InvalidToken(string name, Line line, IParserContext parserContext)
        {
            parserContext.Logger.Fail($"Invalid token '{name}'. {line}");
            return this;
        }

        public IEnumerable<IRootNode> GetDependencies(Nodes nodes)
        {
            var result = new List<IRootNode>();
            AddEnumTypes(nodes, Parameters.Variables, result);
            AddEnumTypes(nodes, Results.Variables, result);
            AddIncludes(nodes, Include.Definitions, result);
            return result.Distinct(NodeComparer);
        }

        private static void AddEnumTypes(Nodes nodes, IList<VariableDefinition> variableDefinitions, List<IRootNode> result)
        {
            foreach (var parameter in variableDefinitions)
            {
                if (!(parameter.Type is CustomVariableType enumVariableType)) continue;

                var dependency = nodes.GetEnum(enumVariableType.TypeName);
                if (dependency == null)
                {
                    throw new InvalidOperationException("Type or enum not found: " + parameter.Type);
                }

                result.Add(dependency);
            }
        }

        private void AddIncludes(Nodes nodes, IList<FunctionInclude> functionIncludes, List<IRootNode> result)
        {
            foreach (var include in functionIncludes)
            {
                var dependency = include.Type == IncludeTypes.Table
                    ? nodes.GetTable(include.Name)
                    : throw new InvalidOperationException("Include not yet supported; " + include.Type);

                if (dependency == null)
                {
                    throw new InvalidOperationException($"Include {include.Type} node not found {include.Name}");
                }

                result.Add(dependency);
            }
        }

        protected override IEnumerable<INode> GetChildren()
        {
            yield return Name;
            yield return Parameters;
            yield return Results;
            yield return Code;
            yield return Include;
        }

        protected override void Validate(IParserContext context)
        {
            ValidateDuplicatedVariablesNames(context);
        }

        private void ValidateDuplicatedVariablesNames(IParserContext context)
        {
            //var variableDeclarations = Code.GetVariableDeclarations();
            DuplicateChecker.Validate(
                context,
                member => member.Name,
                member => $"Duplicated variable name: '{member.Name}'",
                Parameters.Variables,
                Results.Variables);
        }
    }
}