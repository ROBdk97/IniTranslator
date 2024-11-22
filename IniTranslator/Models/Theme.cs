using System;
using System.Linq;

namespace IniTranslator.Models
{
    public record Theme
    {
        public required string Name { get; set; }
        public required string Value { get; set; }
    }
}
