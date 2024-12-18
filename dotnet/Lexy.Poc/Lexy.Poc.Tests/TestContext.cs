using System.Diagnostics;
using Lexy.Poc.Core.Parser;

namespace Lexy.Poc
{
    public static class TestContext
    {
        public static ParserContext TestLine(string value)
        {
            var code = new []{ value };
            var line = new Line(0, value, code);
            var context = new ParserContext(new[] { line});

            context.ProcessLine(line);

            return context;
        }

        public static TokenValidator ValidateTokens(this ParserContext context)
        {
            var methodInfo = new StackTrace().GetFrame(1).GetMethod();
            return context.ValidateTokens(methodInfo.ReflectedType.Name);
        }
    }
}