namespace DotNetDotEnv.Tests;

public partial class VariableTests
{
    [Theory]
    [MemberData(nameof(ValidVariableNames))]
    public void ThrowIfInvalidKey_does_not_throw_for(string value) =>
        Assert.Null(Record.Exception(() => Variable.ThrowIfInvalidKey(value)));

    [Theory]
    [MemberData(nameof(InvalidVariableNames))]
    public void ThrowIfInvalidKey_throws_for(string value) =>
        Assert.Throws<ArgumentOutOfRangeException>(() => Variable.ThrowIfInvalidKey(value));

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
