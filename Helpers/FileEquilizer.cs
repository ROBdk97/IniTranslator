using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace IniTranslator.Helpers
{
    internal static class FileEquilizer
    {
        public static async Task AddMissingEntries(string oldInputFilePath, string inputFilePath, string outputFilePath)
        {
            var readOldIn = Task.Run(() => File.ReadAllLinesAsync(oldInputFilePath));
            var readIn = Task.Run(() => File.ReadAllLinesAsync(inputFilePath));
            var readOut = Task.Run(() => File.ReadAllLinesAsync(outputFilePath));

            var OldFile1Lines = await readOldIn;
            var file1Lines = await readIn;
            var file2Lines = await readOut;

            Debug.WriteLine("Checking for duplicates in file 1");
            file1Lines = await CheckDuplicates(file1Lines);

            Debug.WriteLine("Checking for duplicates in file 2");
            file2Lines = await CheckDuplicates(file2Lines);

            var oldFile1Dict = OldFile1Lines.Select(line => line.Split('=', 2)).ToDictionary(split => split[0], split => split[1]);
            // get a dictionary of all keys and values from file1
            var file1Dict = file1Lines.Select(line => line.Split('=', 2)).ToDictionary(split => split[0], split => split[1]);
            // Check for changes between the old file and the new input file and use the new value
            var changes = file1Dict.Where(pair => oldFile1Dict.ContainsKey(pair.Key) && oldFile1Dict[pair.Key] != pair.Value).ToDictionary();
            Debug.WriteLine($"Found {changes.Count} changes between the old input file and the new input file");

            // get a dictionary of all keys and values from file2
            var file2Dict = file2Lines.Select(line => line.Split('=', 2)).ToDictionary(split => split[0], split => split[1]);

            // update file2Dict with missing keys from file1Dict
            UpdateTranslations(file1Dict, ref file2Dict, changes);

            // sort file2Dict to have the same order as file1Dict
            var orderedList = file1Dict.Keys
            .Where(file2Dict.ContainsKey) // Ensure the key exists in dict2
            .Select(key => new KeyValuePair<string, string>(key, file2Dict[key]))
            .ToList();

            // Create a string builder to build the output file
            var outputBuilder = new StringBuilder();
            // append key value pairs from file2Dict it a "=" as sepeartor
            foreach (var item in orderedList)
            {
                outputBuilder.AppendLine($"{item.Key}={item.Value}");
            }
            // write the output file
            await File.WriteAllTextAsync(outputFilePath, outputBuilder.ToString(), new UTF8Encoding(true));
        }

        // Check for duplicates and ask the user to choose one
        private static Task<string[]> CheckDuplicates(string[] lines)
        {
            var keyValuePairs = new Dictionary<string, List<string>>();

            // First pass to group values by keys
            foreach (var line in lines)
            {
                var parts = line.Split('=', 2);
                var key = parts[0];
                var value = parts.Length > 1 ? parts[1] : "";

                if (!keyValuePairs.ContainsKey(key))
                {
                    keyValuePairs[key] = new List<string>();
                }
                keyValuePairs[key].Add(value);
            }

            // User interaction for duplicates
            foreach (var pair in keyValuePairs)
            {
                if (pair.Value.Count > 1)
                {
                    // dont ask if they are the same
                    if (pair.Value.Distinct().Count() == 1)
                    {
                        keyValuePairs[pair.Key] = [pair.Value[0]];
                        continue;
                    }
                    var window = new Window();
                    var stackPanel = new StackPanel();
                    string title = $"Multiple values found for key '{pair.Key}'.";
                    window.Title = title;
                    stackPanel.Children.Add(new TextBlock { Text = "Chose a Value:" });
                    Debug.WriteLine($"Multiple values found for key '{pair.Key}'. Please choose one:");
                    for (int i = 0; i < pair.Value.Count; i++)
                    {
                        Debug.WriteLine($"{i + 1}: {pair.Value[i]}");
                        // Add a button for each value
                        var button = new Button
                        {
                            Content = pair.Value[i],
                            Tag = i + 1
                        };
                        button.Click += (sender, e) =>
                        {
                            window.Tag = (int)((Button)sender).Tag;
                            window.Close();
                        };
                        stackPanel.Children.Add(button);
                    }
                    window.Content = stackPanel;
                    window.ShowDialog();

                    int choice = (int)window.Tag;
                    if (choice <= 0)
                    {
                        choice = 1;
                    }
                    // Keep only the chosen value
                    keyValuePairs[pair.Key] = [pair.Value[choice - 1]];
                }
            }
            // Construct the final list of lines
            var finalLines = keyValuePairs.SelectMany(pair => pair.Value.Select(value => $"{pair.Key}={value}")).ToList();

            // Return the final list
            return Task.FromResult(finalLines.ToArray());
        }

        private static void UpdateTranslations(Dictionary<string, string> file1Dict, ref Dictionary<string, string> file2Dict, Dictionary<string, string> changes)
        {
            Debug.WriteLine("Updating translations");
            var keysToUpdate = new List<string>();

            foreach (var item in file1Dict)
            {
                string key = item.Key;
                string keyVariant = key.EndsWith(",P") ? key.Remove(key.Length - 2) : key + ",P";

                if (file2Dict.ContainsKey(key))
                {
                    if (changes.ContainsKey(key))
                    {
                        keysToUpdate.Add(key);
                    }
                    else if (file2Dict.ContainsKey(keyVariant))
                    {
                        keysToUpdate.Add(keyVariant);
                    }
                    continue;
                }
                else if (file2Dict.ContainsKey(keyVariant))
                {
                    // Add to a list to update after iterating (to avoid modifying the collection while iterating)
                    keysToUpdate.Add(keyVariant);
                    Debug.WriteLine($"Using existing translation for {keyVariant}");
                }
                else
                {
                    // add the key and value to file2Dict
                    file2Dict[key] = item.Value;
                    Debug.WriteLine($"Adding new entry: \"{item.Key}={item.Value}\"");
                }
            }

            // Update keys in file2Dict
            foreach (var oldKey in keysToUpdate)
            {
                string newKey = oldKey.EndsWith(",P") ? oldKey.Remove(oldKey.Length - 2) : oldKey + ",P";
                if (file2Dict.TryGetValue(oldKey, out string? value))
                {
                    file2Dict.Remove(oldKey);
                    file2Dict[newKey] = value;
                }
                // Overwrite the value if it was changed
                if (changes.TryGetValue(oldKey, out string? value2))
                {
                    file2Dict[oldKey] = value2;
                }
            }
        }
    }
}
