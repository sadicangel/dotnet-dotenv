﻿namespace DotNetDotEnv.Tests;
public class ParserTests
{
    [Fact]
    public void Ignores_comments()
    {
        var env = Parser.Parse("""
            # this is a comment
            """);

        Assert.Empty(env);
    }

    [Fact]
    public void Ignores_keyless_lines()
    {
        var env = Parser.Parse("""
            =Value
            """);

        Assert.Empty(env);
    }

    [Fact]
    public void Parses_empty_values()
    {
        var env = Parser.Parse("""
            Key=
            """);

        Assert.Empty(env["Key"]);
    }

    [Fact]
    public void Parses_unquoted_value()
    {
        var env = Parser.Parse("""
            Key=Value
            """);

        Assert.Equal("Value", env["Key"]);
    }

    [Fact]
    public void Parses_unquoted_white_space_key()
    {
        var env = Parser.Parse("""
             Key =Value
            """);

        Assert.Equal("Value", env["Key"]);
    }

    [Fact]
    public void Parses_unquoted_white_space_value()
    {
        var env = Parser.Parse("""
            Key= Value 
            """);

        Assert.Equal("Value", env["Key"]);
    }

    [Fact]
    public void Parses_unquoted_value_with_inline_comment()
    {
        var env = Parser.Parse("""
            Key=Value # this is a comment
            """);

        Assert.Equal("Value", env["Key"]);
    }

    [Fact]
    public void Parses_single_quoted_value()
    {
        var env = Parser.Parse("""
            Key='#Value#'
            """);

        Assert.Equal("#Value#", env["Key"]);
    }

    [Fact]
    public void Parses_double_quoted_value()
    {
        var env = Parser.Parse("""
            Key="#Value#"
            """);

        Assert.Equal("#Value#", env["Key"]);
    }

    [Fact]
    public void Parses_double_quoted_multiline_value()
    {
        var env = Parser.Parse("""
            Key="
            #
            
            Value
            
            #"
            """);

        Assert.Equal("""

            #

            Value

            #
            """, env["Key"]);
    }

    [Fact]
    public void Parses_double_quoted_multiline_value_unclosed()
    {
        var env = Parser.Parse("""
            Key="
            #
            
            Value
            
            #

            """);

        Assert.Equal("""

            #

            Value

            #

            """, env["Key"]);
    }

    [Fact]
    public void Parses_unbraced_interpolated_value_from_inline_value()
    {
        var env = Parser.Parse("""
            Key1="Value"
            Key2=$Key1
            """);

        Assert.Equal("Value", env["Key2"]);
    }

    [Fact]
    public void Parses_unbraced_interpolated_value_from_environment_variable()
    {
        Environment.SetEnvironmentVariable("Key1", "Value");
        var env = Parser.Parse("""
            Key2=$Key1
            """);

        Assert.Equal("Value", env["Key2"]);
    }

    [Fact]
    public void Parses_braced_interpolated_value_from_inline_value()
    {
        var env = Parser.Parse("""
            Key1="Value"
            Key2=${Key1}
            """);

        Assert.Equal("Value", env["Key2"]);
    }

    [Fact]
    public void Parses_braced_interpolated_value_from_environment_variable()
    {
        Environment.SetEnvironmentVariable("Key1", "Value");
        var env = Parser.Parse("""
            Key2=${Key1}
            """);

        Assert.Equal("Value", env["Key2"]);
    }

    [Fact]
    public void Parses_braced_interpolated_value_default_if_empty_or_null_with_null_value()
    {
        var env = Parser.Parse("""
            Key2=${Key1:-Value}
            """);

        Assert.Equal("Value", env["Key2"]);
    }

    [Fact]
    public void Parses_braced_interpolated_value_default_if_empty_or_null_with_empty_value()
    {
        var env = Parser.Parse("""
            Key1=
            Key2=${Key1:-Value}
            """);

        Assert.Equal("Value", env["Key2"]);
    }

    [Fact]
    public void Parses_braced_interpolated_value_default_if_null_with_null_value()
    {
        var env = Parser.Parse("""
            Key2=${Key1:-Value}
            """);

        Assert.Equal("Value", env["Key2"]);
    }
}