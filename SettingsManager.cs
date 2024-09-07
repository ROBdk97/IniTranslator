﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace IniTranslator
{
    internal static class SettingsManager
    {
        private static readonly JsonSerializerOptions JsonSerializerOptions = new JsonSerializerOptions
        {
            WriteIndented = true // Makes JSON more readable
        };


        internal static readonly string SettingsFilePath = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),  // %appdata% folder
            "ROBdk97",
            System.Reflection.Assembly.GetExecutingAssembly().GetName().Name,      // Dynamically gets the executable name
            "settings.json"
        );

        /// <summary>
        /// Save current settings to JSON asynchronously.
        /// </summary>
        internal static void SaveSettings(this SettingsFile settingsFile)
        {
            try
            {
                // check if the directory exists, if not create it
                string directory = Path.GetDirectoryName(SettingsFilePath);
                if (!Directory.Exists(directory))
                {
                    Directory.CreateDirectory(directory);
                }
                Debug.WriteLine("Saving settings");
                string json = JsonSerializer.Serialize(settingsFile, JsonSerializerOptions);
                File.WriteAllText(SettingsFilePath, json);
            }
            catch (IOException ex)
            {
                LogError($"I/O error saving settings: {ex.Message}");
            }
            catch (UnauthorizedAccessException ex)
            {
                LogError($"Access error saving settings: {ex.Message}");
            }
            catch (Exception ex)
            {
                LogError($"Unexpected error saving settings: {ex.Message}");
            }
        }

        /// <summary>
        /// Load settings from JSON asynchronously or create new settings if file is missing.
        /// </summary>
        /// <returns>Loaded or default settings.</returns>
        internal static SettingsFile LoadSettings()
        {
            if (!File.Exists(SettingsFilePath))
            {
                return CreateDefaultSettings();
            }

            try
            {
                string json = File.ReadAllText(SettingsFilePath);
                return JsonSerializer.Deserialize<SettingsFile>(json) ?? CreateDefaultSettings();
            }
            catch (JsonException ex)
            {
                LogError($"Error parsing JSON settings: {ex.Message}");
                return CreateDefaultSettings();
            }
            catch (IOException ex)
            {
                LogError($"I/O error loading settings: {ex.Message}");
                return CreateDefaultSettings();
            }
            catch (Exception ex)
            {
                LogError($"Unexpected error loading settings: {ex.Message}");
                return CreateDefaultSettings();
            }
        }

        /// <summary>
        /// Create default settings and save them to file asynchronously.
        /// </summary>
        /// <returns>New default settings.</returns>
        private static SettingsFile CreateDefaultSettings()
        {
            var defaultSettings = new SettingsFile();
            try
            {
                string json = JsonSerializer.Serialize(defaultSettings, JsonSerializerOptions);
                File.WriteAllText(SettingsFilePath, json);
            }
            catch (Exception ex)
            {
                LogError($"Error creating default settings: {ex.Message}");
            }
            return defaultSettings;
        }

        /// <summary>
        /// Logs error messages consistently.
        /// </summary>
        /// <param name="message">Error message to log.</param>
        private static void LogError(string message)
        {
            Debug.WriteLine(message);
        }
    }

}
