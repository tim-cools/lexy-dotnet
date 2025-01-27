using Lexy.Compiler.Compiler.CSharp.Syntax;
using Lexy.Compiler.Language.Expressions.Functions;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Lexy.Compiler.Compiler.CSharp.BuiltInFunctions;

internal abstract class EndStartDateFunctionCall<TExpressionFunction> : MethodFunctionCall<TExpressionFunction>
    where TExpressionFunction : EndStartDateFunction
{
    protected override SeparatedSyntaxList<ArgumentSyntax> GetArguments(TExpressionFunction powerFunction)
    {
        return SyntaxFactory.SeparatedList<ArgumentSyntax>(
            new SyntaxNodeOrToken[]
            {
                SyntaxFactory.Argument(Expressions.ExpressionSyntax(powerFunction.EndDateExpression)),
                SyntaxFactory.Token(SyntaxKind.CommaToken),
                SyntaxFactory.Argument(Expressions.ExpressionSyntax(powerFunction.StartDateExpression))
            });
    }
}