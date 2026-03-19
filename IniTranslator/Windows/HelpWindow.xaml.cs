using IniTranslator.ViewModels;
using System.Windows;

namespace IniTranslator.Windows
{
    /// <summary>
    /// Interaction logic for HelpWindow.xaml
    /// </summary>
    public partial class HelpWindow : Window
    {
        public HelpWindow()
        {
            DataContext = new HelpWindowViewModel();
            InitializeComponent();
        }
    }
}
