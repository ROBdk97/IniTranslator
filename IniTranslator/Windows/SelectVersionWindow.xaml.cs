using IniTranslator.ViewModels;
using System.Windows;

namespace IniTranslator.Windows
{
    /// <summary>
    /// Interaction logic for SelectVersionWindow.xaml
    /// </summary>
    public partial class SelectVersionWindow : Window
    {
        private readonly SelectVersionViewModel _viewModel;

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
            _viewModel = new SelectVersionViewModel(versions);
            DataContext = _viewModel;
            InitializeComponent();
        }

        private void Ok_CommandExecuted(object sender, System.Windows.Input.ExecutedRoutedEventArgs e)
        {
            Accept();
        }

        private void Accept()
        {
            if (!string.IsNullOrWhiteSpace(_viewModel.SelectedVersion))
            {
                SelectedVersion = _viewModel.SelectedVersion;
                DialogResult = true;
                Close();
                return;
            }

            MessageBox.Show("Please select a version before proceeding.", "No Selection", MessageBoxButton.OK, MessageBoxImage.Warning);
        }
    }
}
