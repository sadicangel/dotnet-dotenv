using System.Collections;
using System.Diagnostics.CodeAnalysis;

namespace DotNetDotEnv;

public sealed record class DotEnv : IReadOnlyDictionary<string, string>
{
    private readonly Dictionary<string, string> _values;

    public DotEnv() => _values = [];

    public DotEnv(int capacity) => _values = new(capacity);

    public DotEnv(IEnumerable<KeyValuePair<string, string>> collection) => _values = new(collection);

    public string this[string key]
    {
        get => _values[key];
        init
        {
            Variable.ThrowIfInvalidKey(value);
            _values[key] = value;
        }
    }

    IEnumerable<string> IReadOnlyDictionary<string, string>.Keys => _values.Keys;
    IEnumerable<string> IReadOnlyDictionary<string, string>.Values => _values.Values;

    public int Count => _values.Count;

    public bool ContainsKey(string key) => _values.ContainsKey(key);

    public IEnumerator<KeyValuePair<string, string>> GetEnumerator() => _values.GetEnumerator();
    IEnumerator IEnumerable.GetEnumerator() => ((IEnumerable)_values).GetEnumerator();

    public bool TryGetValue(string key, [MaybeNullWhen(false)] out string value) => _values.TryGetValue(key, out value);

    public override string ToString() => string.Join(Environment.NewLine, _values.Select(e => $"{e.Key}=\"{e.Value}\""));

    public static DotEnv Load(string fileName) => Parser.Parse(File.ReadAllText(fileName));

    public void Save(string fileName) => File.WriteAllText(fileName, ToString());
}
