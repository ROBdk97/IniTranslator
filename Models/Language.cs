using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IniTranslator.Models
{
    public record Language
    {
        public required string Name { get; set; }
        public required string Code { get; set; }
    }
}
