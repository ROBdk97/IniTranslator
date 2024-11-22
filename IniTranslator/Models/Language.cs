using System;
using System.Linq;

namespace IniTranslator.Models
{
    public record Language
    {
        public required string Name { get; set; }
        public required string Code { get; set; }
    }
}
