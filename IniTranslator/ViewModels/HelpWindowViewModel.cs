using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Diagnostics;
using System.Windows;

namespace IniTranslator.ViewModels
{
    internal partial class HelpWindowViewModel : ObservableObject
    {
        [RelayCommand]
        private void OpenGitHub(Uri? uri)
        {
            string url = uri?.ToString() ?? string.Empty;
            if (string.IsNullOrWhiteSpace(url))
                return;

            try
            {
                Process.Start(new ProcessStartInfo(url) { UseShellExecute = true });
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Unable to open link: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
