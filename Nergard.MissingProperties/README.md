# Nergard.MissingProperties

An Optimizely CMS 12 admin tool that scans for orphaned property definitions — properties that exist in the database but no longer have a corresponding code model — and lets you delete them.

## Requirements

- .NET 8.0
- Optimizely CMS 12.29+
- MudBlazor 8.x

## Installation

### 1. Add the project reference

```xml
<ProjectReference Include="..\Nergard.MissingProperties\Nergard.MissingProperties.csproj" />
```

### 2. Register services in `Startup.cs`

```csharp
services.AddMudServices();
services.AddMissingProperties();
```

### 3. Render it how you want

I just create a content type specific for the tool and render the component. See example folder for an example of a .cshtml that works.

## How It Works

The tool scans all **code-defined** page types and block types (i.e. those with a `ModelType`). For each one it checks whether every property definition stored in the database still has a matching property on the model. Any that don't are listed as missing.

You can then select and delete individual properties, or delete all of them at once. Each delete action requires confirmation and cannot be undone.

Manually created content types (no `ModelType`) are intentionally skipped.

## License

MIT — see [LICENSE](LICENSE)
