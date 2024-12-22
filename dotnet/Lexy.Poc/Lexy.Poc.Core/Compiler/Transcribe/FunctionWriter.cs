using System;
using System.Collections.Generic;
using System.Linq;
using Lexy.Poc.Core.Language;
using Lexy.Poc.Core.Language.Expressions;
using Lexy.Poc.Core.Parser;
using Lexy.Poc.Core.Parser.Tokens;
using Lexy.Poc.Core.RunTime;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;
using static Lexy.Poc.Core.Transcribe.ExpressionSyntaxFactory;
using static Lexy.Poc.Core.Transcribe.LexySyntaxFactory;

namespace Lexy.Poc.Core.Transcribe
{
    public class FunctionWriter : IRootTokenWriter
    {
        public GeneratedClass CreateCode(IRootComponent component, Components components)
        {
            if (!(component is Function function))
            {
                throw new InvalidOperationException("Root token not Function");
            }

            var name = function.Name.ClassName();

            var members = new List<MemberDeclarationSyntax>();

            members.AddRange(WriteIncludes(function));
            members.AddRange(WriteVariables(function.Parameters.Variables));
            members.AddRange(WriteVariables(function.Results.Variables));

            members.Add(WriteResultMethod(function.Results.Variables));
            members.Add(WriteRunMethod(function));

            var classDeclaration = ClassDeclaration(name)
                .WithModifiers(Modifiers.PublicAsList)
                .WithMembers(List(members));

            return new GeneratedClass(function, name, classDeclaration);
        }

        private IEnumerable<MemberDeclarationSyntax> WriteIncludes(Function function)
        {
            foreach (var include in function.Include.Definitions)
            {
                if (include.Type != IncludeTypes.Table)
                    throw new InvalidOperationException("Invalid include type: " + include.Type);

                var fieldDeclaration = FieldDeclaration(
                    VariableDeclaration(
                            IdentifierName(include.Name))
                        .WithVariables(
                            SingletonSeparatedList(
                                VariableDeclarator(
                                        Identifier(include.Name))
                                    .WithInitializer(
                                        EqualsValueClause(
                                            ObjectCreationExpression(
                                                    IdentifierName(include.Name))
                                                .WithArgumentList(
                                                    ArgumentList()))))))
                    .WithModifiers(
                        TokenList(
                            Token(SyntaxKind.PublicKeyword)));

                yield return fieldDeclaration;
            }
        }

        private MemberDeclarationSyntax WriteResultMethod(IList<VariableDefinition> resultVariables)
        {
            var resultType = ParseName($"{typeof(FunctionResult).Namespace}.{nameof(FunctionResult)}");

            var statements = new List<StatementSyntax> {
                LocalDeclarationStatement(
                    VariableDeclaration(
                        IdentifierName(
                            Identifier(
                                TriviaList(),
                                SyntaxKind.VarKeyword,
                                "var",
                                "var",
                                TriviaList())))
                    .WithVariables(
                        SingletonSeparatedList<VariableDeclaratorSyntax>(
                            VariableDeclarator(
                                    Identifier("result"))
                                .WithInitializer(
                                    EqualsValueClause(
                                        ObjectCreationExpression(
                                            resultType)
                                            .WithArgumentList(
                                                ArgumentList()))))))
            };

            statements.AddRange(resultVariables.Select(variable =>
                (StatementSyntax) ExpressionStatement(
                    AssignmentExpression(
                        SyntaxKind.SimpleAssignmentExpression,
                        ElementAccessExpression(
                                IdentifierName("result"))
                            .WithArgumentList(
                                BracketedArgumentList(
                                    SingletonSeparatedList<ArgumentSyntax>(
                                        Argument(
                                            LiteralExpression(
                                                SyntaxKind.StringLiteralExpression,
                                                Literal(variable.Name)))))),
                        IdentifierName(variable.Name)))));

            statements.Add(ReturnStatement(IdentifierName("result")));

            var function = MethodDeclaration(
                resultType,
                Identifier("__Result"))
                .WithModifiers(
                    TokenList(
                        Token(SyntaxKind.PublicKeyword)))
                .WithBody(
                    Block(statements));

            return function;
        }

        private IEnumerable<MemberDeclarationSyntax> WriteVariables(IList<VariableDefinition> variables)
        {
            foreach (var variable in variables)
            {
                var variableDeclaration = VariableDeclarator(Identifier(variable.Name));
                var defaultValue = TokenValueExpression(variable.Default);
                if (defaultValue != null)
                {
                    variableDeclaration = variableDeclaration.WithInitializer(
                        EqualsValueClause(defaultValue));
                }

                var fieldDeclaration = FieldDeclaration(
                    VariableDeclaration(MapType(variable))
                        .WithVariables(
                            SingletonSeparatedList<VariableDeclaratorSyntax>(
                                variableDeclaration)))
                    .WithModifiers(Modifiers.PublicAsList);

                yield return fieldDeclaration;
            }
        }

        private MethodDeclarationSyntax WriteRunMethod(Function function)
        {
            var statements = function.Code.Expressions.SelectMany(line =>
                new []{
                    ExpressionStatement(
                        InvocationExpression(
                                MemberAccessExpression(
                                    SyntaxKind.SimpleMemberAccessExpression,
                                    IdentifierName("context"),
                                    IdentifierName(nameof(IExecutionContext.LogDebug))))
                            .WithArgumentList(
                                ArgumentList(
                                    SingletonSeparatedList<ArgumentSyntax>(
                                        Argument(
                                            LiteralExpression(
                                                SyntaxKind.StringLiteralExpression,
                                                Literal(line.SourceLine.ToString()))))))),
                    ExpressionStatementSyntax(line)
                });

            var functionSyntax = MethodDeclaration(
                    PredefinedType(
                        Token(SyntaxKind.VoidKeyword)),
                    Identifier("__Run"))
                .WithModifiers(
                    TokenList(
                        Token(SyntaxKind.PublicKeyword)))
                .WithParameterList(
                    ParameterList(
                        SingletonSeparatedList<ParameterSyntax>(
                            Parameter(
                                    Identifier("context"))
                                .WithType(
                                    IdentifierName(nameof(IExecutionContext))))))
                .WithBody(Block(statements));

            return functionSyntax;
        }

        private string Escape(Line line)
        {
            return line.ToString()
                .Replace(@"""", @"""""");
        }

        /*
        private string FormatLiteralValue(Token token)
        {
            return token switch
            {
                //StringLiteralToken _ => $@"""{token.Value}""",
                QuotedLiteralToken _ => $@"""{token.Value}""",
                NumberLiteralToken _ => $@"{token.Value}m",
                _ => token.Value
            };
        }*/
    }

    internal static class ExpressionSyntaxFactory
    {
        public static StatementSyntax ExpressionStatementSyntax(Expression line)
        {
            return line switch
            {
                AssignmentExpression assignment =>
                    ExpressionStatement(
                        AssignmentExpression(
                            SyntaxKind.SimpleAssignmentExpression,
                            IdentifierName(assignment.VariableName),
                            ExpressionSyntax(assignment.Assignment))),

                _ => throw new InvalidOperationException($"Wrong expression type {line.GetType()}: {line}")
            };
        }

        public static ExpressionSyntax ExpressionSyntax(Expression line)
        {
            return line switch
            {
                LiteralExpression expression => TokenValueExpression(expression.Literal),
                VariableExpression expression => IdentifierName(expression.VariableName),
                MemberAccessExpression expression => TranslateMemberAccessExpression(expression),
                _ => throw new InvalidOperationException($"Wrong expression type {line.GetType()}: {line}")
            };
        }

        private static ExpressionSyntax TranslateMemberAccessExpression(MemberAccessExpression expression)
        {
            var parts = expression.Value.Split(TokenValues.MemberAccess);
            if (parts.Length < 2)
            {
                throw new InvalidOperationException("Invalid MemberAccessExpression: " + expression);
            }

            ExpressionSyntax result = MemberAccessExpression(
                SyntaxKind.SimpleMemberAccessExpression,
                IdentifierName(parts[0]),
                IdentifierName(parts[1]));

            for (var index = 2; index < parts.Length; index++)
            {
                result = MemberAccessExpression(
                    SyntaxKind.SimpleMemberAccessExpression,
                    result,
                    IdentifierName(parts[1]));
            }

            return result;
        }
    }

    public static class LexySyntaxFactory
    {
        public static ExpressionSyntax TokenValueExpression(ILiteralToken token)
        {
            if (token == null) return null;

            return token switch
            {
                QuotedLiteralToken _ => LiteralExpression(SyntaxKind.StringLiteralExpression, Literal(token.Value)),
                NumberLiteralToken number => LiteralExpression(SyntaxKind.NumericLiteralExpression, Literal($"{number.NumberValue}m", number.NumberValue)),
                //DateTimeLiteral _ => LiteralExpression(SyntaxKind.StringLiteralExpression, Literal(token.Value)),
                BooleanLiteral boolean => LiteralExpression(boolean.BooleanValue ? SyntaxKind.TrueLiteralExpression : SyntaxKind.FalseLiteralExpression),
                _ => throw new InvalidOperationException("Couldn't map type: " + token.GetType())
            };
        }

        public static TypeSyntax MapType(VariableDefinition variableDefinition) => MapType(variableDefinition.Type);

        public static TypeSyntax MapType(string type)
        {
            return type switch
            {
                TypeNames.String => PredefinedType(Token(SyntaxKind.StringKeyword)),
                TypeNames.Number => PredefinedType(Token(SyntaxKind.DecimalKeyword)),
                TypeNames.DateTime => ParseName("System.DateTime"),
                TypeNames.Boolean => PredefinedType(Token(SyntaxKind.BoolKeyword)),
                _ => throw new InvalidOperationException("Couldn't map type: " + type)
            };
        }

        public static TypeSyntax MapType(VariableType type)
        {
            return type switch
            {
                PrimitiveVariableType primitive => MapType(primitive.Type),
                EnumVariableType enumType => IdentifierName(enumType.EnumName),
                _ => throw new InvalidOperationException("Couldn't map type: " + type)
            };
        }
    }
}