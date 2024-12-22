using System;
using Lexy.Poc.Core.Language;
using Lexy.Poc.Core.Parser.Tokens;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Lexy.Poc.Core.Transcribe
{
    public static class LexySyntaxFactory
    {
        public static ExpressionSyntax TokenValueExpression(ILiteralToken token)
        {
            if (token == null) return null;

            return token switch
            {
                QuotedLiteralToken _ => SyntaxFactory.LiteralExpression(SyntaxKind.StringLiteralExpression, SyntaxFactory.Literal(token.Value)),
                NumberLiteralToken number => SyntaxFactory.LiteralExpression(SyntaxKind.NumericLiteralExpression, SyntaxFactory.Literal($"{number.NumberValue}m", number.NumberValue)),
                //DateTimeLiteral _ => LiteralExpression(SyntaxKind.StringLiteralExpression, Literal(token.Value)),
                BooleanLiteral boolean => SyntaxFactory.LiteralExpression(boolean.BooleanValue ? SyntaxKind.TrueLiteralExpression : SyntaxKind.FalseLiteralExpression),
                _ => throw new InvalidOperationException("Couldn't map type: " + token.GetType())
            };
        }

        public static TypeSyntax MapType(VariableDefinition variableDefinition) => MapType(variableDefinition.Type);

        public static TypeSyntax MapType(string type)
        {
            return type switch
            {
                TypeNames.String => SyntaxFactory.PredefinedType(SyntaxFactory.Token(SyntaxKind.StringKeyword)),
                TypeNames.Number => SyntaxFactory.PredefinedType(SyntaxFactory.Token(SyntaxKind.DecimalKeyword)),
                TypeNames.DateTime => SyntaxFactory.ParseName("System.DateTime"),
                TypeNames.Boolean => SyntaxFactory.PredefinedType(SyntaxFactory.Token(SyntaxKind.BoolKeyword)),
                _ => throw new InvalidOperationException("Couldn't map type: " + type)
            };
        }

        public static TypeSyntax MapType(VariableType type)
        {
            return type switch
            {
                PrimitiveVariableType primitive => MapType(primitive.Type),
                EnumVariableType enumType => SyntaxFactory.IdentifierName(enumType.EnumName),
                _ => throw new InvalidOperationException("Couldn't map type: " + type)
            };
        }
    }
}