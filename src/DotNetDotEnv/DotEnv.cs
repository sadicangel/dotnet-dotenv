using System.Collections;
using System.Diagnostics.CodeAnalysis;
using System.Text.RegularExpressions;

namespace DotNetDotEnv;

public sealed record class DotEnv : IReadOnlyDictionary<string, string>
{
    private readonly Dictionary<string, string> _values;

    public DotEnv() => _values = [];

    public string this[string key] { get => _values[key]; init => _values[key] = VariableName.ThrowIfInvalid(value); }

    IEnumerable<string> IReadOnlyDictionary<string, string>.Keys => _values.Keys;
    IEnumerable<string> IReadOnlyDictionary<string, string>.Values => _values.Values;

    public int Count => _values.Count;

    public bool ContainsKey(string key) => _values.ContainsKey(key);

    public IEnumerator<KeyValuePair<string, string>> GetEnumerator() => _values.GetEnumerator();
    IEnumerator IEnumerable.GetEnumerator() => ((IEnumerable)_values).GetEnumerator();

    public bool TryGetValue(string key, [MaybeNullWhen(false)] out string value) => _values.TryGetValue(key, out value);
}

internal static partial class VariableName
{
    [GeneratedRegex(@"^[a-zA-Z_]+[a-zA-Z0-9_]*$")]
    private static partial Regex GetOrCreateVariableNameRegex();

    public static string ThrowIfInvalid(string value)
    {
        var regex = GetOrCreateVariableNameRegex();
        if (!regex.IsMatch(value))
        {
            throw new ArgumentOutOfRangeException(
                nameof(value),
                value,
                $"Invalid variable name. Must match the following regex pattern: {regex}");
        }

        return value;
    }
}
