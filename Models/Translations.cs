using IniTranslator.ViewModels;
using System;
using System.Linq;

namespace IniTranslator.Models
{
    public class Translations : BaseModel
    {
        // key cant be null or empty
        private string _key = string.Empty;
        private string? _value;
        private string? _oldValue;
        private string? _translation;
        private int _index;

        public string Key
        {
            get => _key;
            set => SetProperty(ref _key, value);
        }

        public string? Value
        {
            get => _value;
            set => SetProperty(ref _value, value);
        }

        public string? OldValue
        {
            get => _oldValue;
            set => SetProperty(ref _oldValue, value);
        }

        public string? Translation
        {
            get => _translation;
            set => SetProperty(ref _translation, value);
        }

        public int Index
        {
            get => _index;
            set => SetProperty(ref _index, value);
        }
    }

}
