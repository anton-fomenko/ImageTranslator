using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ImageTranslator.Models
{
    public class HomeViewModel
    {
        public string OriginalText { get; set; }
        public string TranslatedText { get; set; }
        public string Image { get; set; }
    }
}
