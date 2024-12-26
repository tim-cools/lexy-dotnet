using System;
using Lexy.Compiler.Language;
using Lexy.Compiler.Parser.Tokens;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Lexy.Compiler.Compiler.CSharp
{
    internal static class Types
    {
        public static ExpressionSyntax PrimitiveTypeDefaultExpression(PrimitiveVariableDeclarationType type)
        {
            switch (type.Type)
            {
                case TypeNames.Number:
                case TypeNames.Boolean:
                    var typeSyntax = Syntax(type);
                    return SyntaxFactory.DefaultExpression(typeSyntax);

                case TypeNames.String:
                    return SyntaxFactory.LiteralExpression(
                        SyntaxKind.StringLiteralExpression,
                        SyntaxFactory.Literal(""));

                case TypeNames.Date:
                    return TranslateDate(DateTypeDefault.Value);

                default:
                    throw new InvalidOperationException("Invalid type: " + type.Type);
            }
        }

        public static ExpressionSyntax TranslateDate(DateTimeLiteral dateTimeLiteral)
        {
            return TranslateDate(dateTimeLiteral.DateTimeValue);
        }

        private static ExpressionSyntax TranslateDate(DateTime dateTimeValue)
        {
            return SyntaxFactory.ObjectCreationExpression(
                    SyntaxFactory.QualifiedName(
                        SyntaxFactory.IdentifierName("System"),
                        SyntaxFactory.IdentifierName("DateTime")))
                .WithArgumentList(
                    SyntaxFactory.ArgumentList(
                        SyntaxFactory.SeparatedList<ArgumentSyntax>(
                            new SyntaxNodeOrToken[]
                            {
                                Arguments.Numeric(dateTimeValue.Year),
                                SyntaxFactory.Token(SyntaxKind.CommaToken),
                                Arguments.Numeric(dateTimeValue.Month),
                                SyntaxFactory.Token(SyntaxKind.CommaToken),
                                Arguments.Numeric(dateTimeValue.Day),
                                SyntaxFactory.Token(SyntaxKind.CommaToken),
                                Arguments.Numeric(dateTimeValue.Hour),
                                SyntaxFactory.Token(SyntaxKind.CommaToken),
                                Arguments.Numeric(dateTimeValue.Minute),
                                SyntaxFactory.Token(SyntaxKind.CommaToken),
                                Arguments.Numeric(dateTimeValue.Second)
                            })));
        }

        public static TypeSyntax Syntax(VariableDefinition variableDefinition) => Syntax(variableDefinition.Type);

        public static TypeSyntax Syntax(string type)
        {
            return type switch
            {
                TypeNames.String => SyntaxFactory.PredefinedType(SyntaxFactory.Token(SyntaxKind.StringKeyword)),
                TypeNames.Number => SyntaxFactory.PredefinedType(SyntaxFactory.Token(SyntaxKind.DecimalKeyword)),
                TypeNames.Date => SyntaxFactory.ParseName("System.DateTime"),
                TypeNames.Boolean => SyntaxFactory.PredefinedType(SyntaxFactory.Token(SyntaxKind.BoolKeyword)),
                _ => throw new InvalidOperationException("Couldn't map type: " + type)
            };
        }

        public static TypeSyntax Syntax(VariableType variableType)
        {

            return variableType switch
            {
                PrimitiveType primitive => Syntax(primitive.Type),
                EnumType enumType => SyntaxFactory.IdentifierName(enumType.Type),
                TableType tableType => SyntaxFactory.IdentifierName(tableType.Type),
                _ => throw new InvalidOperationException("Couldn't map type: " + variableType)
            };
        }

        public static TypeSyntax Syntax(VariableDeclarationType type)
        {
            return type switch
            {
                PrimitiveVariableDeclarationType primitive => Syntax(primitive.Type),
                CustomVariableDeclarationType enumType => SyntaxFactory.IdentifierName(enumType.Type),
                ImplicitVariableDeclaration implicitVariable => Syntax(implicitVariable.VariableType),
                _ => throw new InvalidOperationException("Couldn't map type: " + type)
            };
        }
    }
}