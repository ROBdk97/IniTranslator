using CommunityToolkit.Mvvm.ComponentModel;
using System.Collections.Generic;

namespace IniTranslator.ViewModels
{
    internal partial class SelectVersionViewModel : ObservableObject
    {
        public SelectVersionViewModel(IEnumerable<string> versions)
        {
            Versions = versions;
        }

        public IEnumerable<string> Versions { get; }

        [ObservableProperty]
        private string? selectedVersion;
    }
}
