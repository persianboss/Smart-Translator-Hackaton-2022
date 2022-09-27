namespace TranslatorExtension.CognetiveService.Model
{
    public class TranslationModel
    {
        public DetectedLanguage DetectedLanguage { get; set; }
        public Translation[] Translations { get; set; }
    }

    public class DetectedLanguage
    {
        public string Language { get; set; }
        public string Score { get; set; }
    }

    public class Translation
    {
        public string Text { get; set; }
        public string To { get; set; }
    }
}
