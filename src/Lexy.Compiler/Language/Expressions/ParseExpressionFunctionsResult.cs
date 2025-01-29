using Lexy.Compiler.Language.Expressions.Functions;

namespace Lexy.Compiler.Language.Expressions;

public sealed class ParseExpressionFunctionsResult : ParseResult<FunctionCallExpression>
{
    private ParseExpressionFunctionsResult(FunctionCallExpression result) : base(result)
    {
    }

    private ParseExpressionFunctionsResult(bool success, string errorMessage) : base(success, errorMessage)
    {
    }

    public static ParseExpressionFunctionsResult Success(FunctionCallExpression result = null)
    {
        return new ParseExpressionFunctionsResult(result);
    }

    public static ParseExpressionFunctionsResult Failed(string errorMessage)
    {
        return new ParseExpressionFunctionsResult(false, errorMessage);
    }
}