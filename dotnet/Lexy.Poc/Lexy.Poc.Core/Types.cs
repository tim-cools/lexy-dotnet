namespace Lexy.Poc.Core
{
    public enum Types
    {
        Number,
        Boolean,
        DateTime,
        String
    }
}
/*
using Lexy.Poc.Core.RunTime;

namespace Lexy.Runtime
{
    public class FunctionTestSimpleReturn
    {
        decimal Result;
        public Lexy.Poc.Core.RunTime.FunctionResult __Result()
        {
            var result = new Lexy.Poc.Core.RunTime.FunctionResult();
            return result;
        }

        public void __Run(IExecutionContext context)
        {
            context.LogDebug(5:     Result = 777);
            Result = "777m";
        }
    }
}*/