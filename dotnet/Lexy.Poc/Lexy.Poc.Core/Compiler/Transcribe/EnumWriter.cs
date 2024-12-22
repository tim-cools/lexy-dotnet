using System;
using System.Collections.Generic;
using System.Linq;
using Lexy.Poc.Core.Language;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;
using static Lexy.Poc.Core.Transcribe.ExpressionSyntaxFactory;
using static Lexy.Poc.Core.Transcribe.LexySyntaxFactory;

namespace Lexy.Poc.Core.Transcribe
{
    public class EnumWriter : IRootTokenWriter
    {
        public GeneratedClass CreateCode(IRootComponent component, Components components)
        {
            if (!(component is EnumDefinition enumDefinition))
            {
                throw new InvalidOperationException("Root token not Function");
            }

            var name = enumDefinition.Name.Value;
            var members = WriteValues(enumDefinition);

            var enumNode = EnumDeclaration(name)
                .WithMembers(SeparatedList<EnumMemberDeclarationSyntax>(members))
                .WithModifiers(Modifiers.PublicAsList);

            return new GeneratedClass(enumDefinition, name, enumNode);

        }

        private SyntaxNodeOrToken[] WriteValues(EnumDefinition enumDefinition)
        {
            var result = new List<SyntaxNodeOrToken>();
            foreach (var value in enumDefinition.Members)
            {
                if (result.Count > 0)
                {
                    result.Add(Token(SyntaxKind.CommaToken));
                }

                var declaration = EnumMemberDeclaration(value.Name);
                if (value.Value != null)
                {
                    declaration = declaration.WithEqualsValue(
                        EqualsValueClause(
                        SyntaxFactory.LiteralExpression(SyntaxKind.NumericLiteralExpression,
                            Literal((int) value.Value.NumberValue))));
                }
                result.Add(declaration);
            }

            return result.ToArray();
        }
    }

    internal class Modifiers
    {
        public static SyntaxToken Public => SyntaxFactory.Token(SyntaxKind.PublicKeyword);
        public static SyntaxToken Static => SyntaxFactory.Token(SyntaxKind.StaticKeyword);
        public static SyntaxTokenList PublicAsList => SyntaxTokenList.Create(Public);
        public static SyntaxTokenList StaticAsList => SyntaxTokenList.Create(Static);
    }
}