using IniTranslator.Helpers;
using IniTranslator.Models;
using IniTranslator.Properties;
using IniTranslator.Windows;
using Microsoft.Win32;
using ROBdk97.Unp4k.P4kModels;
using System.Collections;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows;

namespace IniTranslator.ViewModels
{
    public partial class MainViewModel : BaseModel
    {
        private readonly ObservableCollection<Translations> _translations; // Internal collection to be displayed
        private SettingsFile _settings; // Holds settings information

        private string _searchText = string.Empty; // Search text for filtering _translations
        private string _status = Resources.Ready;
        private int _statusIndex = 0;
        private int _statusMax = 3;

        public bool IgnoreCase
        {
            get => Settings.IgnoreCase;
            set
            {
                if (Settings.IgnoreCase != value)
                {
                    Settings.IgnoreCase = value;
                    OnPropertyChanged(nameof(IgnoreCase));
                }
            }
        }

        public int StatusIndex
        {
            get => _statusIndex;
            set => SetProperty(ref _statusIndex, value);
        }

        public int StatusMax
        {
            get => _statusMax;
            set => SetProperty(ref _statusMax, value);
        }

        public bool UseRegex
        {
            get => Settings.UseRegex;
            set
            {
                if (Settings.UseRegex != value)
                {
                    Settings.UseRegex = value;
                    OnPropertyChanged(nameof(UseRegex));
                }
            }
        }

        public string SearchText
        {
            get => _searchText;
            set => SetProperty(ref _searchText, value);
        }

        public string Status
        {
            get => _status;
            set => SetProperty(ref _status, value);
        }

        public SettingsFile Settings => _settings;

        /// <summary>
        /// Expose the _translations collection for binding in the UI
        /// </summary>
        public ObservableCollection<Translations> Translations => _translations;

        private static readonly string[] Separator = ["\r\n"];

        public MainViewModel()
        {
            _settings = SettingsManager.LoadSettings(); // Load settings or create default
            _translations = [];
            UpdateStatus(Resources.Ready);
            InitializeAsync();
        }

        private async void InitializeAsync()
        {
            try
            {
                ResetStatus(2);
                await UpdateTranslationsAsync(Settings.EnglishIniPath, Settings.TranslatedIniPath).ConfigureAwait(true);
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error during initialization: {ex.Message}");
                UpdateStatus("Error during initialization");
            }
        }

        public void LoadSettings()
        {
            _settings = SettingsManager.LoadSettings(); // Load settings or create default
            OnPropertyChanged(nameof(Settings));
        }

        /// <summary>
        /// Update _translations from INI files asynchronously
        /// </summary>
        /// <param name="englishFilePath"></param>
        /// <param name="translatedFilePath"></param>
        /// <returns></returns>
        internal async Task<bool> UpdateTranslationsAsync(string? englishFilePath, string? translatedFilePath)
        {
            UpdateStatus("Updating translations...", false);

            if (string.IsNullOrWhiteSpace(englishFilePath) || string.IsNullOrWhiteSpace(translatedFilePath))
            {
                UpdateStatus("Invalid file paths provided.");
                return false;
            }

            Settings.EnglishIniPath = englishFilePath;
            Settings.TranslatedIniPath = translatedFilePath;

            var englishIniTask = ReadIniFileAsync(englishFilePath);
            var translatedIniTask = ReadIniFileAsync(translatedFilePath);

            await Task.WhenAll(englishIniTask, translatedIniTask);

            var englishIni = englishIniTask.Result;
            var translatedIni = translatedIniTask.Result;

            if (englishIni.Count == 0)
            {
                UpdateStatus("English INI file is empty.");
                return false;
            }
            if (translatedIni.Count == 0)
            {
                UpdateStatus("Translated INI file is empty.");
                return false;
            }

            // Ensure updates to ObservableCollection are on the UI thread
            Application.Current.Dispatcher.Invoke(() =>
            {
                _translations.Clear();
                int index = 1;
                foreach (var key in englishIni.Keys)
                {
                    _translations.Add(new Translations
                    {
                        Index = index++,
                        Key = key,
                        Value = englishIni[key],
                        Translation = translatedIni.TryGetValue(key, out var value) ? value : string.Empty
                    });
                }
            });

            OnPropertyChanged(nameof(Translations));
            UpdateStatus("Translations updated");
            return true;
        }

        private static async Task<Dictionary<string, string>> ReadIniFileAsync(string filePath)
        {
            try
            {
                var lines = await File.ReadAllLinesAsync(filePath, Encoding.UTF8);
                return lines
                    .Select(line => line.Split('=', 2))
                    .Where(parts => parts.Length == 2)
                    .ToDictionary(parts => parts[0].Trim(), parts => parts[1].Trim());
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
            if (Settings is null || string.IsNullOrWhiteSpace(Settings.TranslatedIniPath))
            {
                UpdateStatus("Translated INI path is not set.");
                return;
            }

            try
            {
                UpdateStatus("Saving translations");

                // Check for missing or mismatched placeholders before saving
                if (ContainsMissingPlaceHolder())
                {
                    // Display a message box to inform the user about the issue
                    MessageBox.Show(
                        Resources.MissingPlaceholdersDesc,
                        Resources.MissingPlaceholders,
                        MessageBoxButton.OK,
                        MessageBoxImage.Warning
                    );

                    //UpdateStatus("Missing placeholders found. Save aborted.");
                    //return; // Abort the save operation
                }

                var outputBuilder = new StringBuilder();
                foreach (var translation in Translations)
                {
                    outputBuilder.AppendLine($"{translation.Key}={translation.Translation}");
                }

                // Create a backup of the translated ini file
                var backupPath = $"{Settings.TranslatedIniPath}.bak";
                File.Copy(Settings.TranslatedIniPath, backupPath, overwrite: true);
                Debug.WriteLine($"Backup created at '{backupPath}'.");

                // Write the new translated ini file as UTF8 BOM
                File.WriteAllText(Settings.TranslatedIniPath, outputBuilder.ToString(), new UTF8Encoding(encoderShouldEmitUTF8Identifier: true));

                UpdateStatus("Translations saved successfully.");
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error saving translations: {ex.Message}");
                UpdateStatus("Error saving translations");
                MessageBox.Show($"An error occurred while saving translations: {ex.Message}", "Save Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        /// <summary>
        /// Open file location in Explorer
        /// </summary>
        /// <param name="translated"></param>
        internal void ShowInExplorer(bool translated = false)
        {
            var path = translated ? Settings.TranslatedIniPath : Settings.EnglishIniPath;
            if (string.IsNullOrWhiteSpace(path))
            {
                UpdateStatus("File path is not set.");
                return;
            }

            try
            {
                Process.Start(new ProcessStartInfo
                {
                    FileName = "explorer.exe",
                    Arguments = $"/select, \"{path}\"",
                    UseShellExecute = true,
                    CreateNoWindow = true
                });
                UpdateStatus("Opened in Explorer.");
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error opening explorer: {ex.Message}");
                UpdateStatus("Error opening explorer");
                MessageBox.Show($"Unable to open Explorer: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        internal async Task ReloadAsync()
        {
            UpdateStatus("Reloading translations...");
            ResetStatus(2);
            await UpdateTranslationsAsync(Settings.EnglishIniPath, Settings.TranslatedIniPath);
        }

        internal async Task TranslateSelectedItemsAsync(IEnumerable<Translations> selectedItems)
        {
            if (selectedItems == null || selectedItems.Count() == 0)
            {
                UpdateStatus("No items selected for translation.");
                return;
            }

            foreach (var item in selectedItems)
            {
                if (string.IsNullOrWhiteSpace(item.Value))
                    continue;

                try
                {
                    item.Translation = await TranslateAsync(item.Value);
                }
                catch (Exception ex)
                {
                    Debug.WriteLine($"Translation error for key '{item.Key}': {ex.Message}");
                    UpdateStatus($"Error translating key '{item.Key}'.");
                }
            }

            UpdateStatus("Selected items translated.");
        }

        /// <summary>
        /// Translate the given value using the selected translation provider.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        private async Task<string?> TranslateAsync(string value)
        {
            return Settings.TranslationProvider switch
            {
                TranslationProvider.GoogleTranslate => await Translators.GoogleTranslate(
                    value,
                    Settings.Language,
                    Settings.GoogleApiKey),
                TranslationProvider.DeepL => await Translators.DeepLTranslate(
                    value,
                    Settings.Language,
                    Settings.DeepLApiKey),
                _ => value,
            };
        }

        internal void PasteFromClipboard(IList selectedItems)
        {
            if (selectedItems == null || selectedItems.Count == 0)
            {
                UpdateStatus("No items selected to paste into.");
                return;
            }

            var clipboardText = Clipboard.GetText();
            if (string.IsNullOrWhiteSpace(clipboardText))
            {
                UpdateStatus("Clipboard is empty.");
                return;
            }

            var lines = clipboardText.Split(Separator, StringSplitOptions.None)
                                     .Where(line => !string.IsNullOrWhiteSpace(line))
                                     .ToArray();

            var cleanLines = new List<string>();
            foreach (var line in lines)
            {
                if (line.Contains('='))
                {
                    cleanLines.Add(line);
                }
                else if (cleanLines.Any())
                {
                    cleanLines[^1] += line;
                }
            }

            foreach (var line in cleanLines)
            {
                var parts = line.Split('=', 2);
                if (parts.Length != 2)
                    continue;

                var key = parts[0].Trim();
                var value = parts[1].Trim();

                foreach (Translations item in selectedItems)
                {
                    if (item.Key.Equals(key, StringComparison.OrdinalIgnoreCase))
                    {
                        item.Translation = value;
                    }
                }
            }

            UpdateStatus("Pasted translations from clipboard.");
        }

        internal void CopyFromEnglish(IList selectedItems)
        {
            if (selectedItems == null || selectedItems.Count == 0)
            {
                UpdateStatus("No items selected to copy from English.");
                return;
            }

            foreach (Translations item in selectedItems)
            {
                item.Translation = item.Value;
            }

            UpdateStatus("Copied English text to translations.");
        }

        internal void CopySelectedItemsToClipboard(IList selectedItems)
        {
            try
            {
                if (selectedItems == null || selectedItems.Count == 0)
                {
                    UpdateStatus("No items selected to copy.");
                    return;
                }

                var sb = new StringBuilder();
                foreach (Translations item in selectedItems)
                {
                    sb.AppendLine($"{item.Key}={item.Value}");
                }
                Application.Current.Dispatcher.Invoke(() => Clipboard.SetText(sb.ToString()));
                UpdateStatus("Copied selected items to clipboard.");
            } // Catch COMException when clipboard is locked (copy still works)
            catch (COMException) { }
            catch (Exception) { }

        }

        internal void ReplaceSelectedItems(IEnumerable<Translations> selectedItems, string searchText, string replaceText)
        {
            if (selectedItems == null || !selectedItems.Any())
            {
                UpdateStatus("No items selected for replacement.");
                return;
            }

            RegexOptions regexOptions = IgnoreCase ? RegexOptions.IgnoreCase | RegexOptions.Compiled : RegexOptions.Compiled;
            Regex? regexPattern = UseRegex && !string.IsNullOrWhiteSpace(searchText) ? new Regex(searchText, regexOptions) : null;

            foreach (var item in selectedItems)
            {
                if (string.IsNullOrEmpty(item.Translation))
                    continue;

                item.Translation = regexPattern != null
                    ? regexPattern.Replace(item.Translation, replaceText)
                    : ReplaceWithStringComparison(item.Translation, searchText, replaceText);
            }

            UpdateStatus("Replaced selected items.");
        }

        private string ReplaceWithStringComparison(string input, string searchText, string replaceText)
        {
            if (string.IsNullOrEmpty(searchText))
                return input;

            return IgnoreCase
                ? Regex.Replace(input, Regex.Escape(searchText), replaceText, RegexOptions.IgnoreCase)
                : input.Replace(searchText, replaceText, StringComparison.Ordinal);
        }

        /// <summary>
        /// Equalize the English and Translated INI files with an old INI file.
        /// </summary>
        /// <param name="oldFilePath"></param>
        /// <returns></returns>
        internal async Task EqualizeFilesAsync(string? oldFilePath)
        {
            if (string.IsNullOrWhiteSpace(oldFilePath))
            {
                UpdateStatus("Old file path is not provided.");
                return;
            }

            if (string.IsNullOrWhiteSpace(Settings.EnglishIniPath) ||
                string.IsNullOrWhiteSpace(Settings.TranslatedIniPath))
            {
                UpdateStatus("English or Translated INI path is not set.");
                return;
            }

            try
            {
                await FileEqualizer.AddMissingEntries(
                    oldFilePath,
                    Settings.EnglishIniPath,
                    Settings.TranslatedIniPath);

                await ReloadAsync();

                var oldLines = await File.ReadAllLinesAsync(oldFilePath);
                var translationsDict = Translations.ToDictionary(t => t.Key, t => t);

                foreach (var line in oldLines)
                {
                    var index = line.IndexOf('=');
                    if (index < 0)
                        continue;

                    var key = line.Substring(0, index).Trim();
                    var value = line.Substring(index + 1).Trim();

                    if (translationsDict.TryGetValue(key, out var translation))
                    {
                        translation.OldValue = value;
                        // Assuming translation.Value is already set
                    }
                }

                UpdateStatus("Files equalized");
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error equalizing files: {ex.Message}");
                UpdateStatus("Error equalizing files");
                MessageBox.Show($"An error occurred while equalizing files: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        internal async Task LoadBackupAsync()
        {
            if (string.IsNullOrWhiteSpace(Settings.TranslatedIniPath))
            {
                UpdateStatus("Translated INI path is not set.");
                return;
            }

            var backupPath = $"{Settings.TranslatedIniPath}.bak";
            if (!File.Exists(backupPath))
            {
                UpdateStatus("Backup file does not exist.");
                return;
            }

            try
            {
                File.Copy(backupPath, Settings.TranslatedIniPath, overwrite: true);
                Debug.WriteLine($"Backup restored from '{backupPath}'.");
                await ReloadAsync();
                UpdateStatus("Backup loaded successfully.");
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error loading backup: {ex.Message}");
                UpdateStatus("Error loading backup");
                MessageBox.Show($"An error occurred while loading backup: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        internal int GetNextChange(int current)
        {
            if (!Translations.Any(t => !string.IsNullOrWhiteSpace(t.OldValue)))
            {
                MessageBox.Show("Please click on \"File\"->\"Open Old Ini File\" first to use this feature.");
                UpdateStatus("Old INI file not loaded.");
                return current;
            }

            for (int next = current + 1; next < Translations.Count; next++)
            {
                if (Translations[next].Value != Translations[next].OldValue)
                {
                    return next;
                }
            }

            UpdateStatus("No further changes found.");
            return current;
        }

        // 1. %[a-zA-Z0-9]{1,2} for % placeholders with 1-2 alphanumeric characters, no preceding character before %
        // 2. \[~\w+\(.*?\)\] for [~action(...)] placeholders enclosed in square brackets
        // 3. ~\w+\(.*?\) for ~action(...) placeholders without square brackets
        [GeneratedRegex(@"(?<!\S)%[a-zA-Z0-9]{1,2}|\[~\w+\(.*?\)\]|~\w+\(.*?\)", RegexOptions.Compiled)]
        internal static partial Regex PlaceholderRegex();

        /// <summary>
        /// Retrieves all translation entries with mismatched placeholders between Value and Translation.
        /// </summary>
        /// <returns>A list of indices where placeholders are mismatched.</returns>
        public List<int> GetEntriesWithMissingPlaceholders()
        {
            var mismatchedEntries = new List<int>();

            for (int i = 0; i < Translations.Count; i++)
            {
                var entry = Translations[i];

                // Skip entries with null or whitespace values
                if (string.IsNullOrWhiteSpace(entry?.Value))
                    continue;

                // Extract placeholders from the Value field
                var valuePlaceholders = ExtractPlaceholders(entry.Value);

                // Extract placeholders from the Translation field
                var translationPlaceholders = ExtractPlaceholders(entry.Translation ?? string.Empty);

                // Add the index to the result if placeholders are mismatched
                if (!valuePlaceholders.SetEquals(translationPlaceholders))
                {
                    mismatchedEntries.Add(i);
                }
            }

            return mismatchedEntries;
        }

        /// <summary>
        /// Gets the next missing placeholder entry starting from the given index.
        /// </summary>
        /// <param name="current">The current index to start searching from.</param>
        /// <returns>The index of the next mismatched placeholder, or -1 if none are found.</returns>
        public int GetNextMissingPlaceHolder(int current)
        {
            var mismatchedEntries = GetEntriesWithMissingPlaceholders();
            var next = mismatchedEntries.FirstOrDefault(index => index > current, -1);
            if (next == -1)
                UpdateStatus("No further mismatched placeholders found.");
            return next;
        }

        /// <summary>
        /// Determines whether there are any _translations with missing or mismatched placeholders.
        /// </summary>
        /// <returns>True if any missing placeholders are found; otherwise, false.</returns>
        public bool ContainsMissingPlaceHolder() => GetEntriesWithMissingPlaceholders().Count != 0;

        /// <summary>
        /// Extracts placeholders from a string using the specified regex pattern.
        /// </summary>
        /// <param name="input">The input string to analyze.</param>
        /// <returns>A HashSet of normalized placeholders.</returns>
        private static HashSet<string> ExtractPlaceholders(string input)
        {
            return PlaceholderRegex().Matches(input)
                                      .Select(m => m.Value.Trim())
                                      .ToHashSet(StringComparer.OrdinalIgnoreCase);
        }

        /// <summary>
        /// Extract the English global.ini from game files.
        /// </summary>
        internal async Task ExtractFromGameAsync()
        {
            ResetStatus(3);
            if (string.IsNullOrWhiteSpace(Settings.StarCitizenPath) || !Directory.Exists(Settings.StarCitizenPath))
            {
                Settings.StarCitizenPath = StarCitizenPathFinder.GetStarCitizenPath();
            }
            if (string.IsNullOrWhiteSpace(Settings.StarCitizenPath))
            {
                MessageBox.Show("Star Citizen installation path not found. Please set it manually in the settings.", "Path Not Found", MessageBoxButton.OK, MessageBoxImage.Warning);
                UpdateStatus("Star Citizen path not found.");
                return;
            }
            var path = Settings.StarCitizenPath;
            path = Path.GetDirectoryName(path) ?? path;
            var subDirs = Directory.GetDirectories(path);

            var selectVersionWindow = new SelectVersionWindow(subDirs);
            if (selectVersionWindow.ShowDialog() != true)
            {
                UpdateStatus("Version selection canceled.");
                return;
            }

            var selectedVersion = selectVersionWindow.SelectedVersion;
            if (string.IsNullOrWhiteSpace(selectedVersion))
            {
                UpdateStatus("No version selected.");
                return;
            }

            var dialog = new SaveFileDialog
            {
                Filter = "INI files (*.ini)|*.ini|All files (*.*)|*.*",
                FileName = "global.ini",
                InitialDirectory = Path.GetDirectoryName(Settings.EnglishIniPath) ?? Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments),
            };

            if (dialog.ShowDialog() != true)
            {
                UpdateStatus("Save dialog canceled.");
                return;
            }

            var archivePath = Path.Combine(selectedVersion, "Data.p4k");
            if (!File.Exists(archivePath))
            {
                MessageBox.Show($"Archive file not found at '{archivePath}'.", "File Not Found", MessageBoxButton.OK, MessageBoxImage.Error);
                UpdateStatus("Data.p4k archive not found.");
                return;
            }

            try
            {
                var archive = new P4KArchive(archivePath);
                await archive.LoadAsync().ConfigureAwait(true);

                // Data/Localization/english/global.ini
                const string iniPathInArchive = "Data/Localization/english/global.ini";
                var file = archive.FindFiles("global.ini").FirstOrDefault(x => x.FullPath.Equals(iniPathInArchive, StringComparison.OrdinalIgnoreCase)) ?? throw new FileNotFoundException("global.ini not found in the archive.");

                if (File.Exists(dialog.FileName))
                {
                    File.Delete(dialog.FileName);
                }
                using var stream = file.Open();
                using var fileStream = File.Create(dialog.FileName);
                await stream.CopyToAsync(fileStream);
                fileStream.Close();

                await UpdateTranslationsAsync(dialog.FileName, Settings.TranslatedIniPath);

                UpdateStatus("global.ini extracted successfully.");
                Settings.SaveSettings();
            }
            catch (Exception ex)
            {
                UpdateStatus($"Error extracting global.ini: {ex.Message}");
                MessageBox.Show($"An error occurred while extracting global.ini: {ex.Message}", "Extraction Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        /// <summary>
        /// Updates the status property on the UI thread.
        /// </summary>
        /// <param name="message">The status message to set.</param>
        public void UpdateStatus(string message, bool reset = true)
        {
            StatusIndex++;
            Debug.WriteLine(message);
            Application.Current.Dispatcher.Invoke(() => Status = message);
            if (reset)
                ResetStatus();
        }

        public void ResetStatus(int max = 1)
        {
            StatusIndex = 0;
            StatusMax = max;
        }

        internal async Task Open(string v)
        {
            // check if it is in the english folder
            if (v.Contains("english"))
                Settings.EnglishIniPath = v;
            else
                Settings.TranslatedIniPath = v;
            try
            {
                await ReloadAsync().ConfigureAwait(true);
            }
            catch (Exception) { }
        }
    }
}
