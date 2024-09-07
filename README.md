# IniTranslator

**IniTranslator** is a WPF-based desktop application designed to help users compare, edit, and manage translations in INI files, typically used for software localization. It allows for easy synchronization between English and translated versions of INI files, providing features for searching, editing, and managing translation tasks efficiently.

## Table of Contents

- [Features](#features)
- [Installation](#installation)
- [Usage](#usage)
- [Keyboard Shortcuts](#keyboard-shortcuts)
- [Contributing](#contributing)
- [License](#license)
- [Contact](#contact)

## Features

- **Open and Compare INI Files**: Easily load and compare English and translated INI files.
- **Powerful Search Functionality**: Use regex and case-insensitive search to filter and locate translations.
- **Keyboard Shortcuts for Efficiency**: Perform common actions like copying, pasting, translating, and jumping to lines using keyboard shortcuts.
- **Persistent User Settings**: The app remembers window size, position, and state between sessions.
- **Backup and Reload**: Create backups of translations and reload them when needed.
- **Integrated Translator API Support**: Use external translation APIs for quick translations.
- **Open Old INI File to Compare Changes**: Load an older version of an INI file to compare it with the current version, which helps in identifying changes and jumping to the next change.
- **Jump to Line and Next Change Navigation**: Quickly navigate to a specific line or the next change.
- **Help and About Dialogs**: Get information about using the app and keyboard shortcuts.

## Installation

### Prerequisites

- **.NET 8 Desktop Runtime**: Make sure you have the .NET 8 Desktop Runtime installed. The setup file will install it if necessary.

### Download Options

1. **Download the Setup**: The setup file (`IniTranslatorSetup.msi`) will install the application, create desktop and start menu entries, and ensure the .NET 8 Desktop Runtime is installed.

   - [Download IniTranslator Setup](https://github.com/ROBdk97/IniTranslator/releases/download/latest/IniTranslatorSetup.msi)

2. **Download the Executable**: If you already have the .NET 8 Desktop Runtime installed, you can directly download the program executable (`IniTranslator.exe`).

   - [Download IniTranslator Executable](https://github.com/ROBdk97/IniTranslator/releases/download/latest/IniTranslator.exe)

## Usage

1. **Open INI Files**:
   - Use the "Open" button to select and load English and translated INI files for comparison.
2. **Open Old INI File**:
   - Use the "Open Old INI File" option to select an older version of an INI file. This is useful for comparing changes and utilizing the "Jump to Next Change" feature.
3. **Search and Filter Translations**:
   - Enter text in the search box to filter translations. Enable "Regex" for advanced regular expression search or "Ignore Case" for case-insensitive search.
4. **Edit Translations**:
   - Select items in the list and use the context menu or keyboard shortcuts to copy, paste, translate, or manage translations.
5. **Save Changes**:
   - Use the "Save" button to save any changes made to the translated INI file.
6. **Backup and Reload**:
   - Use the backup and reload features to prevent data loss and restore previous states.

## Keyboard Shortcuts

- **Ctrl + C**: Copy selected items to clipboard.
- **Ctrl + V**: Paste clipboard contents to selected items.
- **Ctrl + M**: Copy English values to the translation column.
- **Ctrl + T**: Use Translator API to translate selected items.
- **Ctrl + J**: Jump to a specific line number.
- **Ctrl + S**: Save the current changes.
- **Ctrl + O**: Open an English or translated INI file.
- **Ctrl + N**: Jump to the next change in the list.
- **Ctrl + H**: Show help dialog with usage instructions and shortcuts.

## Contributing

Contributions are welcome! If you want to contribute to the project, follow these steps:

1. Fork the repository on GitHub.
2. Create a new branch for your feature: `git checkout -b feature/YourFeatureName`.
3. Commit your changes: `git commit -m 'Add some feature'`.
4. Push to the branch: `git push origin feature/YourFeatureName`.
5. Open a pull request on GitHub.

For major changes, please open an issue to discuss what you would like to change.

## License

This project is licensed under the MIT License. See the [LICENSE](https://github.com/ROBdk97/IniTranslator/blob/main/LICENSE) file for details.

## Contact

For more information, issues, or feature requests, please visit the [GitHub repository](https://github.com/ROBdk97/IniTranslator) or open an issue there.
