using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;

namespace CoSpeakerProxy.Services;

public class GrammarService
{
    private const int MaxTokens = 1024;
    private readonly HttpClient _httpClient;
    private readonly ILogger<GrammarService> _logger;
    private readonly string _systemPrompt = """
        Ты — карэкатар беларускай мовы для тэкстаў пасля ASR.
        ЗАДАЧА: выпраўляй толькі лексіка-граматычныя памылкі, НЕ дадавай знакі прыпынку, НЕ змяняй парадак слоў без патрэбы.
        КАНЦАВЫ ФАРМАТ: адкажы РОЎНА АДНЫМ JSON, без тэксту да або пасля. Прапануй таксама на беларускай мове.
        ПАЛІ: original, corrected, suggestions[spanStart, spanEnd, original, suggestion, ruleId, explanation, certainty].
        """;
    private readonly string _model = "Выправі памылкі ў наступным тэксце: {0}";

    public GrammarService(AppSettings config, IHttpClientFactory httpClientFactory, ILogger<GrammarService> logger)
    {
        _httpClient = httpClientFactory.CreateClient();
        _httpClient.BaseAddress = new Uri(config.AmazonBedrock.BaseUrl);
        _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", config.AmazonBedrock.ApiKey);
        _httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

        _logger = logger;
        _model = config.AmazonBedrock.Model;
    }

    public async Task<string> CheckGrammarAsync(string text, string lang)
    {
        try
        {
            var body = new
            {
                system = new[] { new { text = _systemPrompt } },

                messages = new object[] {
                    new {
                    role = "user",
                    content = new object[] { new { text } }
                    }
                },

                inferenceConfig = new { maxTokens = MaxTokens, temperature = 0.0, topP = 0.9 }
            };

            var json = JsonSerializer.Serialize(body);
            var response = await _httpClient.PostAsync($"model/{_model}/converse", new StringContent(json, Encoding.UTF8, "application/json"));

            var responseText = await response.Content.ReadAsStringAsync();
            return responseText;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while checking grammar");
            return $"Exception: {ex.Message}";
        }
    }
}