using IniTranslator.ViewModels;
using System.Windows;
using System.Windows.Input;

namespace IniTranslator.Windows
{
    /// <summary>
    /// Interaction logic for ReplaceDialog.xaml
    /// </summary>
    public partial class ReplaceDialog : Window
    {
        public string SearchText { get; private set; }
        public string ReplaceText { get; private set; }

        private readonly ReplaceDialogViewModel _viewModel;

        public ReplaceDialog()
        {
            _viewModel = new ReplaceDialogViewModel();
            DataContext = _viewModel;

            SearchText = string.Empty;
            ReplaceText = string.Empty;
            InitializeComponent();
        }

        protected override void OnSourceInitialized(EventArgs e)
        {
            base.OnSourceInitialized(e);
            CommandBindings.Add(new CommandBinding(ApplicationCommands.Save, (_, __) => Accept()));
        }

        private void Accept()
        {
            SearchText = _viewModel.SearchText;
            ReplaceText = _viewModel.ReplaceText;

            DialogResult = true;
            Close();
        }
    }
}