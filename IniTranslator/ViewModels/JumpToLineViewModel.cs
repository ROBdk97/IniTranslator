using CommunityToolkit.Mvvm.ComponentModel;
namespace IniTranslator.ViewModels
{
    internal partial class JumpToLineViewModel : ObservableObject
    {
        [ObservableProperty]
        private readonly int lineNumber = 1;

        public string Line
        {
            get => LineNumber.ToString();
            set => LineNumber = int.TryParse(value, out var parsed) ? parsed : 0;
        }

        public event EventHandler? RequestedClose;

        public void RequestClose()
        {
            RequestedClose?.Invoke(this, EventArgs.Empty);
        }
    }
}
