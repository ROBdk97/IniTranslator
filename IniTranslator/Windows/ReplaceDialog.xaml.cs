using System.Windows;

namespace IniTranslator.Windows
{
    /// <summary>
    /// Interaction logic for ReplaceDialog.xaml
    /// </summary>
    public partial class ReplaceDialog : Window
    {
        public string SearchText { get; private set; }
        public string ReplaceText { get; private set; }

        public ReplaceDialog()
        {
            SearchText = string.Empty;
            ReplaceText = string.Empty;
            InitializeComponent();
        }

        private void ReplaceAll_Click(object sender, RoutedEventArgs e)
        {
            SearchText = SearchTextBox.Text;
            ReplaceText = ReplaceTextBox.Text;

            // Optionally, add validation here (e.g., ensure SearchText is not empty)

            DialogResult = true;
            Close();
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }
    }
}