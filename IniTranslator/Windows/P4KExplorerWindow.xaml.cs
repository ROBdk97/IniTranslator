using IniTranslator.ViewModels;
using System.Windows;

namespace IniTranslator.Windows
{
    public partial class P4KExplorerWindow : Window
    {
        private readonly P4KExplorerViewModel _viewModel;

        public P4KExplorerWindow()
        {
            InitializeComponent();
            _viewModel = new P4KExplorerViewModel();
            DataContext = _viewModel;
            _viewModel.OpenArchiveCommand.Execute(null);
        }
    }
}
