using System;
using Lexy.Poc.Core.Language.Expressions;
using Lexy.Poc.Core.Parser;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Lexy.Poc.Core.Transcribe
{
    internal static class ExpressionSyntaxFactory
    {
        public static StatementSyntax ExpressionStatementSyntax(Expression line)
        {
            return line switch
            {
                AssignmentExpression assignment =>
                    SyntaxFactory.ExpressionStatement(
                        SyntaxFactory.AssignmentExpression(
                            SyntaxKind.SimpleAssignmentExpression,
                            SyntaxFactory.IdentifierName(assignment.VariableName),
                            ExpressionSyntax(assignment.Assignment))),

                _ => throw new InvalidOperationException($"Wrong expression type {line.GetType()}: {line}")
            };
        }

        public static ExpressionSyntax ExpressionSyntax(Expression line)
        {
            return line switch
            {
                LiteralExpression expression => LexySyntaxFactory.TokenValueExpression(expression.Literal),
                VariableExpression expression => SyntaxFactory.IdentifierName(expression.VariableName),
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