using DeepL;
using IniTranslator.Helpers;
using IniTranslator.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IniTranslator.ViewModels
{
    internal class SettingsViewModel : BaseModel
    {
        private readonly SettingsFile _settingsFile;

        public SettingsViewModel(SettingsFile settingsFile) { _settingsFile = settingsFile; }

        public void SaveSettings() { _settingsFile.SaveSettings(); }

        public ObservableCollection<Language> Languages
        {
            get;
        } =
[
    new Language { Name = "Dutch", Code = LanguageCode.Dutch },
    new Language { Name = "English", Code = LanguageCode.English },
    new Language { Name = "French", Code = LanguageCode.French },
    new Language { Name = "German", Code = LanguageCode.German },
    new Language { Name = "Italian", Code = LanguageCode.Italian },
    new Language { Name = "Lithuanian", Code = LanguageCode.Lithuanian },
    new Language { Name = "Polish", Code = LanguageCode.Polish },
    new Language { Name = "Portuguese", Code = LanguageCode.Portuguese },
    new Language { Name = "Portuguese (Brazil)", Code = LanguageCode.PortugueseBrazilian },
    new Language { Name = "Russian", Code = LanguageCode.Russian },
    new Language { Name = "Spanish", Code = LanguageCode.Spanish },
    new Language { Name = "Turkish", Code = LanguageCode.Turkish }
];


        public ObservableCollection<string> Themes
        {
            get;
        } =
        [
            "Light",
            "Dark"
        ];

        public ObservableCollection<string> TranslationProviders
        {
            get;
        } =
        [
            Models.TranslationProvider.None.ToString(),
            Models.TranslationProvider.GoogleTranslate.ToString(),
            Models.TranslationProvider.DeepL.ToString()
        ];

        public string Theme
        {
            get => _settingsFile.Theme;
            set
            {
                _settingsFile.Theme = value;
                OnPropertyChanged();
            }
        }

        public string Language
        {
            get => _settingsFile.Language;
            set
            {
                _settingsFile.Language = value;
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
    }
}
