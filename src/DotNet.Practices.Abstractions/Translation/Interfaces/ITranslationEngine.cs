namespace DotNet.Practices.Abstractions.Translation.Interfaces;

public interface ITranslationEngine
{
    Task<string> TranslateAsync(Language fromLanguage, Language toLanguage, string text, CancellationToken cancellationToken = default);
}
