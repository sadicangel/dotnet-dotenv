using System.Text.RegularExpressions;

namespace DotNetDotEnv;

internal static partial class Variable
{
    [GeneratedRegex(@"^[a-zA-Z_]+[a-zA-Z0-9_]*$")]
    public static partial Regex GetOrCreateVariableNameRegex();

    [GeneratedRegex(@"\$\{(?<var>[a-zA-Z_]+[a-zA-Z0-9_]*)(?::-(?<defaultColon>.*?))?(?:-(?<defaultDash>.*?))?\}")]
    public static partial Regex GetOrCreateInterpolatedCaptureGroups();


    public static bool IsValidKey(ReadOnlySpan<char> value) =>
        GetOrCreateVariableNameRegex().IsMatch(value);

    public static void ThrowIfInvalidKey(ReadOnlySpan<char> value)
    {
        var regex = GetOrCreateVariableNameRegex();

        if (!regex.IsMatch(value))
        {
            throw new ArgumentOutOfRangeException(
                nameof(value),
                value.ToString(),
                $"Invalid variable name. Must match the following regex pattern: {regex}");
        }
    }
}
