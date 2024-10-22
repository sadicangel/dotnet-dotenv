namespace DotNetDotEnv.Tests;

public partial class VariableNameTests
{
    [Theory]
    [MemberData(nameof(ValidVariableNames))]
    public void Allow_when_value_is(string value) =>
        Assert.Equal(value, VariableName.ThrowIfInvalid(value));

    [Theory]
    [MemberData(nameof(InvalidVariableNames))]
    public void Throw_when_value_is(string value) =>
        Assert.Throws<ArgumentOutOfRangeException>(() => VariableName.ThrowIfInvalid(value));

    public static TheoryData<string> ValidVariableNames() => [
        "DATABASE",
        "APP_NAME",
        "_NAME",
        "RELEASE2024",
        "AppConfig",
        "appSettings"
    ];

    public static TheoryData<string> InvalidVariableNames() => [
        "",
        "app-name",
        "1INVALID_KEY",
        "USER@NAME",
        "PASSWORD#123",
        "APP_NAME ",
        "  APP_NAME",
    ];
}
