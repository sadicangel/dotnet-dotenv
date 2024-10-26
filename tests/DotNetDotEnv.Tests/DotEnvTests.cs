namespace DotNetDotEnv.Tests;

public class DotEnvTests
{
    [Fact]
    public void Load_from_file_creates_valid_DotEnv_instance()
    {
        var dotEnv = DotEnv.Load("Data/.env");

        Assert.Equal("Value1", dotEnv["Key1"]);
    }

    [Fact]
    public void Load_from_stream_creates_valid_DotEnv_instance()
    {
        using var stream = File.OpenRead("Data/.env");
        var dotEnv = DotEnv.Load(stream);

        Assert.Equal("Value1", dotEnv["Key1"]);
    }

    [Fact]
    public void LoadAndApply_from_file_creates_valid_DotEnv_instance_and_applies_to_environment()
    {
        var dotEnv = DotEnv.LoadAndApply("Data/.env");

        using var variables = EnvironmentVariableDisposer.Create(dotEnv);

        Assert.Equal("Value1", Environment.GetEnvironmentVariable("Key1"));
    }

    [Fact]
    public void LoadAndApply_from_stream_creates_valid_DotEnv_instance_and_applies_to_environment()
    {
        using var stream = File.OpenRead("Data/.env");
        var dotEnv = DotEnv.LoadAndApply(stream);

        using var variables = EnvironmentVariableDisposer.Create(dotEnv);

        Assert.Equal("Value1", Environment.GetEnvironmentVariable("Key1"));
    }

    [Fact]
    public void ApplyToEnvironment_applies_correctly()
    {
        var dotEnv = new DotEnv
        {
            ["Key"] = "Value"
        };
        dotEnv.ApplyToEnvironment();

        using var variables = EnvironmentVariableDisposer.Create(dotEnv);

        Assert.Equal("Value", Environment.GetEnvironmentVariable("Key"));
    }

    [Fact]
    public void ApplyToEnvironment_overwrites_existing_values()
    {
        using var variables = EnvironmentVariableDisposer.CreateAndApply([new KeyValuePair<string, string>("Key", "SomethingElse")]);
        var dotEnv = new DotEnv
        {
            ["Key"] = "Value"
        };
        dotEnv.ApplyToEnvironment();

        Assert.Equal("Value", Environment.GetEnvironmentVariable("Key"));
    }

    [Fact]
    public void RemoveFromEnvironment_removes_correctly()
    {
        var dotEnv = new DotEnv
        {
            ["Key"] = "Value"
        };
        dotEnv.ApplyToEnvironment();

        Assert.Equal("Value", Environment.GetEnvironmentVariable("Key"));

        dotEnv.RemoveFromEnvironment();

        Assert.Null(Environment.GetEnvironmentVariable("Key"));
    }

    [Fact]
    public void RemoveFromEnvironment_overwrites_existing_values()
    {
        Environment.SetEnvironmentVariable("Key", "Value");
        Assert.Equal("Value", Environment.GetEnvironmentVariable("Key"));

        var dotEnv = new DotEnv
        {
            ["Key"] = "Value"
        };

        dotEnv.RemoveFromEnvironment();

        Assert.Null(Environment.GetEnvironmentVariable("Key"));
    }
}
