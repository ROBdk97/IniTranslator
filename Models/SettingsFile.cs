using DeepL;
using System;
using System.Linq;
using System.Windows;

namespace IniTranslator.Models
{
    public class SettingsFile
    {
        public string? EnglishIniPath { get; set; }
        public string? TranslatedIniPath { get; set; }
        public bool Regex { get; set; }
        public bool IgnoreCase { get; set; }
        public double WindowWidth { get; set; } = 800;
        public double WindowHeight { get; set; } = 600;
        public double WindowLeft { get; set; }
        public double WindowTop { get; set; }
        public double WindowRight { get; set; }
        public double WindowBottom { get; set; }
        public WindowState WindowState { get; set; } = WindowState.Normal;
        public TranslationProvider TranslationProvider { get; set; } = TranslationProvider.None;
        public string GoogleApiKey { get; set; } = string.Empty;
        public string DeepLApiKey { get; set; } = string.Empty;
        public string Language { get; set; } = LanguageCode.English;
        public string Theme { get; set; } = "Light";
    }
}
