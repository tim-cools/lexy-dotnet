using System;
using System.Diagnostics;
using Lexy.Poc.Core.Parser;
using Microsoft.Extensions.DependencyInjection;
using Shouldly;

namespace Lexy.Poc
{
    public static class TestContext
    {
        public static IParserContext TestLine(this IServiceProvider serviceProvider, string value, bool expectSuccess = true)
        {
            if (serviceProvider == null) throw new ArgumentNullException(nameof(serviceProvider));

            var code = new []{ value };

            var codeContext = serviceProvider.GetRequiredService<ISourceCodeDocument>();
            codeContext.SetCode(code);

            var context = serviceProvider.GetRequiredService<IParserContext>();
            if (context.ProcessLine() != expectSuccess)
            {
                throw new InvalidOperationException("Process line failed");
            }

            return context;
        }

        public static void ValidateError(this IParserContext context, string error)
        {
            context.Logger.HasErrorMessage(error)
                .ShouldBeTrue(context.Logger.FormatMessages());
        }

        public static TokenValidator ValidateTokens(this IParserContext context)
        {
            var methodInfo = new StackTrace().GetFrame(1).GetMethod();
            return context.ValidateTokens(methodInfo.ReflectedType.Name);
        }
    }
}