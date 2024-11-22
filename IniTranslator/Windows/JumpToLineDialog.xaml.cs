using System;
using System.Linq;
using System.Windows;
using System.Windows.Input;

namespace IniTranslator.Windows
{
    /// <summary>
    /// Interaction logic for JumpToLineDialog.xaml
    /// </summary>
    public partial class JumpToLineDialog : Window
    {
        public JumpToLineDialog()
        {
            DataContext = this;
            InitializeComponent();
        }

        private int _lineNumber;
        public string Line
        {
            get => _lineNumber.ToString();
            set
            {
                if (int.TryParse(value, out int lineNumber) && lineNumber > 0)
                {
                    _lineNumber = lineNumber;
                }
                else
                {
                    _lineNumber = lineNumber;
                }
            }
        }
        public int LineNumber => _lineNumber;

        private void Jump_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
        }

        private void Window_Activated(object sender, EventArgs e)
        {
            // focus the textbox when the window is activated
            JumpToLineTextBox.Focus();
            JumpToLineTextBox.SelectAll();
        }

        private void JumpToLineTextBox_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            // allow only numbers and backspace
            if (e.Key < Key.D0 || e.Key > Key.D9)
            {
                if (e.Key < Key.NumPad0 || e.Key > Key.NumPad9)
                {
                    if (e.Key != Key.Back)
                    {
                        e.Handled = true;
                    }
                }
            }
            // DialogResult = true when Enter key is pressed
            if (e.Key == Key.Enter)
            {
                DialogResult = true;
            }
        }
    }
}
