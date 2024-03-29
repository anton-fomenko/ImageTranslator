﻿using Google.Api.Gax.ResourceNames;
using Google.Cloud.Translate.V3;
using Google.Cloud.Vision.V1;
using ImageTranslator.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;

namespace ImageTranslator.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
            return View(new HomeViewModel());
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [HttpPost]
        public IActionResult TranslateImage(string url, IFormFile uploadedFile, string lang)
        {
            HomeViewModel model = new HomeViewModel();

            if (url == null && uploadedFile == null)
            {
                model.ValidationMessage = "Please either provide the URL or upload the file from your computer.";
                
                return View("Index", model);
            }

            Image image = null;

            if (url != null)
            {
                image = Image.FetchFromUri(url);
                model.Image = image.Content.ToBase64();
            }     
            else
            {
                byte[] bytes = GetByteArrayFromFile(uploadedFile);
                image = Image.FromBytes(bytes);
                model.Image = Convert.ToBase64String(bytes);
            }

            EntityAnnotation annotation = GetTextAnnotationFromImage(image);

            model.OriginalText = annotation.Description;
            model.OriginalLanguage = new CultureInfo(annotation.Locale).EnglishName;
            model.TranslatedText = Translate(model.OriginalText, lang);
            model.TranslatedLanguage = new CultureInfo(lang).EnglishName;

            return View("Index", model);
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        private EntityAnnotation GetTextAnnotationFromImage(Image image)
        {
            ImageAnnotatorClient client = ImageAnnotatorClient.Create();
            IReadOnlyList<EntityAnnotation> textAnnotations = client.DetectText(image);

            return textAnnotations.First();
        }

        private byte[] GetByteArrayFromFile(IFormFile file)
        {
            using (var target = new MemoryStream())
            {
                file.CopyTo(target);
                return target.ToArray();
            }
        }

        private string Translate(string input, string langCode)
        {
            TranslationServiceClient client = TranslationServiceClient.Create();
            TranslateTextRequest request = new TranslateTextRequest
            {
                Contents = { input },
                TargetLanguageCode = langCode,
                Parent = new ProjectName("glossy-calculus-316915").ToString()
            };
            TranslateTextResponse response = client.TranslateText(request);
            Translation translation = response.Translations[0];
            return translation.TranslatedText;
        }
    }
}
