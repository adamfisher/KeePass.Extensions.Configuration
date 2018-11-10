# KeePass.Extensions.Configuration

[![](https://raw.githubusercontent.com/pixel-cookers/built-with-badges/master/nuget/nuget-long.png)](https://www.nuget.org/packages/KeePass.Extensions.Configuration)

KeePass configuration provider implementation for `Microsoft.Extensions.Configuration` allows you to connect a KeePass `.kdbx` database file into your configuration providers for .NET Core, keeping sensitive information in your application secure.

## Getting Started

### Database with No Authentication

You can add a KeePass database file by simply passing the path to the database file if there is no authentication mechanisms required (like a password or master key file):

```csharp
builder.AddKeePass("Path/To/KeePass.kdbx");
```

### Specifying the Master Password

Adding a KeePass database file requiring a password is as simple as specifying it as the second parameter:

```csharp
builder.AddKeePass("Path/To/KeePass.kdbx", "MyPassword");
```

### Using The Current Windows Account

If you used Windows account-based authentication to setup your database file, you will want to set `useCurrentWindowsAccount: true` to use the currently running Windows account at runtime.

```csharp
builder.AddKeePass("Path/To/KeePass.kdbx", useCurrentWindowsAccount: true);
```

## License

This library is published under the Apache 2.0 license.