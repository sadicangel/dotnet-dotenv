using Microsoft.Extensions.Configuration;

namespace DotNetDotEnv.AspNetCore;

/// <summary>
/// Represents a .env file as an <see cref="IConfigurationSource"/>.
/// </summary>
public sealed class DotEnvStreamConfigurationSource : StreamConfigurationSource
{
    /// <summary>
    /// Builds the <see cref="JsonStreamConfigurationProvider"/> for this source.
    /// </summary>
    /// <param name="builder">The <see cref="IConfigurationBuilder"/>.</param>
    /// <returns>An <see cref="JsonStreamConfigurationProvider"/></returns>
    public override IConfigurationProvider Build(IConfigurationBuilder builder) =>
        new DotEnvStreamConfigurationProvider(this);
}
