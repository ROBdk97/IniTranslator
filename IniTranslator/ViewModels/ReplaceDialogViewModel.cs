using CommunityToolkit.Mvvm.ComponentModel;

namespace IniTranslator.ViewModels
{
    internal partial class ReplaceDialogViewModel : ObservableObject
    {
        [ObservableProperty]
        private string searchText = string.Empty;

        [ObservableProperty]
        private string replaceText = string.Empty;
    }
}
