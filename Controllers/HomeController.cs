using Google.Apis.Auth.OAuth2;
using Google.Cloud.Storage.V1;
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
        public IActionResult UploadImage(IFormFile uploadedFile)
        {
            ImageAnnotatorClient client = ImageAnnotatorClient.Create();
            byte[] bytes = GetByteArrayFromFile(uploadedFile);
            Image image = Image.FromBytes(bytes);
            
            IReadOnlyList<EntityAnnotation> textAnnotations = client.DetectText(image);

            foreach (EntityAnnotation text in textAnnotations)
            {
                Debug.WriteLine($"Description: {text.Description}");
            }

            HomeViewModel model = new HomeViewModel();
            model.Text = String.Join(Environment.NewLine, textAnnotations.Select(t => t.Description));
            model.Image = Convert.ToBase64String(bytes);

            return View("Index", model);
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        private byte[] GetByteArrayFromFile(IFormFile file)
        {
            using (var target = new MemoryStream())
            {
                file.CopyTo(target);
                return target.ToArray();
            }
        }
    }
}
