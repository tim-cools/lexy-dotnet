using System;
using System.Collections.Generic;
using Lexy.Poc.Core.Parser;
using Lexy.Poc.Core.Parser.Tokens;

namespace Lexy.Poc.Core.Language
{
    public class Scenario : RootNode
    {
        public ScenarioName Name { get; }

        public Function Function { get; private set; }
        public EnumDefinition Enum { get; private set; }
        public Table Table { get; private set; }

        public ScenarioFunctionName FunctionName { get; }

        public ScenarioParameters Parameters { get; }
        public ScenarioResults Results { get; }
        public ScenarioTable ValidationTable { get; }

        public ScenarioExpectError ExpectError { get; }
        public ScenarioExpectRootErrors ExpectRootErrors { get; }

        public override string NodeName => Name.Value;

        private Scenario(string name, SourceReference reference) : base(reference)
        {
            Name = new ScenarioName(reference);
            FunctionName = new ScenarioFunctionName(reference);

            Parameters = new ScenarioParameters(reference);
            Results = new ScenarioResults(reference);
            ValidationTable = new ScenarioTable(reference);

            ExpectError = new ScenarioExpectError(reference);
            ExpectRootErrors = new ScenarioExpectRootErrors(reference);

            Name.ParseName(name);
        }

        internal static Scenario Parse(NodeName name, SourceReference reference)
        {
            return new Scenario(name.Name, reference);
        }

        public override IParsableNode Parse(IParserContext context)
        {
            var line = context.CurrentLine;
            if (line.IsComment())
            {
                throw new InvalidOperationException("No comments expected. Comment should be parsed by Document only.");
            }

            var name = line.Tokens.TokenValue(0);
            var reference = context.LineStartReference();
            if (!line.Tokens.IsTokenType<KeywordToken>(0))
            {
                context.Logger.Fail(reference, $"Invalid token '{name}'. Keyword expected.");
                return this;
            }

            return name switch
            {
                Keywords.FunctionKeyword => ParseFunction(context, reference),
                Keywords.EnumKeyword => ParseEnum(context, reference),
                Keywords.TableKeyword => ParseTable(context, reference),

                Keywords.Function => ResetRootNode(context, ParseFunctionName(context)),
                Keywords.Parameters => ResetRootNode(context, Parameters),
                Keywords.Results => ResetRootNode(context, Results),
                Keywords.ValidationTable => ResetRootNode(context, ValidationTable),
                Keywords.ExpectError => ResetRootNode(context, ExpectError.Parse(context)),
                Keywords.ExpectRootErrors => ResetRootNode(context, ExpectRootErrors),

                _ => InvalidToken(context, name, reference)
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

        private IParsableNode ParseFunction(IParserContext context, SourceReference reference)
        {
            if (Function != null)
            {
                context.Logger.Fail(reference, $"Duplicated inline Function '{NodeName}'.");
                return null;
            }

            var tokenName = Parser.NodeName.Parse(context.CurrentLine, context);

            Function = Function.Create(tokenName?.Name ?? $"{Name.Value}Function", reference);
            context.Logger.SetCurrentNode(Function);
            return Function;
        }

        private IParsableNode ParseEnum(IParserContext context, SourceReference reference)
        {
            if (Enum != null)
            {
                context.Logger.Fail(reference, $"Duplicated inline Enum '{NodeName}'.");
                return null;
            }

            var tokenName = Parser.NodeName.Parse(context.CurrentLine, context);

            Enum = EnumDefinition.Parse(tokenName, reference);
            context.Logger.SetCurrentNode(Enum);
            return Enum;
        }

        private IParsableNode ParseTable(IParserContext context, SourceReference reference)
        {
            if (Table != null)
            {
                context.Logger.Fail(reference, $"Duplicated inline Enum '{NodeName}'.");
                return null;
            }

            var tokenName = Parser.NodeName.Parse(context.CurrentLine, context);

            Table = Table.Parse(tokenName, reference);
            context.Logger.SetCurrentNode(Table);
            return Table;
        }

        private IParsableNode InvalidToken(IParserContext parserContext, string name, SourceReference reference)
        {
            parserContext.Logger.Fail(reference, $"Invalid token '{name}'.");
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
