using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace DotNetDotEnv.AspNetCore.Tests;

public class DotNetDotEnvConfigurationExtensionsTests
{
    [Fact]
    public async Task Load_DotEnv_variables_from_default_path()
    {
        using var dotEnv = new TempDotEnvFile(fileName: ".env");

        var builder = Host.CreateEmptyApplicationBuilder(default);
        builder.Configuration.AddDotEnvFile();
        using var host = builder.Build();
        await host.StartAsync();

        var value = host.Services.GetRequiredService<IConfiguration>()["Key"];
        Assert.Equal("Value", value);
    }

    [Fact]
    public async Task Load_DotEnv_variables_from_specified_path()
    {
        var fileName = "another_path";
        using var dotEnv = new TempDotEnvFile(fileName);

        var builder = Host.CreateEmptyApplicationBuilder(default);
        builder.Configuration.AddDotEnvFile(fileName);
        using var host = builder.Build();
        await host.StartAsync();

        var value = host.Services.GetRequiredService<IConfiguration>()["Key"];
        Assert.Equal("Value", value);
    }

    [Fact]
    public void Throw_if_env_does_exist_and_is_not_optional()
    {
        var builder = Host.CreateEmptyApplicationBuilder(default);
        Assert.Throws<FileNotFoundException>(() => builder.Configuration.AddDotEnvFile());
    }

    [Fact]
    public void Does_not_throw_if_env_does_exist_and_is_optional()
    {
        var builder = Host.CreateEmptyApplicationBuilder(default);
        Assert.Null(Record.Exception(() => builder.Configuration.AddDotEnvFile(path: ".env", optional: true)));
    }
}

file readonly struct TempDotEnvFile : IDisposable
{
    public string FileName { get; }

    public DotEnv DotEnv { get; }

    public TempDotEnvFile(string? fileName)
    {
        FileName = fileName ?? ".env";
        DotEnv = new DotEnv(new Dictionary<string, string> { ["Key"] = "Value" });
        DotEnv.Save(FileName);
    }

    public void Dispose() => File.Delete(FileName);
}
