using System;
using System.Collections.Generic;
using Lexy.RunTime;

namespace Lexy.Runtime
{
    public enum A
    {
        B,
        C
    }

    public class Table__SimpleTable
    {
        public class __Row
        {
            public decimal Search = default(decimal);
            public decimal Value = default(decimal);
        }

        private static List<__Row> __values;
        static Table__SimpleTable()
        {
            A A = A.B;
            if (A == A.B)
            {

            }

            __values = new List<__Row>
            {
                new __Row
                {
                    Search = 0m,
                    Value = 0m
                },
                new __Row
                {
                    Search = 1m,
                    Value = 1m
                }
            };
        }

        public static int Count => __values.Count;
        public static IReadOnlyList<__Row> Values => __values;
    }

    public static class Function__ValidateTableKeywordFunction
    {
        public class __Parameters
        {
        }

        public class __Result
        {
            public decimal Result = default(decimal);
        }

        public static __Result __Run(__Parameters __parameters, IExecutionContext __context)
        {
            if (__parameters == null)
                throw new ArgumentNullException(nameof(__parameters));
            if (__context == null)
                throw new ArgumentNullException(nameof(__context));
            var __result = new __Result();
            __context.SetFileName("file.ex");
            __context.OpenScope("scope", 666);
            var __logLine5 = __context.LogLine("message", 123, LogVariablesBuilder.New()
                .AddVariable("abc", __result.Result)
                .AddVariable("def", __result.Result)
                .Build());
            __result.Result = __LookUpSimpleTableValueBySearch(2m, __context);
            __logLine5.AddWriteVariables(LogVariablesBuilder.New()
                .AddVariable("abc", __result.Result)
                .AddVariable("def", __result.Result)
                .Build());
            __context.UseLastNodeAsScope();
            __context.RevertToParentScope();
            __context.CloseScope();
            return __result;
        }

        private static decimal __LookUpSimpleTableValueBySearch(decimal condition, IExecutionContext __context)
        {
            return BuiltInTableFunctions.LookUp("Value", "Search", "SimpleTable", Table__SimpleTable.Values, condition, row => row.Search, row => row.Value, __context);
        }
    }
}