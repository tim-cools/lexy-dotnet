using System;
using System.Collections.Generic;
using Lexy.Compiler.Language.Expressions;
using Lexy.Compiler.Language.Expressions.Functions;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace Lexy.Compiler.Compiler.CSharp.ExpressionStatementExceptions;

internal class NewFunctionExpressionStatementException : IExpressionStatementException
{
    public bool Matches(Expression expression)
    {
        return expression is VariableDeclarationExpression assignmentExpression
               && assignmentExpression.Assignment is FunctionCallExpression functionCallExpression
               && functionCallExpression is NewFunction;
    }

    public IEnumerable<StatementSyntax> CallExpressionSyntax(Expression expression)
    {
        if (expression is not VariableDeclarationExpression assignmentExpression)
            throw new InvalidOperationException("expression should be VariableDeclarationExpression");
        if (assignmentExpression.Assignment is not FunctionCallExpression functionCallExpression)
            throw new InvalidOperationException("assignmentExpression.Assignment should be FunctionCallExpression");
        if (functionCallExpression is not NewFunction)
            throw new InvalidOperationException("functionCallExpression.FunctionCallExpression should be NewFunction");

        var typeSyntax = Types.Syntax(assignmentExpression.Type);

        var initialize = ObjectCreationExpression(typeSyntax)
            .WithArgumentList(ArgumentList());

        var variable = VariableDeclarator(Identifier(assignmentExpression.Name))
            .WithInitializer(EqualsValueClause(initialize));

        yield return LocalDeclarationStatement(
            VariableDeclaration(typeSyntax)
                .WithVariables(SingletonSeparatedList(variable)));
    }
}