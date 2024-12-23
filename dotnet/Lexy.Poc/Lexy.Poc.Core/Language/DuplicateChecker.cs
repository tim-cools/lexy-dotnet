using System;
using System.Collections.Generic;
using Lexy.Poc.Core.Parser;

namespace Lexy.Poc.Core.Language
{
    public static class DuplicateChecker
    {
        public static void Validate<T>(IValidationContext context, Func<T, string> getName, Func<T, string> getErrorMessage, params IList<T>[] lists)
        {
            if (context == null) throw new ArgumentNullException(nameof(context));
            if (getName == null) throw new ArgumentNullException(nameof(getName));
            if (getErrorMessage == null) throw new ArgumentNullException(nameof(getErrorMessage));
            if (lists == null) throw new ArgumentNullException(nameof(lists));

            var found = new List<string>();
            foreach (var list in lists)
            {
                foreach (var item in list)
                {
                    var name = getName(item);
                    if (found.Contains(name))
                    {
                        context.Logger.Fail(getErrorMessage(item));
                    }
                    else
                    {
                        found.Add(name);
                    }
                }
            }
        }
    }
}