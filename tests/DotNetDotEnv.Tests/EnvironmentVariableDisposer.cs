namespace DotNetDotEnv.Tests;

// Helper class that deletes environment variables and the end of scope.
internal readonly struct EnvironmentVariableDisposer : IDisposable
{
    private readonly Dictionary<string, string> _variables;

    private EnvironmentVariableDisposer(IEnumerable<KeyValuePair<string, string>> variables) => _variables = new(variables);

    public static EnvironmentVariableDisposer Create(IEnumerable<KeyValuePair<string, string>> variables) => new(variables);

    public static EnvironmentVariableDisposer CreateAndApply(IEnumerable<KeyValuePair<string, string>> variables)
    {
        foreach (var (key, value) in variables)
            Environment.SetEnvironmentVariable(key, value);
        return new(variables);
    }

    public void Dispose()
    {
        foreach (var key in _variables.Keys)
            Environment.SetEnvironmentVariable(key, null);
        _variables.Clear();
    }
}
