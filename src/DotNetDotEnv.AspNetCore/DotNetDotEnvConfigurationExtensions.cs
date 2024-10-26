using DotNetDotEnv.AspNetCore;
using Microsoft.Extensions.FileProviders;

#pragma warning disable IDE0130 // Namespace does not match folder structure
namespace Microsoft.Extensions.Configuration;
#pragma warning restore IDE0130 // Namespace does not match folder structure

public static class DotNetDotEnvConfigurationExtensions
{
    /// <summary>
    /// Adds the .env configuration provider at <paramref name="path"/> to <paramref name="builder"/>.
    /// </summary>
    /// <param name="builder">The <see cref="IConfigurationBuilder"/> to add to.</param>
    /// <param name="path">Path relative to the base path stored in
    /// <see cref="IConfigurationBuilder.Properties"/> of <paramref name="builder"/>.</param>
    /// <returns>The <see cref="IConfigurationBuilder"/>.</returns>
    public static IConfigurationBuilder AddDotEnvFile(this IConfigurationBuilder builder, string path = ".env") =>
        AddDotEnvFile(builder, provider: null, path: path, optional: false, reloadOnChange: false);

    /// <summary>
    /// Adds the .env configuration provider at <paramref name="path"/> to <paramref name="builder"/>.
    /// </summary>
    /// <param name="builder">The <see cref="IConfigurationBuilder"/> to add to.</param>
    /// <param name="path">Path relative to the base path stored in
    /// <see cref="IConfigurationBuilder.Properties"/> of <paramref name="builder"/>.</param>
    /// <param name="optional">Whether the file is optional.</param>
    /// <returns>The <see cref="IConfigurationBuilder"/>.</returns>
    public static IConfigurationBuilder AddDotEnvFile(this IConfigurationBuilder builder, string path, bool optional) =>
        AddDotEnvFile(builder, provider: null, path: path, optional: optional, reloadOnChange: false);

    /// <summary>
    /// Adds the .env configuration provider at <paramref name="path"/> to <paramref name="builder"/>.
    /// </summary>
    /// <param name="builder">The <see cref="IConfigurationBuilder"/> to add to.</param>
    /// <param name="path">Path relative to the base path stored in
    /// <see cref="IConfigurationBuilder.Properties"/> of <paramref name="builder"/>.</param>
    /// <param name="optional">Whether the file is optional.</param>
    /// <param name="reloadOnChange">Whether the configuration should be reloaded if the file changes.</param>
    /// <returns>The <see cref="IConfigurationBuilder"/>.</returns>
    public static IConfigurationBuilder AddDotEnvFile(this IConfigurationBuilder builder, string path, bool optional, bool reloadOnChange) =>
        AddDotEnvFile(builder, provider: null, path: path, optional: optional, reloadOnChange: reloadOnChange);

    /// <summary>
    /// Adds a .env configuration source to <paramref name="builder"/>.
    /// </summary>
    /// <param name="builder">The <see cref="IConfigurationBuilder"/> to add to.</param>
    /// <param name="provider">The <see cref="IFileProvider"/> to use to access the file.</param>
    /// <param name="path">Path relative to the base path stored in
    /// <see cref="IConfigurationBuilder.Properties"/> of <paramref name="builder"/>.</param>
    /// <param name="optional">Whether the file is optional.</param>
    /// <param name="reloadOnChange">Whether the configuration should be reloaded if the file changes.</param>
    /// <returns>The <see cref="IConfigurationBuilder"/>.</returns>
    public static IConfigurationBuilder AddDotEnvFile(this IConfigurationBuilder builder, IFileProvider? provider, string path, bool optional, bool reloadOnChange)
    {
        ArgumentNullException.ThrowIfNull(builder);
        ArgumentException.ThrowIfNullOrEmpty(path);

        return builder.AddDotEnvFile(s =>
        {
            s.FileProvider = provider;
            s.Path = path;
            s.Optional = optional;
            s.ReloadOnChange = reloadOnChange;
            s.ResolveFileProvider();
        });
    }

    /// <summary>
    /// Adds a .env configuration source to <paramref name="builder"/>.
    /// </summary>
    /// <param name="builder">The <see cref="IConfigurationBuilder"/> to add to.</param>
    /// <param name="configureSource">Configures the source.</param>
    /// <returns>The <see cref="IConfigurationBuilder"/>.</returns>
    public static IConfigurationBuilder AddDotEnvFile(this IConfigurationBuilder builder, Action<DotEnvConfigurationSource>? configureSource)
        => builder.Add(configureSource);

    /// <summary>
    /// Adds a .env configuration source to <paramref name="builder"/>.
    /// </summary>
    /// <param name="builder">The <see cref="IConfigurationBuilder"/> to add to.</param>
    /// <param name="stream">The <see cref="Stream"/> to read the json configuration data from.</param>
    /// <returns>The <see cref="IConfigurationBuilder"/>.</returns>
    public static IConfigurationBuilder AddDotEnvStream(this IConfigurationBuilder builder, Stream stream)
    {
        ArgumentNullException.ThrowIfNull(builder);

        return builder.Add<DotEnvStreamConfigurationSource>(s => s.Stream = stream);
    }
}
