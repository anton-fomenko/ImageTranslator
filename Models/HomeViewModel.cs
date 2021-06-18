namespace ImageTranslator.Models
{
    public class HomeViewModel
    {
        public string OriginalText { get; set; }
        public string TranslatedText { get; set; }
        public string Image { get; set; }
        public string ValidationMessage { get; set; }
        public string TranslatedLanguage { get; set; }
        public string OriginalLanguage { get; set; }
    }
}
