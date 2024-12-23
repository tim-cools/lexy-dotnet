using System;
using System.Collections.Generic;
using Lexy.Poc.Core.Language.Expressions;
using Lexy.Poc.Core.Parser;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace Lexy.Poc.Core.Transcribe
{
    internal static class ExpressionSyntaxFactory
    {
        private static readonly IDictionary<ExpressionOperator, SyntaxKind> translateOperators =
            new Dictionary<ExpressionOperator, SyntaxKind>()
            {
                { ExpressionOperator.Addition, SyntaxKind.AddExpression },
                { ExpressionOperator.Subtraction, SyntaxKind.SubtractExpression },
                { ExpressionOperator.Multiplication, SyntaxKind.MultiplyExpression },
                { ExpressionOperator.Division, SyntaxKind.DivideExpression },
                { ExpressionOperator.Modulus, SyntaxKind.ModuloExpression },

                { ExpressionOperator.GreaterThan, SyntaxKind.GreaterThanExpression },
                { ExpressionOperator.GreaterThanOrEqual, SyntaxKind.GreaterThanOrEqualExpression },
                { ExpressionOperator.LessThan, SyntaxKind.LessThanExpression },
                { ExpressionOperator.LessThanOrEqual, SyntaxKind.LessThanOrEqualExpression },

                { ExpressionOperator.And, SyntaxKind.LogicalAndExpression },
                { ExpressionOperator.Or, SyntaxKind.LogicalOrExpression },
                { ExpressionOperator.Equals, SyntaxKind.EqualsExpression },
                { ExpressionOperator.NotEqual, SyntaxKind.NotEqualsExpression },
            };

        public static StatementSyntax ExpressionStatementSyntax(Expression line)
        {
            return line switch
            {
                AssignmentExpression assignment => TranslateAssignmentExpression(assignment),
                VariableDeclarationExpression variableDeclarationExpression => TranslateVariableDeclarationExpression(variableDeclarationExpression),
                _ => throw new InvalidOperationException($"Wrong expression type {line.GetType()}: {line}")
            };
        }

        private static ExpressionStatementSyntax TranslateAssignmentExpression(AssignmentExpression assignment)
        {
            return ExpressionStatement(
                AssignmentExpression(
                    SyntaxKind.SimpleAssignmentExpression,
                    IdentifierName(assignment.VariableName),
                    ExpressionSyntax(assignment.Assignment)));
        }

        private static StatementSyntax TranslateVariableDeclarationExpression(VariableDeclarationExpression expression)
        {
            var variable = VariableDeclarator(
                Identifier(expression.VariableName));

            if (expression.Assignment != null)
            {
                variable = variable
                    .WithInitializer(
                        EqualsValueClause(
                            ExpressionSyntax(expression.Assignment)));
            }

            return LocalDeclarationStatement(
                VariableDeclaration(
                        LexySyntaxFactory.MapType(expression.VariableType))
                    .WithVariables(
                        SingletonSeparatedList(
                            variable)));
        }

        public static ExpressionSyntax ExpressionSyntax(Expression line)
        {
            return line switch
            {
                LiteralExpression expression => LexySyntaxFactory.TokenValueExpression(expression.Literal),
                VariableExpression expression => SyntaxFactory.IdentifierName(expression.VariableName),
                MemberAccessExpression expression => TranslateMemberAccessExpression(expression),
                BinaryExpression expression => TranslateBinaryExpression(expression),
                ParenthesizedExpression expression => SyntaxFactory.ParenthesizedExpression(ExpressionSyntax(expression.Expression)),
                _ => throw new InvalidOperationException($"Wrong expression type {line.GetType()}: {line}")
            };
        }

        private static ExpressionSyntax TranslateBinaryExpression(BinaryExpression expression)
        {
            var kind = Translate(expression.Operator);
            return SyntaxFactory.BinaryExpression(
                kind,
                ExpressionSyntax(expression.Left),
                ExpressionSyntax(expression.Right));
        }

        private static SyntaxKind Translate(ExpressionOperator expressionOperator)
        {
            if (!translateOperators.TryGetValue(expressionOperator, out var result))
            {
                throw new ArgumentOutOfRangeException(nameof(expressionOperator), expressionOperator, null);
            }

            return result;
        }

        private static ExpressionSyntax TranslateMemberAccessExpression(MemberAccessExpression expression)
        {
            var parts = expression.Value.Split(TokenValues.MemberAccess);
            if (parts.Length < 2)
            {
                throw new InvalidOperationException("Invalid MemberAccessExpression: " + expression);
            }

            ExpressionSyntax result = SyntaxFactory.MemberAccessExpression(
                SyntaxKind.SimpleMemberAccessExpression,
                SyntaxFactory.IdentifierName(parts[0]),
                SyntaxFactory.IdentifierName(parts[1]));

            for (var index = 2; index < parts.Length; index++)
            {
                result = SyntaxFactory.MemberAccessExpression(
                    SyntaxKind.SimpleMemberAccessExpression,
                    result,
                    SyntaxFactory.IdentifierName(parts[1]));
            }

            return result;
        }
    }
}