﻿using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ImageTranslator.Models
{
    public class HomeViewModel
    {
        public string Text { get; set; }
        public string Image { get; set; }
    }
}
