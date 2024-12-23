using System;
using System.Collections.Generic;
using Lexy.Poc.Core.Parser;
using Lexy.Poc.Core.Parser.Tokens;

namespace Lexy.Poc.Core.Language
{
    public class Scenario : RootNode
    {
        public ScenarioName Name { get; } = new ScenarioName();

        public Function Function { get; private set; }
        public EnumDefinition Enum { get; private set; }
        public Table Table { get; private set; }

        public ScenarioFunctionName FunctionName { get; } = new ScenarioFunctionName();

        public ScenarioParameters Parameters { get; } = new ScenarioParameters();
        public ScenarioResults Results { get; } = new ScenarioResults();
        public ScenarioTable ValidationTable { get; } = new ScenarioTable();

        public ScenarioExpectError ExpectError { get; } = new ScenarioExpectError();
        public ScenarioExpectRootErrors ExpectRootErrors { get; } = new ScenarioExpectRootErrors();

        public override string NodeName => Name.Value;

        private Scenario(string name)
        {
            Name.ParseName(name);
        }

        internal static Scenario Parse(NodeName name)
        {
            return new Scenario(name.Name);
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
                context.Logger.Fail($"Invalid token '{name}'. Keyword expected.");
                return this;
            }

            return name switch
            {
                Keywords.FunctionKeyword => ParseFunction(context),
                Keywords.EnumKeyword => ParseEnum(context),
                Keywords.TableKeyword => ParseTable(context),

                Keywords.Function => ResetRootNode(context, ParseFunctionName(context)),
                Keywords.Parameters => ResetRootNode(context, Parameters),
                Keywords.Results => ResetRootNode(context, Results),
                Keywords.ValidationTable => ResetRootNode(context, ValidationTable),
                Keywords.ExpectError => ResetRootNode(context, ExpectError.Parse(context)),
                Keywords.ExpectRootErrors => ResetRootNode(context, ExpectRootErrors),
                _ => InvalidToken(context, name)
            };
        }

        private IParsableNode ResetRootNode(IParserContext parserContext, IParsableNode node)
        {
            parserContext.ProcessNode(this);
            return node;
        }

        private IParsableNode ParseFunctionName(IParserContext context)
        {
            FunctionName.Parse(context);
            return this;
        }

        private IParsableNode ParseFunction(IParserContext context)
        {
            if (Function != null)
            {
                context.Logger.Fail($"Duplicated inline Function '{NodeName}'.");
                return null;
            }

            var tokenName = Parser.NodeName.Parse(context.CurrentLine, context);

            Function = Function.Create(tokenName?.Name ?? $"{Name.Value}Function");
            context.Logger.SetCurrentNode(Function);
            return Function;
        }

        private IParsableNode ParseEnum(IParserContext context)
        {
            if (Enum != null)
            {
                context.Logger.Fail($"Duplicated inline Enum '{NodeName}'.");
                return null;
            }

            var tokenName = Parser.NodeName.Parse(context.CurrentLine, context);

            Enum = EnumDefinition.Parse(tokenName);
            context.Logger.SetCurrentNode(Enum);
            return Enum;
        }

        private IParsableNode ParseTable(IParserContext context)
        {
            if (Table != null)
            {
                context.Logger.Fail($"Duplicated inline Enum '{NodeName}'.");
                return null;
            }

            var tokenName = Parser.NodeName.Parse(context.CurrentLine, context);

            Table = Table.Parse(tokenName);
            context.Logger.SetCurrentNode(Table);
            return Table;
        }

        private IParsableNode InvalidToken(IParserContext parserContext, string name)
        {
            parserContext.Logger.Fail($"Invalid token '{name}'.");
            return this;
        }

        protected override IEnumerable<INode> GetChildren()
        {
            yield return Name;

            if (Function != null) yield return Function;
            if (Enum != null) yield return Enum;
            if (Table != null) yield return Table;

            yield return FunctionName;
            yield return Parameters;
            yield return Results;
            yield return ValidationTable;
            yield return ExpectError;
            yield return ExpectRootErrors;
        }

        protected override void Validate(IValidationContext context)
        {
        }
    }
}
