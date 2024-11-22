using IniTranslator.Models;
using IniTranslator.ViewModels;
using System;
using System.Linq;
using System.Windows;

namespace IniTranslator.Windows
{
    /// <summary>
    /// Interaction logic for SettingsWindow.xaml
    /// </summary>
    public partial class SettingsWindow : Window
    {
        private readonly SettingsViewModel ViewModel;
        public SettingsWindow(SettingsFile settings)
        {
            ViewModel = new SettingsViewModel(settings);
            DataContext = ViewModel;

            InitializeComponent();
            GoogleTranslateApiKeyBox.Password = ViewModel.GoogleTranslateApiKey;
            DeepLApiKeyBox.Password = ViewModel.DeepLApiKey;
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            ViewModel.GoogleTranslateApiKey = GoogleTranslateApiKeyBox.Password;
            ViewModel.DeepLApiKey = DeepLApiKeyBox.Password;
            ViewModel.SaveSettings();
        }
    }
}
