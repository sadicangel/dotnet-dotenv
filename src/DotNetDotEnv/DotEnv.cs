using System.Collections;
using System.Diagnostics.CodeAnalysis;

namespace DotNetDotEnv;

/// <summary>
/// Represents a read-only dictionary of <c>.env</c> variables.
/// This class provides methods to read and write <c>.env</c> files and
/// ensures variable keys follow required conventions.
/// </summary>
public sealed record class DotEnv : IReadOnlyDictionary<string, string>, IDictionary<string, string>
{
    private readonly Dictionary<string, string> _values;

    /// <summary>
    /// Initializes a new instance of the <see cref="DotEnv"/> class that is empty.
    /// </summary>
    public DotEnv() => _values = [];

    /// <summary>
    /// Initializes a new instance of the <see cref="DotEnv"/> class with a specified initial capacity.
    /// </summary>
    /// <param name="capacity">The initial capacity for the dictionary.</param>
    public DotEnv(int capacity) => _values = new(capacity);

    /// <summary>
    /// Initializes a new instance of the <see cref="DotEnv"/> class and populates it with
    /// the specified collection of key-value pairs.
    /// </summary>
    /// <param name="collection">The collection of variable key-value pairs.</param>
    /// <exception cref="ArgumentException">Thrown when a key is invalid.</exception>
    public DotEnv(IEnumerable<KeyValuePair<string, string>> collection)
    {
        _values = new(collection);
        foreach (var (key, _) in _values)
            Variable.ThrowIfInvalidKey(key);
    }

    /// <summary>
    /// Gets the variable value associated with the specified key.
    /// </summary>
    /// <param name="key">The key of the environment variable to get.</param>
    /// <returns>The value associated with the specified key.</returns>
    /// <exception cref="KeyNotFoundException">Thrown when the key is not found.</exception>
    public string this[string key]
    {
        get => _values[key];
        init
        {
            Variable.ThrowIfInvalidKey(key);
            _values[key] = value;
        }
    }

    /// <summary>
    /// Gets the number of key-value pairs contained in the <see cref="DotEnv"/> instance.
    /// </summary>
    public int Count => _values.Count;

    /// <summary>
    /// Gets an enumerable collection of all variable keys in the <see cref="DotEnv"/> instance.
    /// </summary>
    public IEnumerable<string> Keys => _values.Keys;

    /// <summary>
    /// Gets an enumerable collection of all variable values in the <see cref="DotEnv"/> instance.
    /// </summary>
    public IEnumerable<string> Values => _values.Values;

    /// <summary>
    /// Determines whether the <see cref="DotEnv"/> instance contains the specified key.
    /// </summary>
    /// <param name="key">The key to locate.</param>
    /// <returns><see langword="true"/> if the <see cref="DotEnv"/> contains the specified key; otherwise, <see langword="false"/>.</returns>
    public bool ContainsKey(string key) => _values.ContainsKey(key);

    /// <summary>
    /// Returns an enumerator that iterates through the key-value pairs in the <see cref="DotEnv"/>.
    /// </summary>
    /// <returns>An enumerator for the key-value pairs in the dictionary.</returns>
    public IEnumerator<KeyValuePair<string, string>> GetEnumerator() => _values.GetEnumerator();
    IEnumerator IEnumerable.GetEnumerator() => ((IEnumerable)_values).GetEnumerator();

    /// <summary>
    /// Gets the value associated with the specified key.
    /// </summary>
    /// <param name="key">The key of the value to get.</param>
    /// <param name="value">When this method returns, contains the value associated with the specified key,
    /// if the key is found; otherwise, <see langword="null"/>.</param>
    /// <returns><see langword="true"/> if the key was found; otherwise, <see langword="false"/>.</returns>
    public bool TryGetValue(string key, [MaybeNullWhen(false)] out string value) => _values.TryGetValue(key, out value);

    /// <summary>
    /// Returns a string representation of the environment variables in <c>.env</c> format.
    /// </summary>
    /// <returns>A string representing all key-value pairs in the <c>.env</c> format.</returns>
    public override string ToString() => string.Join(Environment.NewLine, _values.Select(e => $"{e.Key}=\"{e.Value}\""));

    /// <summary>
    /// Loads environment variables from the specified file and returns a new <see cref="DotEnv"/> instance.
    /// </summary>
    /// <param name="fileName">The path to the file to load environment variables from.</param>
    /// <returns>A <see cref="DotEnv"/> instance populated with the variables from the file.</returns>
    public static DotEnv Load(string fileName)
    {
        var variables = Parser.Parse(File.ReadAllText(fileName));
        return new DotEnv(variables);
    }

    /// <summary>
    /// Loads environment variables from the specified stream and returns a new <see cref="DotEnv"/> instance.
    /// </summary>
    /// <returns>A <see cref="DotEnv"/> instance populated with the variables from the stream.</returns>
    public static DotEnv Load(Stream stream)
    {
        using var reader = new StreamReader(stream);
        var variables = Parser.Parse(reader.ReadToEnd());
        return new DotEnv(variables);
    }

    /// <summary>
    /// Loads environment variables from the specified file, sets them in the current
    /// process's environment variables and returns a new <see cref="DotEnv"/> instance.
    /// </summary>
    /// <param name="fileName">The path to the file to load environment variables from.</param>
    /// <returns>A <see cref="DotEnv"/> instance populated with the variables from the file.</returns>
    public static DotEnv LoadAndApply(string fileName)
    {
        var dotEnv = Load(fileName);
        dotEnv.ApplyToEnvironment();
        return dotEnv;
    }

    /// <summary>
    /// Loads environment variables from the specified stream, sets them in the current
    /// process's environment variables and returns a new <see cref="DotEnv"/> instance.
    /// </summary>
    /// <param name="stream">The stream to load environment variables from.</param>
    /// <returns>A <see cref="DotEnv"/> instance populated with the variables from the stream.</returns>
    public static DotEnv LoadAndApply(Stream stream)
    {
        var dotEnv = Load(stream);
        dotEnv.ApplyToEnvironment();
        return dotEnv;
    }

    /// <summary>
    /// Saves the variables to the specified file in <c>.env</c> format.
    /// </summary>
    /// <param name="fileName">The path to the file where the variables will be saved.</param>
    public void Save(string fileName) => File.WriteAllText(fileName, ToString());

    /// <summary>
    /// Sets the current key-value pairs in the <see cref="DotEnv"/> instance as environment
    /// variables in the current process.
    /// </summary>
    public void ApplyToEnvironment()
    {
        foreach (var (key, value) in _values)
        {
            Environment.SetEnvironmentVariable(key, value);
        }
    }

    /// <summary>
    /// Removes the current key-value pairs in the <see cref="DotEnv"/> instance from the process's environment variables.
    /// </summary>
    public void RemoveFromEnvironment()
    {
        foreach (var key in _values.Keys)
        {
            Environment.SetEnvironmentVariable(key, null);
        }
    }

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
