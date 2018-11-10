# KeePass.Extensions.Configuration

[![](https://raw.githubusercontent.com/pixel-cookers/built-with-badges/master/nuget/nuget-long.png)](https://www.nuget.org/packages/KeePass.Extensions.Configuration)

KeePass configuration provider implementation for `Microsoft.Extensions.Configuration` allows you to connect a KeePass `.kdbx` database file into your configuration providers for .NET Core, keeping sensitive information in your application secure.

## Getting Started

Each entry inside a KeePass database corresponds to a configuration entry. The grouping hierarchy and title of an entry determines the **key** while the **value** is the password field by default. You can change these default mappings by specifying a `resolveKey()` or `resolveValue()` function or limiting the number of entries loaded into the configuration by specifying a `filterEntries()` function. More details are provided in the *Advanced Configuration* section below.

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

## Example

Suppose you have a database called **KeePassTestDatabase.kdbx** containing the following three groups and eight entries along with a master password of `1234`:

[![](https://raw.githubusercontent.com/adamfisher/KeePass.Extensions.Configuration/master/Images/KeePassTestDatabase-Root.PNG)](#)
[![](https://raw.githubusercontent.com/adamfisher/KeePass.Extensions.Configuration/master/Images/KeePassTestDatabase-Internet.PNG)](#)
[![](https://raw.githubusercontent.com/adamfisher/KeePass.Extensions.Configuration/master/Images/KeePassTestDatabase-DuplicateEntries.PNG)](#)

The available keys are based on the `resolveKey` and `resolveValue` delegate functions. See the *Advanced Configuration* section for ways to customize the key/value mappings of entries to configuration.

Based on the default key and value resolvers, the following keys are available once the configuration provider is built:

| Key                                                                | Value                |
|--------------------------------------------------------------------|----------------------|
| KeePassTestDatabase:Sample Entry:Sammy34                           | DayOldEggs458        |
| KeePassTestDatabase:Sample Entry:Michael321                        | 12345                |
| KeePassTestDatabase:Internet:Facebook:Takent33                     | jaehai0Ush           |
| KeePassTestDatabase:Internet:Twitter:Nottlespiche                  | zouQuo3ia            |
| KeePassTestDatabase:Internet:npm:FrancesRBenjamin@teleworm.us      | ooch5Eeroi           |
| KeePassTestDatabase:Duplicate Entries:Duplicate Entry:BaconRules   | WU1uiERyRsWtv5sDDl1e |
| KeePassTestDatabase:Duplicate Entries:Duplicate Entry:BaconRules`1 | WU1uiERyRsWtv5sDDl1e |
| KeePassTestDatabase:Duplicate Entries:Duplicate Entry:BaconRules`2 | WU1uiERyRsWtv5sDDl1e |

## Advanced Configuration

### Filter Entries

For security reasons, maybe you don't want to load all of the entries in a KeePass database into the configuration provider. You can specify an optional filter function to limit the entries to only the ones matching a specified condition.

In the follow example, we are only allowing password entries containing `"Used by web app"` in the notes of the entry:

```csharp
builder.AddKeePass("Path/To/KeePass.kdbx",
                filterEntries: database =>
                    database.RootGroup.GetEntries(true)
                        .Where(entry => entry.Strings.ReadSafe("Notes").Contains("Used by web app")));
```

### Custom Key & Value Resolvers

By default, **keys** are resolved to be colon separated based on the group hierarchy and title of the entry while **values** is the the value stored in the password field of the entry.

You can override one or both of the ways a key and value are resolved if there are other fields that make more sense for you. In the following example, we are overriding both the key to only be the title of the entry and the value to be the notes field.

```csharp
builder.AddKeePass("Path/To/KeePass.kdbx",
	resolveKey: entry => entry.Strings.ReadSafe("Title"),
	resolveValue: (key, entry) => entry.Strings.ReadSafe("Notes")
);
```

## License

This library is published under the Apache 2.0 license.