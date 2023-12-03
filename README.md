# WeCantSpell.Roslyn

WeCantSpell.Roslyn is a spell check analyzer for Roslyn languages, such as C# and Visual Basic. It uses [WeCantSpell.Hunspell](https://www.nuget.org/packages/WeCantSpell.Hunspell/), a fully managed C# port of the Hunspell spell checker engine, to check and suggest words.

## Features

- Reads Hunspell DIC and AFF file formats
- Supports checking and suggesting words
- Can be queried concurrently
- Compatible with .NET Core, .NET Standard and multiple .NET framework versions

## Installation

You can install WeCantSpell.Roslyn as a NuGet package using the following command:

`dotnet add package WeCantSpell.Roslyn`

## Configuration

To use WeCantSpell.Roslyn, you need to configure the analyzer options in your project. There are two ways to do this:

- Using `.wecantspell` or `.wecantspell.json` files, located in the project root or inside nested directories. The structure of the file is as follows:

```json
{
    "isRoot" : true,
    "languages" : [
        "en-US",
        "ru-RU"
    ]
}
```

The `isRoot` property indicates whether the file is located in the solution root or not. The `languages` property specifies the list of languages to use for spell checking.

- Using additional project dictionaries in Hunspell format, named one of: `.spelling.dic`, `.directory.dic`, `.wecantspell.dic`. These files can also be located in the project root or inside nested directories. They contain custom words to add to the spell checker.

You also need to set the severity of the analyzer diagnostics in your editorconfig file. For example:

```ini
root = true

[*.{cs,vb}]
# WeCantSpell properties
dotnet_diagnostic.SP3110.severity = warning
dotnet_diagnostic.SP3111.severity = suggestion
dotnet_diagnostic.SP3112.severity = suggestion
dotnet_diagnostic.SP3113.severity = suggestion
```

The analyzer will report spelling errors in your code according to the following error codes:

- SP3110: Identifier Spelling - Identifier name may contain a spelling error.
- SP3111: Text Literal Spelling - Text literal may contain a spelling error.
- SP3112: Comment Spelling - Comment may contain a spelling error.
- SP3113: Xml Documentation Spelling - Xml documentation may contain a spelling error.

For more details, please refer to the [source code](https://github.com/aarondandy/WeCantSpell.Roslyn) of the project.