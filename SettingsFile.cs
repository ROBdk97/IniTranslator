using System;
using System.Linq;
using System.Windows;

namespace IniTranslator
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
    }
}
