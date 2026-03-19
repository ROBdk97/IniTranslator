# IniTranslator

![GitHub all releases](https://img.shields.io/github/downloads/ROBdk97/IniTranslator/total)

IniTranslator is a Windows WPF desktop application for comparing, editing, and maintaining INI-based translations. It helps translators keep English and localized INI files synchronized with side-by-side editing, search and replace, placeholder validation, automatic backups, version comparison, and direct extraction from Star Citizen P4K archives.

![Screenshot](Image.png)

## Table of Contents

- [Overview](#overview)
- [Features](#features)
- [Requirements](#requirements)
- [Installation](#installation)
- [Build from Source](#build-from-source)
- [Usage](#usage)
- [Keyboard Shortcuts](#keyboard-shortcuts)
- [Settings](#settings)
- [Project Structure](#project-structure)
- [Third-Party Components](#third-party-components)
- [Contributing](#contributing)
- [License](#license)
- [Repository](#repository)

## Overview

IniTranslator is designed for translation maintenance rather than generic text editing.

Typical workflow:

1. Open the current English INI file and the translated INI file.
2. Review entries in a side-by-side editor.
3. Search, filter, replace, or copy source text into translation fields.
4. Translate selected rows with Google Translate or DeepL.
5. Validate placeholders before saving.
6. Save changes with an automatic `.bak` backup of the translated file.
7. Optionally compare against an older English INI or extract a fresh `global.ini` from Star Citizen.

## Features

- Side-by-side INI editing for source and translated values.
- Fast filtering by key, English text, or translated text.
- Optional regular-expression search and case-insensitive search.
- Batch replace on selected translations.
- Copy/paste support for structured `key=value` clipboard data.
- Copy English source text directly into translation fields.
- Translation of selected rows through Google Translate or DeepL.
- Placeholder mismatch detection for patterns such as `%xx` and Star Citizen action tokens.
- Automatic backup creation when saving translated files.
- Reload backup support from the main UI.
- Comparison against an older INI file to identify changed entries.
- Extraction of `Data/Localization/english/global.ini` from Star Citizen `Data.p4k` archives.
- Built-in P4K archive explorer with search, file export, and directory extraction.
- Light, Dark, and System theme support.
- Persisted window position, size, language, search settings, provider choice, and game path.
- Localized UI with built-in language options:
  English, Spanish, French, Italian, Lithuanian, Portuguese (Brazil), and German.

## Requirements

- Windows 10 or later.
- Star Citizen installed locally if you want to use archive extraction or the P4K explorer.
- For source builds: .NET 10 SDK.

Notes:

- The GitHub release package is published as a self-contained `win-x64` build.
- API keys are only required if you want to use machine translation features.

## Installation

Download the latest packaged build from GitHub Releases:

- [Release.zip](https://github.com/ROBdk97/IniTranslator/releases/latest/download/Release.zip)

Then:

1. Extract the archive.
2. Run `IniTranslator.exe`.

No separate .NET desktop runtime installation is required for the published release package.

## Build from Source

The CI pipeline restores and publishes the WPF application as a self-contained `win-x64` build with .NET 10.

To build locally:

```powershell
dotnet restore .\IniTranslator\IniTranslator.csproj -r win-x64
dotnet build .\IniTranslator\IniTranslator.csproj -c Release -r win-x64
```

To publish a release-style build locally:

```powershell
dotnet publish .\IniTranslator\IniTranslator.csproj -c Release -r win-x64 --no-restore -o publish -p:DebugType=None -p:PublishSingleFile=true --self-contained true
```

## Usage

### Open and edit translations

Use `File > Open` to select:

1. The English source INI file.
2. The translated INI file.

The main list shows:

- Line number
- Key
- English value
- Editable translation value

### Search and filter

The toolbar search box filters the current list in real time.

- Search covers key, English text, and translated text.
- `RegEx` enables regular expression matching.
- `Ignore Case` switches between ordinal and case-insensitive matching.
- `Replace` opens a dialog to update selected translations in bulk.

### Translation helpers

Editing commands are available from the menu and keyboard shortcuts:

- Copy selected entries to the clipboard.
- Paste structured `key=value` content back into matching selected rows.
- Copy English text into the translation column.
- Translate selected rows with the configured provider.

Currently implemented machine translation providers:

- Google Translate
- DeepL

### Placeholder validation

IniTranslator checks placeholders used in the English value against the translated value and helps you jump between mismatches.

Examples of supported placeholder patterns include:

- `%s`, `%d`, `%r`
- `[~action(...)]`
- `~action(...)`

When mismatches are found, the app warns before saving so you can review them.

### Backups and reload

When you save a translated file, IniTranslator creates a backup next to it:

- `yourfile.ini.bak`

You can restore that backup with `File > Load Backup`.

### Compare with an older INI

Use `File > Open Old INI File` to load an earlier English INI version.

This workflow is intended to help when the source file changes between game versions:

- missing keys are added back into the current translation set
- entry ordering is synchronized with the current English file
- changed source entries can be reviewed with the `Jump to Next Change` action

### Extract from Star Citizen

Use `File > Extract from Game` to extract the current English `global.ini` from a selected Star Citizen installation version.

The app attempts to find the game automatically by:

- checking the default installation directory
- falling back to the RSI Launcher log

After selecting a version, IniTranslator reads `Data.p4k`, extracts `Data/Localization/english/global.ini`, and reloads the current translation view against the extracted file.

### P4K archive explorer

Use `File > Explore P4K Archive` to open the built-in archive browser.

The explorer supports:

- loading a Star Citizen `Data.p4k` archive
- browsing directories in a tree view
- searching files by name/path
- viewing basic metadata for the selected item
- exporting individual files
- extracting entire directories

CryXmlB files are exported as `.xml` when applicable.

## Keyboard Shortcuts

- `Ctrl+O`: Open English and translated INI files
- `Ctrl+S`: Save translated INI file
- `Ctrl+C`: Copy selected entries
- `Ctrl+V`: Paste clipboard content into selected entries
- `Ctrl+M`: Copy English values into the translation column
- `Ctrl+T`: Translate selected entries
- `Ctrl+J`: Jump to a specific line

Toolbar actions also expose these navigation features:

- Jump to next changed entry
- Jump to next placeholder mismatch

## Settings

The settings window includes:

- UI language
- application theme
- translation provider
- DeepL API key
- Google Translate API key
- Star Citizen installation path

Settings are stored in:

- `%AppData%\ROBdk97\IniTranslator\settings.json`

API keys are stored in the settings file in encrypted form by the application.

## Project Structure

- `IniTranslator/`: main WPF application
- `IniTranslator/ViewModels/`: main editing, settings, help, and P4K explorer logic
- `IniTranslator/Windows/`: dialogs and secondary windows
- `IniTranslator/Helpers/`: settings, translation, file equalization, clipboard, and path helpers
- `IniTranslator/Models/`: settings and translation data models
- `ROBdk97.Unp4k/`: modified P4K archive handling library used by the app
- `IniTranslaterSetup/`: legacy setup project artifacts

## Third-Party Components

- Modified version of [dolkensp/unp4k](https://github.com/dolkensp/unp4k)
- Modified version of [icsharpcode/SharpZipLib](https://github.com/icsharpcode/SharpZipLib)
- [DeepL.net](https://www.nuget.org/packages/DeepL.net)
- `libzstd.dll` / Zstandard support used by the archive tooling

## Contributing

Contributions are welcome.

1. Fork the repository.
2. Create a branch for your change.
3. Make and test the change.
4. Open a pull request with a clear description.

If you plan a larger change, open an issue first so the scope can be discussed.

## License

This project is licensed under the [MIT License](https://github.com/ROBdk97/IniTranslator/blob/main/LICENSE).

## Repository

- GitHub: [ROBdk97/IniTranslator](https://github.com/ROBdk97/IniTranslator)
- Issues: [Open an issue](https://github.com/ROBdk97/IniTranslator/issues)
