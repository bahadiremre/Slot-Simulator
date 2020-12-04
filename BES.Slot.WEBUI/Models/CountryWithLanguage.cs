using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BES.Slot.WEBUI.Models
{
    public class CountryWithLanguage
    {
        public string Name { get; set; }
        public string NativeName { get; set; }
        public string Flag { get; set; }
        public List<Language> Languages { get; set; }
    }
}
