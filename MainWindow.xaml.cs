using IniTranslator.Helpers;
using IniTranslator.Models;
using IniTranslator.ViewModels;
using IniTranslator.Windows;
using Microsoft.Win32;
using System.ComponentModel;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;

namespace IniTranslator
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly MainViewModel ViewModel;

        public MainWindow()
        {
            ViewModel = new MainViewModel();
            DataContext = ViewModel;
            InitializeComponent();
            AttachColumnWidthChangedHandler();
        }

        private async void Open_ClickAsync(object sender, RoutedEventArgs e)
        {
            var englishFilePath = ShowOpenFileDialog("Select English INI File");
            if (string.IsNullOrWhiteSpace(englishFilePath))
                return;
            var translatedFilePath = ShowOpenFileDialog("Select Translated INI File");
            if (string.IsNullOrWhiteSpace(translatedFilePath))
                return;
            await ViewModel.UpdateTranslationsAsync(englishFilePath, translatedFilePath);
        }

        private static string? ShowOpenFileDialog(string title)
        {
            var openFileDialog = new OpenFileDialog { Filter = "Ini files (*.ini)|*.ini", Title = title };
            return openFileDialog.ShowDialog() == true ? openFileDialog.FileName : null;
        }

        private void PerformSearch()
        {
            string searchText = ViewModel.SearchText;
            CollectionView view = (CollectionView)CollectionViewSource.GetDefaultView(listView.ItemsSource);
            // Clear existing filter if search text is empty
            if (string.IsNullOrWhiteSpace(searchText))
            {
                view.Filter = null;
                view.Refresh();
                return;
            }

            bool useRegex = ViewModel.Regex;
            bool ignoreCase = ViewModel.IgnoreCase;
            var comparison = ignoreCase ? StringComparison.OrdinalIgnoreCase : StringComparison.Ordinal;

            // Initialize regex only if needed
            Regex? regex = useRegex ? new Regex(searchText, ignoreCase ? RegexOptions.IgnoreCase : RegexOptions.None) : null;

            // Apply the filter
            view.Filter = item =>
            {
                if (item is not Translations translation)
                    return false;

                // Local function to handle regex or string comparison
                bool Matches(string? input) =>
                    input != null && (useRegex ? regex!.IsMatch(input) : input.Contains(searchText, comparison));

                return Matches(translation.Key) ||
                       Matches(translation.Translation) ||
                       Matches(translation.Value);
            };

            // Refresh the CollectionView to update the ListView
            view.Refresh();
        }

        private void ListView_SizeChanged(object sender, SizeChangedEventArgs e) { AdjustColumnWidths(); }

        private void AdjustColumnWidths()
        {
            double totalWidth = listView.ActualWidth - 35; // Subtract padding for scroll bars and margins
            if (totalWidth <= 0)
                return;

            var gridView = (GridView)listView.View;
            double lineColumnWidth = gridView.Columns[0].ActualWidth;
            totalWidth -= lineColumnWidth;
            double keyColumnWidth = gridView.Columns[1].ActualWidth;
            totalWidth -= keyColumnWidth;
            if (totalWidth <= 0)
                return;
            double englishColumnWidth = totalWidth * 0.5;
            double translationColumnWidth = totalWidth * 0.5;

            gridView.Columns[2].Width = englishColumnWidth;
            gridView.Columns[3].Width = translationColumnWidth;
        }

        private void AttachColumnWidthChangedHandler()
        {
            if (listView.View is not GridView gridView)
                return;

            foreach (var column in gridView.Columns)
            {
                // Attach event to listen for changes to the Width property
                DependencyPropertyDescriptor.FromProperty(GridViewColumn.WidthProperty, typeof(GridViewColumn))
                    .AddValueChanged(column, ColumnWidthChangedHandler);
            }
        }

        private void ColumnWidthChangedHandler(object? sender, EventArgs e)
        {
            if ((sender as GridViewColumn)?.Header?.ToString() == "Key")
            {
                AdjustColumnWidths();
            }
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            ViewModel.Settings.WindowTop = Top;
            ViewModel.Settings.WindowLeft = Left;
            ViewModel.Settings.WindowWidth = Width;
            ViewModel.Settings.WindowHeight = Height;
            ViewModel.Settings.WindowState = WindowState;
            ViewModel.Settings.SaveSettings();
            // Ask user to save changes before closing
            var result = MessageBox.Show(
                "There are unsaved changes. Do you want to save before closing?",
                "Unsaved Changes",
                MessageBoxButton.YesNoCancel,
                MessageBoxImage.Warning);
            if (result == MessageBoxResult.Yes)
            {
                ViewModel.Save();
            }
            else if (result == MessageBoxResult.Cancel)
            {
                e.Cancel = true;
            }
            DetachColumnWidthChangedHandler();
        }

        private void DetachColumnWidthChangedHandler()
        {
            if (listView.View is not GridView gridView)
                return;

            foreach (var column in gridView.Columns)
            {
                DependencyPropertyDescriptor.FromProperty(GridViewColumn.WidthProperty, typeof(GridViewColumn))
                    .RemoveValueChanged(column, ColumnWidthChangedHandler);
            }
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e) { ViewModel.Save(); }

        private void ShowEnInExplorer_Click(object sender, RoutedEventArgs e) { ViewModel.ShowInExplorer(); }

        private void ShowTrInExplorer_Click(object sender, RoutedEventArgs e) { ViewModel.ShowInExplorer(true); }

        private void ListView_KeyDown(object sender, KeyEventArgs e)
        {
            // Control + C = Copy to Clipboard
            if (e.Key == Key.C && (Keyboard.Modifiers & ModifierKeys.Control) == ModifierKeys.Control)
            {
                MainViewModel.CopySelectedItemsToClipboard(listView.SelectedItems);
                e.Handled = true; // Mark the event as handled
            }
            // Control + V = Paste from Clipboard
            if (e.Key == Key.V && (Keyboard.Modifiers & ModifierKeys.Control) == ModifierKeys.Control)
            {
                MainViewModel.PasteFromClipboard(listView.SelectedItems);
                e.Handled = true; // Mark the event as handled
            }
            // Control + M = Copy from English
            if (e.Key == Key.M && (Keyboard.Modifiers & ModifierKeys.Control) == ModifierKeys.Control)
            {
                CopyFromEnglish_Click(sender, e);
                e.Handled = true; // Mark the event as handled
            }
            // Control + T = Use  Translator API
            if (e.Key == Key.T && (Keyboard.Modifiers & ModifierKeys.Control) == ModifierKeys.Control)
            {
                ViewModel.TranslateSelectedItemsAsync(listView.SelectedItems);
                e.Handled = true; // Mark the event as handled
            }
            // Control + J = Jump to Line
            if (e.Key == Key.J && (Keyboard.Modifiers & ModifierKeys.Control) == ModifierKeys.Control)
            {
                JumpToLine();
                e.Handled = true; // Mark the event as handled
            }
        }

        private void Copy_Click(object sender, RoutedEventArgs e)
        { MainViewModel.CopySelectedItemsToClipboard(listView.SelectedItems); }

        private void Paste_Click(object sender, RoutedEventArgs e)
        { MainViewModel.PasteFromClipboard(listView.SelectedItems); }

        private async void Reload_Click(object sender, RoutedEventArgs e) { await ViewModel.Reload(); }

        private void SearchTextBox_TextChanged(object sender, TextChangedEventArgs? e) { PerformSearch(); }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            Width = ViewModel.Settings.WindowWidth;
            Height = ViewModel.Settings.WindowHeight;
            Top = ViewModel.Settings.WindowTop;
            Left = ViewModel.Settings.WindowLeft;
            WindowState = ViewModel.Settings.WindowState;
        }

        private void CopyFromEnglish_Click(object sender, RoutedEventArgs e)
        { MainViewModel.CopyFromEnglish(listView.SelectedItems); }

        private void MenuItem_Click(object sender, RoutedEventArgs e)
        { ViewModel.TranslateSelectedItemsAsync(listView.SelectedItems); }

        private void JumpToLine()
        {
            // Open a dialog to get the line number
            var dialog = new JumpToLineDialog
            {
                Owner = this
            };
            if (dialog.ShowDialog() == true)
            {
                int line = dialog.LineNumber;

                // Clamp the line number to a valid range
                line = Math.Max(1, Math.Min(line, listView.Items.Count));

                // Scroll to the specified line and select it
                listView.ScrollIntoView(listView.Items[line - 1]);
                listView.SelectedIndex = line - 1;
            }
        }

        private void SearchBoxChanged(object sender, RoutedEventArgs e)
        {
            SearchTextBox_TextChanged(sender, null);
        }

        private void Help_Click(object sender, RoutedEventArgs e)
        {
            // Show shortcuts box with help text for shortcuts and general usage
            string general = "General Usage:\n" +
                            "- Open English and Translated INI files\n" +
                            "- Select items to translate\n" +
                            "- Use shortcuts to copy, paste, translate, etc.\n" +
                            "- Select Old INI file to compare changes\n" +
                            "- Jump to next change in the list\n" +
                            "- Save changes to the translated INI file\n";
            string shortcuts = "Keyboard Shortcuts:\n" +
                             "Ctrl + C:\tCopy selected items to clipboard\n" +
                             "Ctrl + V:\tPaste clipboard contents to selected items\n" +
                             "Ctrl + M:\tCopy English values to translation\n" +
                             "Ctrl + T:\tUse Translator API to translate selected items\n" +
                             "Ctrl + J:\tJump to line\n";
            MessageBox.Show($"{general}\n{shortcuts}", "Help", MessageBoxButton.OK, MessageBoxImage.Information);

        }

        private void About_Click(object sender, RoutedEventArgs e)
        {
            System.Diagnostics.Process.Start("explorer", "https://github.com/ROBdk97/IniTranslator");
        }

        private async void OpenOldIni_Click(object sender, RoutedEventArgs e)
        {
            string? oldFilePath = ShowOpenFileDialog("Select Old INI File");
            await ViewModel.EqualizeFiles(oldFilePath);
        }

        private async void LoadBackup_Click(object sender, RoutedEventArgs e)
        {
            await ViewModel.LoadBackupAsync();
        }

        private void JumpToNextChange(object sender, RoutedEventArgs e)
        {
            // get current selected index
            int index = listView.SelectedIndex;
            if(index < 0) index = 0;
            // if no item is selected, jump to the first change
            var newIndex = ViewModel.GetNextChange(index);
            if (newIndex <= index) return;

            // Scroll to the specified line and select it
            listView.ScrollIntoView(listView.Items[newIndex]);
            listView.SelectedIndex = newIndex;
        }

        private void OpenSettings_Click(object sender, RoutedEventArgs e)
        {
            var settingsWindow = new SettingsWindow(ViewModel.Settings)
            {
                Owner = this
            };
            settingsWindow.ShowDialog();
            ViewModel.LoadSettings();
        }
    }
}