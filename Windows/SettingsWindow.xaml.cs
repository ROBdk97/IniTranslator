using IniTranslator.Helpers;
using IniTranslator.Models;
using IniTranslator.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

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
