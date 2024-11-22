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

### Features

- **Multi-Language INI File Handling**: Open, edit, and compare English and translated INI files side-by-side, with support for extracting INI files directly from Star Citizen game archives.  
- **Real-Time Search and Filtering**: Quickly locate translations with regex and case-insensitive search options, enabling efficient navigation through complex INI files.  
- **Advanced Placeholder Validation**: Detect and resolve mismatched or missing placeholders with automatic navigation to problematic entries.  
- **Integrated Backup System**: Create, restore, and manage translation backups seamlessly, ensuring data safety.  
- **Translation API Integration**: Use Google Translate or DeepL APIs for quick and accurate translations, configurable in the settings.  
- **Version Comparison**: Compare translations with older INI versions, identify differences, and jump directly to changes.  
- **Customizable User Interface**: Switch between Light and Dark themes, and save preferences like window size, position, and settings for a consistent experience.  
- **Keyboard-Driven Navigation**: Perform actions like jumping to lines, replacing text, or translating entries using intuitive keyboard shortcuts.  
- **Contextual Editing Tools**: Copy English values to translations, replace text with regex support, and paste structured clipboard data into selected entries.  
- **File System Integration**: Open INI files directly in Windows Explorer or extract English files from game data archives for complete workflow management.  
- **Live Progress Indicators**: Monitor task completion status with progress bars and detailed status messages.  
- **Comprehensive Help and Documentation**: In-app guidance, including detailed keyboard shortcuts, contextual tooltips, and a link to the GitHub repository for community support.

## Installation

### Prerequisites

- **.NET 8 Desktop Runtime**: Make sure you have the .NET 8 Desktop Runtime installed. The setup file will install it if necessary.

### Download Options

1. **Download the Setup**: The setup file (`IniTranslatorSetup.msi`) will install the application, create desktop and start menu entries, and ensure the .NET 8 Desktop Runtime is installed.

   - [Download IniTranslator Setup](https://github.com/ROBdk97/IniTranslator/releases/download/latest/IniTranslatorSetup.msi)

2. **Download the Executable**: If you already have the .NET 8 Desktop Runtime installed, you can directly download the program executable (`IniTranslator.exe`).

   - [Download IniTranslator Executable](https://github.com/ROBdk97/IniTranslator/releases/download/latest/IniTranslator.exe)

# Ini Translator - Help & Documentation

## General Usage:
- **Open Files:** Use *File > Open...* or press `Ctrl + O` to load English and Translated INI files.
- **Extract INI:** Use *File > Extract from Game* to retrieve the English INI file from Star Citizen game archives.
- **Edit Translations:** Click on items in the list to modify their translations.
- **Translate:** Use *Edit > Translate* or press `Ctrl + T` to automatically translate selected items.
- **Validate Placeholders:** Identify mismatched placeholders using the "Next Missing Placeholder" toolbar button.
- **Save Progress:** Save translations with *File > Save* or press `Ctrl + S`.
- **Restore Backups:** Reload previous translations using *File > Load Backup*.

## Keyboard Shortcuts:
- **Ctrl + O:** Open INI files
- **Ctrl + S:** Save translations
- **Ctrl + C:** Copy selected items to clipboard
- **Ctrl + V:** Paste clipboard contents into selected items
- **Ctrl + M:** Copy English values to translation
- **Ctrl + T:** Translate selected items
- **Ctrl + J:** Jump to a specific line
- **F3:** Jump to the next changed entry
- **F4:** Jump to the next mismatched placeholder

## Menu Options:

### File:
- **Open...**: Load English and Translated INI files.
- **Open Old INI File...**: Load an older INI version for comparison.
- **Extract from Game**: Extract the English INI file from game data archives.
- **Save**: Save translations to the translated INI file.
- **Reload**: Reload translations from the currently loaded INI files.
- **Load Backup**: Restore translations from a backup file.
- **Show in Explorer**: Open the file location in Windows Explorer.
- **Exit**: Close the application.

### Edit:
- **Copy**: Copy selected items to clipboard.
- **Paste**: Paste clipboard contents into selected items.
- **Copy from English**: Replace translations with English values.
- **Translate**: Automatically translate selected items using the configured API.
- **Replace...**: Find and replace text within selected translations.
- **Jump to Line...**: Navigate to a specific line.

### Tools:
- **Settings**: Open the settings window to configure preferences like API keys and themes.
- **Theme**: Switch between Light and Dark themes.

### Help:
- **Documentation**: View detailed usage instructions.
- **About**: Learn more about Ini Translator.

## Toolbar Functions:
- **Jump to Next Change:** Navigate to the next updated or changed translation.
- **Next Missing Placeholder:** Highlight the next entry with mismatched placeholders.
- **Search Box:** Filter translations using keywords or regular expressions.
- **Replace:** Find and replace text in selected translations.
- **Switch Theme:** Toggle between Light and Dark themes.
- **Total Keys:** Display the total number of translation keys.

## Additional Features:
- **Translation API Support:** Use Google Translate or DeepL for automated translations.
- **Placeholder Validation:** Detect missing or mismatched placeholders before saving translations.
- **Backup Management:** Automatically create backups when saving translations.
- **INI Version Comparison:** Compare translations with older INI files to identify changes.

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
