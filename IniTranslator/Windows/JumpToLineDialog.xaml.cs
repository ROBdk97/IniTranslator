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
            // Check for modifier keys and allow typical shortcuts (Ctrl+A, Ctrl+C, Ctrl+V, etc.)
            if ((Keyboard.Modifiers & ModifierKeys.Control) == ModifierKeys.Control)
            {
                // Allow specific key combinations
                switch (e.Key)
                {
                    case Key.A: // Select All
                    case Key.C: // Copy
                    case Key.V: // Paste
                    case Key.X: // Cut
                    case Key.Z: // Undo
                    case Key.Y: // Redo
                        e.Handled = false; // Allow these keys
                        return;
                }
            }

            // Allow only numbers and backspace
            if ((e.Key < Key.D0 || e.Key > Key.D9) && (e.Key < Key.NumPad0 || e.Key > Key.NumPad9) && e.Key != Key.Back)
            {
                e.Handled = true;
            }

            // Handle Enter key separately
            if (e.Key == Key.Enter)
            {
                DialogResult = true;
            }
        }

    }
}
