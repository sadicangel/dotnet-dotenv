using Microsoft.Extensions.Configuration;

namespace DotNetDotEnv.AspNetCore;

/// <summary>
/// Provides configuration key-value pairs that are obtained from a .env file.
/// </summary>
/// <remarks>
/// Initializes a new instance with the specified source.
/// </remarks>
/// <param name="source">The source settings.</param>
public class DotEnvConfigurationProvider(DotEnvConfigurationSource source) : FileConfigurationProvider(source)
{
    /// <summary>
    /// Loads the .env data from a stream.
    /// </summary>
    /// <param name="stream">The stream to read.</param>
    public override void Load(Stream stream) => Data = DotEnv.Read(stream)!;
}
