# dotnet-dotenv

<img src=icon.png width=128></img>  
A lightweight .NET utility designed to read and write `.env` configuration files.

Supports advanced interpolation features such as direct substitution and fallback mechanisms with default values.

```cs
var dotEnv = DotEnv.Load("path/to/.env");

// Apply to environment
dotEnv.ApplyToEnvironment();

// Access values
Console.WriteLine($"API_KEY: {dotEnv["API_KEY"]}");

// Remove from environment
dotEnv.RemoveFromEnvironment();
```

## Features

- Load environment variables from a file or stream.
- Apply loaded environment variables to the current environment.
- Remove environment variables from the current environment.
- Save environment variables back to a `.env` file.
- Supports variable interpolation.

## Loading Environment Variables
1. Load from a .env File
```cs
var dotEnv = DotEnv.Load("path/to/.env");
```
2. Load from a Stream
```cs
using var stream = File.OpenRead("path/to/.env");
var dotEnv = DotEnv.Load(stream);
```

## Applying Environment Variables

Set the environment variables in the current process using `ApplyToEnvironment`.

```cs
dotEnv.ApplyToEnvironment();
```

To load environment variables from a file and apply them directly to the environment:

```cs
DotEnv.LoadAndApply("path/to/.env");
```

## Removing Environment Variables

To remove the environment variables from the current process:

```cs
dotEnv.RemoveFromEnvironment();
```

## Saving to a File

To save the current environment variables to a .env file:

```cs
dotEnv.Save("path/to/.env");
```

## Variable interpolation

Interpolation is supported (braced (${VAR}) and unbraced ($VAR)) for unquoted and double-quoted values. 

- Direct substitution
    ${VAR} -> value of VAR
- Default value
  - ${VAR:-default} -> value of VAR if set and non-empty, otherwise default
  - ${VAR-default} -> value of VAR if set, otherwise default


```dotenv
HOSTNAME=localhost
URL=https://$(HOSTNAME):${PORT:-8080}/
```

```cs
var dotEnv = DotEnv.Load("path/to/.env");
Console.WriteLine(dotEnv[URL]); // https://localhost:8080/
```

Variables are resolved at load time only, by the order they're defined in the .env file.

# ASP .NET Core

The `AddDotEnvFile` method provides a straightforward way to integrate environment variables from a `.env` file into the ASP.NET Core configuration system.

You can customize the file path and whether the file is optional, as well as configure reloading behavior when the file changes. This makes managing environment variables more efficient and streamlined within your application.

```cs
// Load the default .env file
builder.Configuration.AddDotEnvFile();
// Load an optional .env file
builder.Configuration.AddDotEnvFile("path/to/.env", optional: true);
```