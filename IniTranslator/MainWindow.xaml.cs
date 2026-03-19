using IniTranslator.Helpers;
using IniTranslator.Models;
using IniTranslator.ViewModels;
using IniTranslator.Windows;
using System.ComponentModel;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

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


        /// <summary>
        /// Performs search filtering on the ListView based on the SearchText.
        /// </summary>
        private void PerformSearch()
        {
            string searchText = ViewModel.SearchText;
            CollectionView view = (CollectionView)CollectionViewSource.GetDefaultView(listView.ItemsSource);

            if (string.IsNullOrWhiteSpace(searchText))
            {
                view.Filter = null;
                view.Refresh();
                return;
            }

            bool useRegex = ViewModel.UseRegex;
            bool ignoreCase = ViewModel.IgnoreCase;
            StringComparison comparison = ignoreCase ? StringComparison.OrdinalIgnoreCase : StringComparison.Ordinal;

            Regex? regex = useRegex && !string.IsNullOrWhiteSpace(searchText)
                ? new Regex(searchText, ignoreCase ? RegexOptions.IgnoreCase : RegexOptions.None)
                : null;

            view.Filter = item =>
            {
                if (item is not Translations translation)
                    return false;

                bool Matches(string? input) =>
                    input != null && (useRegex ? regex!.IsMatch(input) : input.Contains(searchText, comparison));

                return Matches(translation.Key) ||
                       Matches(translation.Translation) ||
                       Matches(translation.Value);
            };

            view.Refresh();
        }

        /// <summary>
        /// Handles the SizeChanged event of the ListView to adjust column widths.
        /// </summary>
        private void ListView_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            AdjustColumnWidths();
        }

        /// <summary>
        /// Adjusts the widths of the GridView columns to fit the ListView.
        /// </summary>
        private void AdjustColumnWidths()
        {
            double totalWidth = listView.ActualWidth - 35; // Adjust for scrollbar and margins
            if (totalWidth <= 0)
                return;

            if (listView.View is not GridView gridView)
                return;

            double lineColumnWidth = gridView.Columns[0].ActualWidth;
            double keyColumnWidth = gridView.Columns[1].ActualWidth;
            totalWidth -= (lineColumnWidth + keyColumnWidth);

            if (totalWidth <= 0)
                return;

            double englishColumnWidth = totalWidth * 0.5;
            double translationColumnWidth = totalWidth * 0.5;

            gridView.Columns[2].Width = englishColumnWidth;
            gridView.Columns[3].Width = translationColumnWidth;
        }

        /// <summary>
        /// Attaches event handlers to listen for changes to GridViewColumn widths.
        /// </summary>
        private void AttachColumnWidthChangedHandler()
        {
            if (listView.View is not GridView gridView)
                return;

            foreach (var column in gridView.Columns)
            {
                DependencyPropertyDescriptor.FromProperty(GridViewColumn.WidthProperty, typeof(GridViewColumn))
                    .AddValueChanged(column, ColumnWidthChangedHandler);
            }
        }

        /// <summary>
        /// Detaches event handlers from GridViewColumn widths.
        /// </summary>
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

        /// <summary>
        /// Handles changes to GridViewColumn widths to adjust other columns accordingly.
        /// </summary>
        private void ColumnWidthChangedHandler(object? sender, EventArgs e)
        {
            if ((sender as GridViewColumn)?.Header?.ToString() == "Key")
            {
                AdjustColumnWidths();
            }
        }

        /// <summary>
        /// Handles the Closing event of the Window to prompt saving changes and persist window settings.
        /// </summary>
        private void Window_Closing(object sender, CancelEventArgs e)
        {
            // Persist window settings
            ViewModel.Settings.WindowTop = Top;
            ViewModel.Settings.WindowLeft = Left;
            ViewModel.Settings.WindowWidth = Width;
            ViewModel.Settings.WindowHeight = Height;
            ViewModel.Settings.WindowState = WindowState;
            ViewModel.Settings.SaveSettings();

            // Prompt user to save changes
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


        /// <summary>
        /// Handles changes in the Search TextBox to perform search filtering.
        /// </summary>
        private void SearchTextBox_TextChanged(object sender, TextChangedEventArgs? e)
        {
            PerformSearch();
        }

        /// <summary>
        /// Handles the Loaded event of the Window to apply saved window settings.
        /// </summary>
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            Width = ViewModel.Settings.WindowWidth;
            Height = ViewModel.Settings.WindowHeight;
            Top = ViewModel.Settings.WindowTop;
            Left = ViewModel.Settings.WindowLeft;
            WindowState = ViewModel.Settings.WindowState;
        }


        /// <summary>
        /// Opens a dialog to jump to a specific line in the ListView.
        /// </summary>
        private void JumpToLine()
        {
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
        /// <summary>
        /// Handles the Jump to Line menu click event to jump to a specific line in the ListView.
        /// </summary>
        private void JumpToLine_Click(object sender, RoutedEventArgs e)
        {
            JumpToLine();
        }
        private void SetLightTheme_Click(object sender, RoutedEventArgs e)
        {
            ViewModel.ApplyTheme(ThemeMode.Light);
        }

        private void SetDarkTheme_Click(object sender, RoutedEventArgs e)
        {
            ViewModel.ApplyTheme(ThemeMode.Dark);
        }

        private void SetSystemTheme_Click(object sender, RoutedEventArgs e)
        {
            ViewModel.ApplyTheme(ThemeMode.System);
        }

        private void SetNoneTheme_Click(object sender, RoutedEventArgs e)
        {
            ViewModel.ApplyTheme(ThemeMode.None);
        }


        /// <summary>
        /// Handles changes in the SearchBox to trigger search.
        /// </summary>
        private void SearchBoxChanged(object sender, RoutedEventArgs e)
        {
            SearchTextBox_TextChanged(sender, null);
        }

        /// <summary>
        /// Displays the Help dialog with usage and shortcut information.
        /// </summary>
        private void Help_Click(object sender, RoutedEventArgs e)
        {
            var helpWindow = new HelpWindow()
            {
                Owner = this
            };
            helpWindow.ShowDialog();
        }

        /// <summary>
        /// Opens the About dialog or navigates to the project's GitHub page.
        /// </summary>
        private void About_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo
                {
                    FileName = "https://github.com/ROBdk97/IniTranslator",
                    UseShellExecute = true
                });
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Unable to open the link: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                ViewModel.UpdateStatus("Error opening About link.");
            }
        }


        /// <summary>
        /// Opens the Settings window to allow user to modify application settings.
        /// </summary>
        private void OpenSettings_Click(object sender, RoutedEventArgs e)
        {
            var settingsWindow = new SettingsWindow(ViewModel.Settings)
            {
                Owner = this
            };
            if (settingsWindow.ShowDialog() == true)
            {
                ViewModel.LoadSettings();
            }
        }

        /// <summary>
        /// Jumps to the next change entry in the ListView.
        /// </summary>
        private void JumpNextChange_Click(object sender, RoutedEventArgs e)
        {
            int index = listView.SelectedIndex;
            if (index < 0) index = 0;

            int newIndex = ViewModel.GetNextChange(index);
            if (newIndex <= index || newIndex >= listView.Items.Count) return;

            listView.ScrollIntoView(listView.Items[newIndex]);
            listView.SelectedIndex = newIndex;
        }

        /// <summary>
        /// Jumps to the next missing placeholder entry in the ListView.
        /// </summary>
        private void JumpNextMissingPlaceholder_Click(object sender, RoutedEventArgs e)
        {
            int index = listView.SelectedIndex;
            if (index < 0) index = 0;

            int newIndex = ViewModel.GetNextMissingPlaceHolder(index);
            if (newIndex <= index || newIndex >= listView.Items.Count) return;

            listView.ScrollIntoView(listView.Items[newIndex]);
            listView.SelectedIndex = newIndex;
        }

        /// <summary>
        /// Handles the Exit menu click event to close the application.
        /// </summary>
        private void Exit_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }


        /// <summary>
        /// Handles the Replace menu click event to perform replace functionality.
        /// </summary>
        private void Replace_Click(object sender, RoutedEventArgs e)
        {
            // Open ReplaceDialog to get search and replace terms
            var replaceWindow = new Windows.ReplaceDialog
            {
                Owner = this
            };
            if (replaceWindow.ShowDialog() == true)
            {
                string searchText = replaceWindow.SearchText;
                string replaceText = replaceWindow.ReplaceText;
                var selectedItems = listView.SelectedItems.Cast<Translations>();
                ViewModel.ReplaceSelectedItems(selectedItems, searchText, replaceText);
            }
        }

        internal async void Open(string? v)
        {
            if (v is null)
                return;
            if (string.IsNullOrWhiteSpace(v))
                return;
            try
            {
                await ViewModel.Open(v).ConfigureAwait(true);
            }
            catch (Exception) { }
        }
    }
}
