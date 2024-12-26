namespace Lexy.RunTime.RunTime
{
    public interface IExecutionContext
    {
        void LogDebug(string message);

        void LogVariable<T>(string name, T value);
    }
}