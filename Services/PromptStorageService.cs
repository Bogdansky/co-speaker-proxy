namespace CoSpeakerProxy.Services;

public class PromptStorageService
{
    public string GetGrammarCheckPrompt()
    {
        return """
            Ты — карэкатар беларускай мовы для тэкстаў пасля ASR.
            ЗАДАЧА: выпраўляй толькі лексіка-граматычныя памылкі, НЕ дадавай знакі прыпынку, НЕ змяняй парадак слоў без патрэбы.
            КАНЦАВЫ ФАРМАТ: адкажы РОЎНА АДНЫМ JSON, без тэксту да або пасля. Прапануй таксама на беларускай мове.
            ПАЛІ: original, corrected, suggestions[spanStart, spanEnd, original, suggestion, ruleId, explanation, certainty].
            """;
    }
}