using System;

namespace Lexy.Poc.Parser.ExpressionParser
{
    public static class ExpressionTestExtensions
    {
        public static void ValidateOfType<T>(this object value, Action<T> validate) where T : class
        {
            if (value == null) throw new ArgumentNullException(nameof(value));

            var specificValue = value as T;
            if (specificValue == null)
            {
                throw new InvalidOperationException(
                    $"Values '{value.GetType().Name}' should be of type ‘{typeof(T).Name}'");
            }

            validate(specificValue);
        }

    }
}