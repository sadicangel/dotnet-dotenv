using System.Collections;

namespace DotNetDotEnv.Tests;

// This holds a subset of variables that will be cleared when this instance is disposed.
public sealed class TempEnvironment : IDisposable, IEnumerable<KeyValuePair<string, string?>>
{
    private readonly List<string> _keys = [];

    public TempEnvironment(IEnumerable<KeyValuePair<string, string>> variables)
    {
        _keys = [.. variables.Select(v => v.Key)];
        foreach (var (key, value) in variables)
            this[key] = value;
    }

    public IEnumerable<string> Keys => _keys;
    public IEnumerable<string?> Values => _keys.Select(Environment.GetEnvironmentVariable);

    public string? this[string key]
    {
        get => Environment.GetEnvironmentVariable(key);
        set
        {
            if (Environment.GetEnvironmentVariable(key) is null)
                Environment.SetEnvironmentVariable(key, value);
        }
    }

    public IEnumerator<KeyValuePair<string, string?>> GetEnumerator()
    {
        foreach (var key in _keys)
            yield return new KeyValuePair<string, string?>(key, this[key]);
    }
    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    public void Dispose()
    {
        if (_keys.Count > 0)
        {
            foreach (var key in _keys)
                Environment.SetEnvironmentVariable(key, null);
            _keys.Clear();
        }
    }
}
