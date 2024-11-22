using System;
using System.IO;
using System.Text.RegularExpressions;

public partial class StarCitizenPathFinder
{
    /// <summary>
    /// Retrieves the Star Citizen installation path.
    /// </summary>
    /// <returns>The installation path as a string.</returns>
    public static string? GetStarCitizenPath()
    {
        string defaultPath = @"C:\Program Files\Roberts Space Industries\StarCitizen";

        if (Directory.Exists(defaultPath))
            return defaultPath;

        Console.WriteLine("Default path not found. Attempting to retrieve from log file...");

        string logFilePath = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
            "rsilauncher", "logs", "log.log");

        if (File.Exists(logFilePath))
        {
            var extractedPath = ExtractPathFromLog(logFilePath);
            if (!string.IsNullOrWhiteSpace(extractedPath) && Directory.Exists(extractedPath))
                return extractedPath;
        }
        return null;
    }

    /// <summary>
    /// Extracts the Star Citizen path from the RSI Launcher log file.
    /// </summary>
    /// <param name="logFilePath">Path to the RSI Launcher log file.</param>
    /// <returns>The extracted path, or null if not found.</returns>
    private static string? ExtractPathFromLog(string logFilePath)
    {
        string[] lines = File.ReadAllLines(logFilePath);

        foreach (string line in lines)
        {
            var match = LaunchSC().Match(line);
            if (match.Success)
                return match.Groups[2].Value.Replace("\\\\", "\\");
        }

        return null;
    }

    [GeneratedRegex("Launching Star Citizen (PTU|LIVE) from \\(([^)]+)\\)")]
    private static partial Regex LaunchSC();
}
