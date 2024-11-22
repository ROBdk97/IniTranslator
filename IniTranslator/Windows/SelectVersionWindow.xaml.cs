using System.Windows;

namespace IniTranslator.Windows
{
    /// <summary>
    /// Interaction logic for SelectVersionWindow.xaml
    /// </summary>
    public partial class SelectVersionWindow : Window
    {
        /// <summary>
        /// Gets the selected version.
        /// </summary>
        public string? SelectedVersion { get; private set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="SelectVersionWindow"/> class.
        /// </summary>
        /// <param name="versions">The list of versions to display.</param>
        public SelectVersionWindow(IEnumerable<string> versions)
        {
            InitializeComponent();
            VersionsListBox.ItemsSource = versions;
        }

        /// <summary>
        /// Handles the OK button click event.
        /// </summary>
        private void OkButton_Click(object sender, RoutedEventArgs e)
        {
            if (VersionsListBox.SelectedItem is string selected)
            {
                SelectedVersion = selected;
                DialogResult = true;
                Close();
            }
            else
            {
                MessageBox.Show("Please select a version before proceeding.", "No Selection", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }
    }
}
