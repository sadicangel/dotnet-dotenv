using Microsoft.Extensions.Configuration;

namespace DotNetDotEnv.AspNetCore;

/// <summary>
/// Provides configuration key-value pairs that are obtained from a .env stream.
/// </summary>
public sealed class DotEnvStreamConfigurationProvider(DotEnvStreamConfigurationSource source)
    : StreamConfigurationProvider(source)
{
    /// <summary>
    /// Loads .env configuration key-value pairs from a stream into a provider.
    /// </summary>
    /// <param name="stream">The .env <see cref="Stream"/> to load configuration data from.</param>
    public override void Load(Stream stream) => Data = DotEnv.Read(stream)!;
}
