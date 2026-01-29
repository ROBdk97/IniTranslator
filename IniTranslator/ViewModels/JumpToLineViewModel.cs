using CommunityToolkit.Mvvm.ComponentModel;
namespace IniTranslator.ViewModels
{
    internal partial class JumpToLineViewModel : ObservableObject
    {
        [ObservableProperty]
        private int lineNumber = 1;

        public string Line
        {
            get => LineNumber.ToString();
            set
            {
                if (int.TryParse(value, out var parsed))
                {
                    LineNumber = parsed;
                }
                else
                {
                    LineNumber = 0;
                }
            }
        }

        public event EventHandler? RequestedClose;

        public void RequestClose()
        {
            RequestedClose?.Invoke(this, EventArgs.Empty);
        }
    }
}
