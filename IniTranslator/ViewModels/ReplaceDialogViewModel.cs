using CommunityToolkit.Mvvm.ComponentModel;

namespace IniTranslator.ViewModels
{
    internal partial class ReplaceDialogViewModel : ObservableObject
    {
        [ObservableProperty]
        private readonly string searchText = string.Empty;

        [ObservableProperty]
        private readonly string replaceText = string.Empty;
    }
}
