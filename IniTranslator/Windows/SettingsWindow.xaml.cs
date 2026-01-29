using IniTranslator.Models;
using IniTranslator.ViewModels;
using IniTranslator.Helpers;
using System;
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

        private void Save_Click(object sender, RoutedEventArgs e)
        {
            ViewModel.SaveAllCommand.Execute(new PasswordPair
            {
                GoogleApiKey = GoogleTranslateApiKeyBox.Password,
                DeepLApiKey = DeepLApiKeyBox.Password
            });

            Close();
        }
    }
}
