using System.Text;

namespace DotNetDotEnv;

internal static class Parser
{
    public static Dictionary<string, string> Parse(ReadOnlySpan<char> input)
    {
        var ignoreInvalidLines = true;

        var stringBuilder = default(StringBuilder);
        StringBuilder GetStringBuilder() => (stringBuilder ??= new StringBuilder()).Clear();

        var keyValuePairs = new Dictionary<string, string>();
        Span<Range> ranges = stackalloc Range[2];
        var enumerator = input.EnumerateLines();
        while (enumerator.MoveNext())
        {
            var line = enumerator.Current;

            if (line.IsWhiteSpace())
            {
                continue;
            }

            if (line.TrimStart().StartsWith("#", StringComparison.Ordinal))
            {
                continue;
            }

            if (line.Split(ranges, '=', StringSplitOptions.TrimEntries) is not 2)
            {
                if (ignoreInvalidLines) continue;
                throw new FormatException($"Invalid key-value format '{line}'");
            }

            var keySpan = line[ranges[0]];
            if (!Variable.IsValidKey(keySpan))
            {
                if (ignoreInvalidLines) continue;
                Variable.ThrowIfInvalidKey(keySpan);
            }

            var canInterpolateValue = true;
            var valueSpan = line[ranges[1]];
            switch (valueSpan)
            {
                case ['"', ..]:
                    {
                        valueSpan = valueSpan[1..];
                        var i = valueSpan.IndexOf("\"", StringComparison.Ordinal);
                        if (i >= 0)
                        {
                            valueSpan = valueSpan[..i];
                        }
                        else
                        {
                            var endFound = false;
                            var builder = GetStringBuilder().Append(valueSpan);
                            while (enumerator.MoveNext())
                            {
                                builder.Append(Environment.NewLine);
                                var l = enumerator.Current;
                                var k = l.IndexOf("\"", StringComparison.Ordinal);
                                if (k >= 0)
                                {
                                    builder.Append(l[..k]);
                                    endFound = true;
                                    break;
                                }
                                else
                                {
                                    builder.Append(l);
                                }
                            }
                            if (!endFound && !ignoreInvalidLines)
                            {
                                throw new FormatException($"Invalid value format '{valueSpan}'");
                            }

                            valueSpan = builder.ToString();
                        }
                    }
                    break;

                case ['\'', ..]:
                    {
                        canInterpolateValue = false;
                        valueSpan = valueSpan[1..];
                        var i = valueSpan.IndexOf("'", StringComparison.Ordinal);
                        if (i < 0)
                        {
                            if (ignoreInvalidLines) continue;
                            throw new FormatException($"Invalid value format '{valueSpan}'");
                        }
                        valueSpan = valueSpan[..i];
                    }
                    break;

                default:
                    {
                        var i = valueSpan.IndexOf(" #", StringComparison.Ordinal);
                        if (i > 0)
                        {
                            valueSpan = valueSpan[..i];
                        }
                    }
                    break;
            }

            var key = keySpan.ToString();
            var value = valueSpan.ToString();

            if (canInterpolateValue)
            {
                if (value.Contains('$', StringComparison.Ordinal))
                {
                    // Replace braced ${VAR}
                    if (value.Contains("${", StringComparison.Ordinal))
                    {
                        value = Variable.GetOrCreateInterpolatedCaptureGroups().Replace(value, match =>
                        {
                            var variable = match.Groups["var"].Value;
                            var defaultIfEmpty = match.Groups["defaultColon"].Value;
                            var defaultIfNull = match.Groups["defaultDash"].Value;

                            var result = keyValuePairs.GetValueOrDefault(variable)
                                ?? Environment.GetEnvironmentVariable(variable);
                            return result switch
                            {
                                "" or null when !string.IsNullOrEmpty(defaultIfEmpty) => defaultIfEmpty,
                                null when !string.IsNullOrEmpty(defaultIfNull) => defaultIfNull,
                                _ => result ?? ""
                            };
                        });
                    }

                    // Replace unbraced $VAR
                    var i = value.IndexOf('$', StringComparison.Ordinal);
                    if (i >= 0)
                    {
                        var variable = value[(i + 1)..].ToString();
                        value = $"{value[..i]}{keyValuePairs.GetValueOrDefault(variable)
                            ?? Environment.GetEnvironmentVariable(variable)}";
                    }
                }
            }

            keyValuePairs[key] = value;
        }

        return keyValuePairs;
    }
}
