using System;
using System.Windows;
using System.Windows.Input;
using IniTranslator.ViewModels;

namespace IniTranslator.Windows
{
    /// <summary>
    /// Interaction logic for JumpToLineDialog.xaml
    /// </summary>
    public partial class JumpToLineDialog : Window
    {
        private readonly JumpToLineViewModel _viewModel;

        public JumpToLineDialog()
        {
            _viewModel = new JumpToLineViewModel();
            DataContext = _viewModel;
            InitializeComponent();
        }

        public int LineNumber => _viewModel.LineNumber;

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

        private void Jump_CommandExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            DialogResult = true;
        }

    }
}
