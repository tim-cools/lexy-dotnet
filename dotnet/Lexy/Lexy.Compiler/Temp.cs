using Lexy.RunTime.RunTime;

namespace Lexy.Compiler
{
    public class FunctionTestSimpleReturn
    {
        public decimal Input = 5m;
        public decimal Result = default(decimal);
        public FunctionResult __Result()
        {
            var result = new FunctionResult();
            result["Result"] = Result;
            return result;
        }

        public void __Run(IExecutionContext context)
        {
            context.LogDebug("8:     Result = Input");
            var Results = 789 - -456;

            var a = BuiltInNumberFunctions.Int(8);
        }
    }
}