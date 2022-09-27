using TranslatorExtension.CognetiveService.Model;

namespace TranslatorExtension.CognetiveService
{
    public interface ITranslationService
    {
        Task<string> TranslateToEnglishAsync(string textToTranslate, string targetLanguage);
    }
}