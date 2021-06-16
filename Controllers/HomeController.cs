using Google.Api.Gax.ResourceNames;
using Google.Apis.Auth.OAuth2;
using Google.Cloud.Storage.V1;
using Google.Cloud.Translate.V3;
using Google.Cloud.Vision.V1;
using ImageTranslator.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

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
        public IActionResult TranslateImage(IFormFile uploadedFile, string lang)
        {
            byte[] bytes = GetByteArrayFromFile(uploadedFile);
            Image image = Image.FromBytes(bytes);

            HomeViewModel model = new HomeViewModel();
            model.OriginalText = GetTextFromImage(image);
            model.TranslatedText = Translate(model.OriginalText, lang);
            model.Image = Convert.ToBase64String(bytes);

            return View("Index", model);
        }

        [HttpPost]
        public IActionResult TranslateImageViaUrl(string url, string lang)
        {
            Image image = Image.FetchFromUri(url);

            HomeViewModel model = new HomeViewModel();
            model.OriginalText = GetTextFromImage(image);
            model.TranslatedText = Translate(model.OriginalText, lang);
            model.Image = image.Content.ToString();

            return View("Index", model);
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        private string GetTextFromImage(Image image)
        {
            ImageAnnotatorClient client = ImageAnnotatorClient.Create();
            IReadOnlyList<EntityAnnotation> textAnnotations = client.DetectText(image);

            HomeViewModel model = new HomeViewModel();
            string text = String.Join(Environment.NewLine, textAnnotations.Select(t => t.Description));

            return text;
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
