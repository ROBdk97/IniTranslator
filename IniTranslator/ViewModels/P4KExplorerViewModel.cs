using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using IniTranslator.Helpers;
using IniTranslator.Windows;
using Microsoft.Win32;
using ROBdk97.Unp4k.P4kModels;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Windows;

namespace IniTranslator.ViewModels
{
    public partial class P4KExplorerViewModel : ObservableObject
    {
        [ObservableProperty]
        private P4KArchive? archive;

        [ObservableProperty]
        private P4KItem? selectedItem;

        [ObservableProperty]
        private string searchText = string.Empty;

        [ObservableProperty]
        private ObservableCollection<P4KItem> rootChildren = [];

        [ObservableProperty]
        private bool isLoading;

        [ObservableProperty]
        private string status = "Ready";

        [ObservableProperty]
        private string? selectedItemDetails = string.Empty;

        public bool CanExportSelectedItem => SelectedItem is P4KEntry or P4KDirectory;

        public string ExportButtonText => SelectedItem is P4KDirectory ? "Extract Directory" : "Export File";

        public P4KExplorerViewModel()
        {
        }

        [RelayCommand]
        private async Task OpenArchiveAsync()
        {
            var scPath = StarCitizenPathFinder.GetStarCitizenPath();
            if (string.IsNullOrWhiteSpace(scPath))
            {
                MessageBox.Show("Star Citizen installation path not found. Please set it manually in the settings.", "Path Not Found", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var path = Path.GetDirectoryName(scPath) ?? scPath;
            var subDirs = Directory.GetDirectories(path);

            var selectVersionWindow = new SelectVersionWindow(subDirs);
            if (selectVersionWindow.ShowDialog() != true)
                return;

            var selectedVersion = selectVersionWindow.SelectedVersion;
            if (string.IsNullOrWhiteSpace(selectedVersion))
                return;

            var archivePath = Path.Combine(selectedVersion, "Data.p4k");
            if (!File.Exists(archivePath))
                return;

            await LoadArchiveAsync(archivePath);
        }

        [RelayCommand]
        public async Task LoadArchiveAsync(string archivePath)
        {
            if (string.IsNullOrWhiteSpace(archivePath) || !File.Exists(archivePath))
            {
                Status = "Invalid archive path.";
                return;
            }

            IsLoading = true;
            Status = "Loading archive...";

            try
            {
                Archive = new P4KArchive(archivePath);
                await Archive.LoadAsync().ConfigureAwait(true);

                RootChildren.Clear();
                foreach (var item in Archive.Root.Children)
                {
                    RootChildren.Add(item);
                }

                Status = "Archive loaded successfully.";
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error loading archive: {ex.Message}");
                Status = $"Error loading archive: {ex.Message}";
            }
            finally
            {
                IsLoading = false;
            }
        }

        partial void OnSelectedItemChanged(P4KItem? value)
        {
            if (value == null)
            {
                SelectedItemDetails = string.Empty;
            }
            else
            {
                var details = new System.Text.StringBuilder();
                details.AppendLine($"Name: {value.Name}");
                details.AppendLine($"Full Path: {value.FullPath}");
                details.AppendLine($"Type: {(value.IsDirectory ? "Directory" : "File")}");

                if (value is P4KEntry entry)
                {
                    details.AppendLine($"Compression: {entry.CompressionMethod}");
                    details.AppendLine($"Is CryXML: {entry.IsCryXmlB}");
                    details.AppendLine($"Is DataCore: {entry.IsDataCore}");
                }

                if (value.IsDirectory)
                {
                    details.AppendLine($"Child Items: {value.Children.Count}");
                }

                SelectedItemDetails = details.ToString();
            }

            OnPropertyChanged(nameof(CanExportSelectedItem));
            OnPropertyChanged(nameof(ExportButtonText));
            ExportSelectedCommand.NotifyCanExecuteChanged();
        }

        [RelayCommand(CanExecute = nameof(CanExportSelectedItem))]
        private void ExportSelected()
        {
            if (SelectedItem is P4KDirectory directory)
            {
                ExtractDirectory(directory);
                return;
            }

            if (SelectedItem is P4KEntry entry)
            {
                ExportFile(entry);
            }
        }

        [RelayCommand]
        private void ExportFile(P4KEntry? entry)
        {
            if (entry == null)
            {
                Status = "No file selected.";
                return;
            }

            var dialog = new SaveFileDialog
            {
                FileName = entry.Name,
                InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.Desktop),
            };

            if (dialog.ShowDialog() != true)
                return;

            try
            {
                using var sourceStream = entry.Open();
                using var targetStream = File.Create(dialog.FileName);
                sourceStream.CopyTo(targetStream);
                Status = $"File exported to {dialog.FileName}";
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error exporting file: {ex.Message}");
                Status = $"Error exporting file: {ex.Message}";
            }
        }

        [RelayCommand]
        private void ExtractDirectory(P4KDirectory? directory)
        {
            if (directory == null)
            {
                Status = "No directory selected.";
                return;
            }

            var dialog = new OpenFolderDialog
            {
                Title = "Select extraction location",
            };

            if (dialog.ShowDialog() != true)
                return;

            try
            {
                var destinationRoot = Path.Combine(dialog.FolderName, directory.Name);
                Directory.CreateDirectory(destinationRoot);
                ExtractDirectoryRecursive(directory, destinationRoot);
                Status = $"Directory extracted to {destinationRoot}";
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error extracting directory: {ex.Message}");
                Status = $"Error extracting directory: {ex.Message}";
            }
        }

        private static void ExtractDirectoryRecursive(P4KDirectory sourceDirectory, string destinationPath)
        {
            foreach (var child in sourceDirectory.Children)
            {
                if (child is P4KDirectory subDirectory)
                {
                    var subDirectoryPath = Path.Combine(destinationPath, subDirectory.Name);
                    Directory.CreateDirectory(subDirectoryPath);
                    ExtractDirectoryRecursive(subDirectory, subDirectoryPath);
                    continue;
                }

                if (child is P4KEntry fileEntry)
                {
                    var targetFilePath = Path.Combine(destinationPath, fileEntry.Name);
                    using var sourceStream = fileEntry.Open();
                    using var targetStream = File.Create(targetFilePath);
                    sourceStream.CopyTo(targetStream);
                }
            }
        }

        [RelayCommand]
        private void SearchArchive()
        {
            if (string.IsNullOrWhiteSpace(SearchText) || Archive == null)
            {
                Status = "Please enter a search term.";
                return;
            }

            try
            {
                var results = Archive.FindFiles(SearchText);
                RootChildren.Clear();
                foreach (var item in results)
                {
                    RootChildren.Add(item);
                }
                Status = $"Found {results.Count} matching files.";
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error searching archive: {ex.Message}");
                Status = $"Error searching archive: {ex.Message}";
            }
        }

        [RelayCommand]
        private void ClearSearch()
        {
            if (Archive == null)
                return;

            SearchText = string.Empty;
            RootChildren.Clear();
            foreach (var item in Archive.Root.Children)
            {
                RootChildren.Add(item);
            }
            Status = "Search cleared.";
        }
    }
}
