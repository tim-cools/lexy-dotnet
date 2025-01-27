using System.Collections;
using System.Collections.Generic;

namespace Lexy.RunTime;

public class LogVariables : IEnumerable<KeyValuePair<string, LogVariable>>
{
    public IDictionary<string, LogVariable> Variables { get; }

    public LogVariables(IDictionary<string, LogVariable> variables = null)
    {
        Variables = variables ?? new Dictionary<string, LogVariable>();
    }

    public LogVariable this[string name] => Variables.TryGetValue(name, out var value) ? value : null;

    public bool Contains(string name) => Variables.ContainsKey(name);

    public IEnumerator<KeyValuePair<string, LogVariable>> GetEnumerator()
    {
        return Variables.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return Variables.GetEnumerator();
    }
}