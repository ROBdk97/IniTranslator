using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using IniTranslator.Helpers;
using IniTranslator.Models;
using IniTranslator.Properties;
using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Windows;

namespace IniTranslator.ViewModels
{
    internal partial class SettingsViewModel : ObservableObject
    {
        private readonly SettingsFile _settingsFile;

        public SettingsViewModel(SettingsFile settingsFile) { _settingsFile = settingsFile; }

        [RelayCommand]
        private void SaveSettings()
        {
            _settingsFile.SaveSettings();
        }

        [RelayCommand]
        private void SaveAll(IniTranslator.Helpers.PasswordPair? passwords)
        {
            if (passwords is not null)
            {
                if (!string.IsNullOrWhiteSpace(passwords.GoogleApiKey))
                    GoogleTranslateApiKey = passwords.GoogleApiKey;
                if (!string.IsNullOrWhiteSpace(passwords.DeepLApiKey))
                    DeepLApiKey = passwords.DeepLApiKey;
            }

            _settingsFile.SaveSettings();
        }

        public ObservableCollection<Language> Languages
        {
            get;
        } =
[
    new Language { Name = Resources.English, Code = "en" },
    new Language { Name = Resources.SpanishSpain, Code = "es-ES" },
    new Language { Name = Resources.FrenchFrance, Code = "fr-FR" },
    new Language { Name = Resources.ItalianItaly, Code = "it-IT" },
    new Language { Name = Resources.LithuanianLithuania, Code = "lt-LT" },
    new Language { Name = Resources.PortugueseBrazil, Code = "pt-BR" },
    new Language { Name = Resources.GermanGermany, Code = "de-DE" }
];


        public ObservableCollection<ThemeMode> Themes
        {
            get;
        } =
        [ThemeMode.Light, ThemeMode.Dark, ThemeMode.System];

        public ObservableCollection<string> TranslationProviders
        {
            get;
        } =
        [
            Models.TranslationProvider.None.ToString(),
            Models.TranslationProvider.GoogleTranslate.ToString(),
            Models.TranslationProvider.DeepL.ToString(),
            Models.TranslationProvider.OpenAI.ToString()
        ];

        public ThemeMode Theme
        {
            get => _settingsFile.Theme;
            set
            {
                _settingsFile.Theme = value;
                SettingsManager.SetTheme(value);
                OnPropertyChanged();
            }
        }

        public string Language
        {
            get => _settingsFile.Language;
            set
            {
                _settingsFile.Language = value;
                _settingsFile.SetLanguage();
                OnPropertyChanged();
            }
        }

        public string TranslationProvider
        {
            get => _settingsFile.TranslationProvider.ToString();
            set
            {
                if (Enum.TryParse(value, out TranslationProvider provider))
                {
                    _settingsFile.TranslationProvider = provider;
                    OnPropertyChanged();
                }
                Debug.WriteLine($"Failed to parse {value} to TranslationProvider");
            }
        }

        public string GoogleTranslateApiKey
        {
            get => CryptoHelper.Decrypt(_settingsFile.GoogleApiKey);
            set
            {
                _settingsFile.GoogleApiKey = CryptoHelper.Encrypt(value);
                OnPropertyChanged();
            }
        }

        public string DeepLApiKey
        {
            get => CryptoHelper.Decrypt(_settingsFile.DeepLApiKey);
            set
            {
                _settingsFile.DeepLApiKey = CryptoHelper.Encrypt(value);
                OnPropertyChanged();
            }
        }

        public string? StarCitizenPath
        {
            get => _settingsFile.StarCitizenPath;
            set
            {
                _settingsFile.StarCitizenPath = value;
                OnPropertyChanged();
            }
        }
    }
}
