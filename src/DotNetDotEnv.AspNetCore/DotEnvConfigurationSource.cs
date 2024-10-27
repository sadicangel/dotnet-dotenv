using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.FileProviders.Physical;

namespace DotNetDotEnv.AspNetCore;

/// <summary>
/// Represents a .env file as an <see cref="IConfigurationSource"/>.
/// </summary>
public class DotEnvConfigurationSource : FileConfigurationSource
{
    /// <summary>
    /// Builds the <see cref="DotEnvConfigurationProvider"/> for this source.
    /// </summary>
    /// <param name="builder">The <see cref="IConfigurationBuilder"/>.</param>
    /// <returns>A <see cref="DotEnvConfigurationProvider"/> instance.</returns>
    public override IConfigurationProvider Build(IConfigurationBuilder builder)
    {
        // Need a new file provider that loads dot prefixed and hidden files.
        FileProvider = new PhysicalFileProvider(
            root: AppContext.BaseDirectory ?? string.Empty,
            filters: ExclusionFilters.System);
        OnLoadException ??= builder.GetFileLoadExceptionHandler();

        return new DotEnvConfigurationProvider(this);
    }
}
