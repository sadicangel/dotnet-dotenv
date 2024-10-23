using System.Collections;
using System.Diagnostics.CodeAnalysis;

namespace DotNetDotEnv;

public sealed record class DotEnv : IReadOnlyDictionary<string, string>, IDictionary<string, string>
{
    private readonly Dictionary<string, string> _values;

    public DotEnv() => _values = [];

    public DotEnv(int capacity) => _values = new(capacity);

    public DotEnv(IEnumerable<KeyValuePair<string, string>> collection)
    {
        _values = new(collection);
        foreach (var (key, _) in _values)
            Variable.ThrowIfInvalidKey(key);
    }

    public string this[string key]
    {
        get => _values[key];
        init
        {
            Variable.ThrowIfInvalidKey(key);
            _values[key] = value;
        }
    }

    public int Count => _values.Count;

    public bool ContainsKey(string key) => _values.ContainsKey(key);

    public IEnumerator<KeyValuePair<string, string>> GetEnumerator() => _values.GetEnumerator();
    IEnumerator IEnumerable.GetEnumerator() => ((IEnumerable)_values).GetEnumerator();

    public bool TryGetValue(string key, [MaybeNullWhen(false)] out string value) => _values.TryGetValue(key, out value);

    public override string ToString() => string.Join(Environment.NewLine, _values.Select(e => $"{e.Key}=\"{e.Value}\""));

    public static DotEnv Read(string fileName)
    {
        var variables = Parser.Parse(File.ReadAllText(fileName));
        return new DotEnv(variables);
    }

    public static DotEnv Read(Stream stream)
    {
        using var reader = new StreamReader(stream);
        var variables = Parser.Parse(reader.ReadToEnd());
        return new DotEnv(variables);
    }

    public void Save(string fileName) => File.WriteAllText(fileName, ToString());

    IEnumerable<string> IReadOnlyDictionary<string, string>.Keys => _values.Keys;
    IEnumerable<string> IReadOnlyDictionary<string, string>.Values => _values.Values;
    ICollection<string> IDictionary<string, string>.Keys => _values.Keys;
    ICollection<string> IDictionary<string, string>.Values => _values.Values;
    bool ICollection<KeyValuePair<string, string>>.IsReadOnly => true;
    string IDictionary<string, string>.this[string key] { get => _values[key]; set => throw new NotSupportedException("Read-only collection"); }
    void ICollection<KeyValuePair<string, string>>.CopyTo(KeyValuePair<string, string>[] array, int arrayIndex) =>
        ((ICollection<KeyValuePair<string, string>>)_values).CopyTo(array, arrayIndex);
    void IDictionary<string, string>.Add(string key, string value) =>
        throw new NotSupportedException("Read-only collection");
    bool IDictionary<string, string>.Remove(string key) =>
        throw new NotSupportedException("Read-only collection");
    void ICollection<KeyValuePair<string, string>>.Add(KeyValuePair<string, string> item) =>
        throw new NotSupportedException("Read-only collection");
    void ICollection<KeyValuePair<string, string>>.Clear() =>
        throw new NotSupportedException("Read-only collection");
    bool ICollection<KeyValuePair<string, string>>.Contains(KeyValuePair<string, string> item) =>
        throw new NotSupportedException("Read-only collection");
    bool ICollection<KeyValuePair<string, string>>.Remove(KeyValuePair<string, string> item) =>
        throw new NotSupportedException("Read-only collection");
}
