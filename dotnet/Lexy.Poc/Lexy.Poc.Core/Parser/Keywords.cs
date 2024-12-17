using System.Collections.Generic;

namespace Lexy.Poc.Core.Parser
{
    internal static class Keywords
    {
        private static readonly IList<string> values = new List<string>
        {
            TokenNames.FunctionComponent,
            TokenNames.EnumComponent,
            TokenNames.TableComponent,
            TokenNames.ScenarioComponent,

            TokenNames.Function,
            TokenNames.Table,

            TokenNames.Include,
            TokenNames.Parameters,
            TokenNames.Results,
            TokenNames.Code,
            TokenNames.ExpectError,

            TokenNames.Comment,
        };

        public static bool Contains(string keyword) => values.Contains(keyword);
    }
}