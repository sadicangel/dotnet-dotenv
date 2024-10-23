using Microsoft.Extensions.Hosting;

namespace DotNetDotEnv.AspNetCore.Tests;

public sealed class SharedFixture : IAsyncLifetime
{
    public async Task<IHost> CreateHostAsync(Func<IHostBuilder, Task>? configureAsync = null)
    {
        var builder = Host.CreateDefaultBuilder();
        if (configureAsync is not null)
            await configureAsync.Invoke(builder);
        var host = builder.Build();
        await host.StartAsync();
        return host;
    }

    public Task InitializeAsync() => throw new NotImplementedException();
    public Task DisposeAsync() => throw new NotImplementedException();
}

public sealed class TestCollection : ICollectionFixture<SharedFixture>;
