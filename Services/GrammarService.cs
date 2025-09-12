using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;

namespace CoSpeakerProxy.Services;

public class GrammarService
{
    private const int MaxTokens = 1024;
    private readonly HttpClient _httpClient;
    private readonly string _systemPrompt = """
        Ты — карэкатар беларускай мовы для тэкстаў пасля ASR.
        ЗАДАЧА: выправі {0}. Выпраўляй толькі лексіка-граматычныя памылкі, НЕ дадавай знакі прыпынку, НЕ змяняй парадак слоў без патрэбы.
        КАНЦАВЫ ФАРМАТ: адкажы РОЎНА АДНЫМ JSON, без тэксту да або пасля.
        ПАЛІ: original, corrected, suggestions[spanStart, spanEnd, original, suggestion, ruleId, explanation, certainty].
        """;


    public GrammarService(AppSettings config, IHttpClientFactory httpClientFactory)
    {
        _httpClient = httpClientFactory.CreateClient();
        _httpClient.BaseAddress = new Uri($"{config.AmazonBedrock.BaseUrl}/{config.AmazonBedrock.Model}");
        _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", config.AmazonBedrock.ApiKey);
        _httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
    }

    public async Task<string> CheckGrammarAsync(string text, string lang)
    {
        var prompt = string.Format(_systemPrompt, text);

        var body = new
        {
            max_tokens = 1024,
            temperature = 0,
            messages = new object[]
            {
                new { role = "user", content = new [] { new { type="text", text=prompt } } }
            }
        };
        
        var json = JsonSerializer.Serialize(body);
        var response = await _httpClient.PostAsync("invoke", new StringContent(json, Encoding.UTF8, "application/json"));

        var responseText = await response.Content.ReadAsStringAsync();
        return responseText;
    }
}