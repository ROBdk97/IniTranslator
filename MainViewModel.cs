using System;
using System.Collections;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.Windows;

namespace IniTranslator
{
    public class MainViewModel : BaseModel
    {
        private readonly ObservableCollection<Translations> translations; // Internal collection to be displayed
        private readonly SettingsFile settingsFile; // Holds settings information

        private string searchText = string.Empty; // Search text for filtering translations
        public bool IgnoreCase
        {
            get => Settings.IgnoreCase;
            set
            {
                Settings.IgnoreCase = value;
                OnPropertyChanged(nameof(IgnoreCase));
            }
        }
        public bool Regex
        {
            get => Settings.Regex;
            set
            {
                Settings.Regex = value;
                OnPropertyChanged(nameof(Regex));
            }
        }

        public string SearchText { get => searchText; set => SetProperty(ref searchText, value); }
        public SettingsFile Settings { get => settingsFile; }

        /// <summary>
        /// Expose the translations collection for binding in the UI
        /// </summary>
        public ObservableCollection<Translations> Translations => translations;

        private static readonly string[] separator = ["\r\n"];

        public MainViewModel()
        {
            translations = [];
            settingsFile = SettingsManager.LoadSettings(); // Load settings or create default
            _ = UpdateTranslationsAsync(settingsFile.EnglishIniPath, settingsFile.TranslatedIniPath); // Start updating translations
        }

        /// <summary>
        /// Update translations from INI files asynchronously
        /// </summary>
        /// <param name="englishFilePath"></param>
        /// <param name="translatedFilePath"></param>
        /// <returns></returns>
        internal async Task UpdateTranslationsAsync(string? englishFilePath, string? translatedFilePath)
        {
            Debug.WriteLine("Updating translations");

            if (string.IsNullOrWhiteSpace(englishFilePath) || string.IsNullOrWhiteSpace(translatedFilePath))
                return;

            settingsFile.EnglishIniPath = englishFilePath;
            settingsFile.TranslatedIniPath = translatedFilePath;

            var englishIni = await ReadIniFileAsync(englishFilePath);
            var translatedIni = await ReadIniFileAsync(translatedFilePath);

            translations.Clear(); // Clear existing translations
            var index = 1;
            foreach (var key in englishIni.Keys)
            {
                translations.Add(new Translations
                {
                    Index = index++,
                    Key = key,
                    Value = englishIni[key],
                    Translation = translatedIni.TryGetValue(key, out var value) ? value : string.Empty
                });
            }
            OnPropertyChanged(nameof(Translations));
            Debug.WriteLine("Translations updated");
        }

        private static async Task<Dictionary<string, string>> ReadIniFileAsync(string filePath)
        {
            try
            {
                var lines = await File.ReadAllLinesAsync(filePath, new UTF8Encoding(true));
                return lines
                    .Select(line => line.Split('=', 2))
                    .Where(parts => parts.Length == 2)
                    .ToDictionary(parts => parts[0], parts => parts[1].Trim());
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error reading INI file: {ex.Message}");
                return [];
            }
        }

        /// <summary>
        /// Save translations back to the translated INI file
        /// </summary>
        internal void Save()
        {
            try
            {
                Debug.WriteLine("Saving translations");

                if (settingsFile is null || string.IsNullOrWhiteSpace(settingsFile.TranslatedIniPath))
                    return;

                var outputBuilder = new StringBuilder();
                foreach (var translation in Translations)
                {
                    outputBuilder.AppendLine($"{translation.Key}={translation.Translation}");
                }

                // Create a backup of the translated ini file
                File.Copy(settingsFile.TranslatedIniPath, settingsFile.TranslatedIniPath + ".bak", true);
                // Write the new translated ini file as UTF8 BOM
                File.WriteAllText(settingsFile.TranslatedIniPath, outputBuilder.ToString(), new UTF8Encoding(true));
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error saving translations: {ex.Message}");
            }
        }

        /// <summary>
        /// Open file location in Explorer
        /// </summary>
        /// <param name="translated"></param>
        internal void ShowInExplorer(bool translated = false)
        {
            var path = translated ? settingsFile.TranslatedIniPath : settingsFile.EnglishIniPath;
            if (string.IsNullOrWhiteSpace(path)) return;
            try
            {
                Process.Start("explorer.exe", $"/select, \"{path}\"");
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error opening explorer: {ex.Message}");
            }
        }

        internal async Task Reload()
        {
            await UpdateTranslationsAsync(settingsFile.EnglishIniPath, settingsFile.TranslatedIniPath);
        }

        internal void TranslateSelectedItems(IList selectedItems)
        {

        }

        internal static void PasteFromClipboard(IList selectedItems)
        {
            if (selectedItems is null) return;
            if (selectedItems.Count == 0) return;

            var clipboardText = Clipboard.GetText();
            var lines = clipboardText.Split(separator, StringSplitOptions.None);
            // remove empty lines
            lines = lines.Where(line => !string.IsNullOrWhiteSpace(line)).ToArray();
            // if a line has no '=' character, put the line at the end of the previous line
            var cleanLines = new List<string>();
            for (int i = 0; i < lines.Length; i++)
            {
                if (lines[i].Contains('='))
                {
                    cleanLines.Add(lines[i]);
                }
                else if (cleanLines.Count > 0)
                {
                    cleanLines[^1] += lines[i];
                }
            }
            foreach (var line in cleanLines)
            {
                var parts = line.Split('=');
                if (parts.Length != 2) continue;

                var key = parts[0];
                var value = parts[1].Trim();
                foreach (Translations item in selectedItems)
                {
                    if (item.Key == key)
                    {
                        item.Translation = value;
                    }
                }
            }
        }

        internal static void CopyFromEnglish(IList selectedItems)
        {
            if (selectedItems is null) return;
            if (selectedItems.Count == 0) return;
            foreach (var item in selectedItems)
            {
                if (item is Translations translation)
                {
                    translation.Translation = translation.Value;
                }
            }
        }

        internal static void CopySelectedItemsToClipboard(IList selectedItems)
        {
            if (selectedItems is null) return;
            if (selectedItems.Count == 0) return;
            var sb = new StringBuilder();
            foreach (Translations item in selectedItems)
            {
                sb.AppendLine($"{item.Key}={item.Value}");
            }

            Clipboard.SetDataObject(sb.ToString());
        }

        internal void ReplaceSelectedItems(IList<Translations> selectedItems, string searchText, string replaceText)
        {
            if (selectedItems is null || selectedItems.Count == 0) return;

            // Compile the regex for better performance if using regex
            RegexOptions regexOptions = IgnoreCase ? RegexOptions.IgnoreCase : RegexOptions.None;
            Regex? regexPattern = Regex ? new Regex(searchText, regexOptions | RegexOptions.Compiled) : null;

            foreach (var item in selectedItems)
            {
                item.Translation = Regex
                    ? ReplaceWithRegex(item.Translation ?? string.Empty, regexPattern!, replaceText)
                    : ReplaceWithStringComparison(item.Translation ?? string.Empty, searchText, replaceText);
            }
        }

        private static string ReplaceWithRegex(string input, Regex regex, string replaceText)
        {
            return regex.Replace(input, replaceText);
        }

        private string ReplaceWithStringComparison(string input, string searchText, string replaceText)
        {
            return input.Replace(searchText, replaceText, IgnoreCase ? StringComparison.OrdinalIgnoreCase : StringComparison.Ordinal);
        }

        internal async Task EqualizeFiles(string? oldFilePath)
        {
            if (string.IsNullOrWhiteSpace(oldFilePath)) return;
            if (string.IsNullOrWhiteSpace(settingsFile.EnglishIniPath) || string.IsNullOrWhiteSpace(settingsFile.TranslatedIniPath)) return;
            await FileEquilizer.AddMissingEntries(oldFilePath, settingsFile.EnglishIniPath, settingsFile.TranslatedIniPath);
            await Reload();
            string[] oldLines = await File.ReadAllLinesAsync(oldFilePath);
            var translationsDict = Translations.ToDictionary(t => t.Key);  // Create a dictionary for quick lookup

            foreach (var line in oldLines)
            {
                var index = line.IndexOf('=');  // Use IndexOf for better performance than Split
                if (index < 0) continue;  // Skip lines that do not contain '='

                var key = line.Substring(0, index);
                var value = line.Substring(index + 1).Trim();

                if (translationsDict.TryGetValue(key, out var translation))
                {
                    translation.OldValue = value;
                    //translation.Translation = translation.Value;  // Assuming translation.Value is already set
                }
            }


            Debug.WriteLine("Files equalized");
        }

        internal async Task LoadBackupAsync()
        {
            if (string.IsNullOrWhiteSpace(settingsFile.TranslatedIniPath)) return;
            var backupPath = settingsFile.TranslatedIniPath + ".bak";
            if (File.Exists(backupPath))
            {
                File.Copy(backupPath, settingsFile.TranslatedIniPath, true);
            }
            await Reload();
        }

        internal int GetNextChange(int current)
        {
            var next = current + 1;
            while (next < Translations.Count)
            {
                // if value and old value are not the same or translation is empty return the index
                if (Translations[next].Value != Translations[next].OldValue)
                {
                    return next;
                }
                next++;
            }
            return current;
        }
    }
}
