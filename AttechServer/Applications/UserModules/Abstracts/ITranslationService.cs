namespace AttechServer.Applications.UserModules.Abstracts
{
    public interface ITranslationService
    {
        Task<string> TranslateTextAsync(string text, string sourceLanguage, string targetLanguage);
    }
}
