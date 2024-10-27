using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace DotNetDotEnv.AspNetCore.Tests;

public class DotNetDotEnvConfigurationExtensionsTests
{
    [Fact]
    public async Task AddDotEnvFile_loads_variables_from_default_path()
    {
        using var dotEnv = new TempFile(
            fileName: ".env",
            content: """
            Key=Value
            """);

        var builder = Host.CreateEmptyApplicationBuilder(default);
        builder.Configuration.AddDotEnvFile();
        using var host = builder.Build();
        await host.StartAsync();

        var value = host.Services.GetRequiredService<IConfiguration>()["Key"];
        Assert.Equal("Value", value);
    }

    [Fact]
    public async Task AddDotEnvFile_loads_variables_from_specified_path()
    {
        var fileName = "another_path";
        using var dotEnv = new TempFile(
            fileName,
            content: """
            Key=Value
            """);

        var builder = Host.CreateEmptyApplicationBuilder(default);
        builder.Configuration.AddDotEnvFile(fileName);
        using var host = builder.Build();
        await host.StartAsync();

        var value = host.Services.GetRequiredService<IConfiguration>()["Key"];
        Assert.Equal("Value", value);
    }

    [Fact]
    public void AddDotEnvFile_throws_if_env_does_exist_and_is_not_optional()
    {
        var builder = Host.CreateEmptyApplicationBuilder(default);
        Assert.Throws<FileNotFoundException>(() => builder.Configuration.AddDotEnvFile());
    }

    [Fact]
    public void AddDotEnvFile_does_not_throw_if_env_does_exist_and_is_optional()
    {
        var builder = Host.CreateEmptyApplicationBuilder(default);
        Assert.Null(Record.Exception(() => builder.Configuration.AddDotEnvFile(path: ".env", optional: true)));
    }
}
