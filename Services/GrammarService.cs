using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using CoSpeakerProxy.Models.Builders;

namespace CoSpeakerProxy.Services;

public class GrammarService
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<GrammarService> _logger;
    private readonly string _systemPrompt;
    private readonly string _model;
    private readonly JsonSerializerOptions DefaultJsonSerializerOptions = new() { PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower };

    public GrammarService(
        AppSettings config,
        IHttpClientFactory httpClientFactory,
        ILogger<GrammarService> logger,
        PromptStorageService promptStorageService)
    {
        _httpClient = httpClientFactory.CreateClient();
        _httpClient.BaseAddress = new Uri(config.AmazonBedrock.BaseUrl);
        _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", config.AmazonBedrock.ApiKey);
        _httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

        _logger = logger;
        _model = config.AmazonBedrock.Model;
        _systemPrompt = promptStorageService.GetGrammarCheckPrompt();
    }

    /// <summary>
    /// Pass text to be checked for grammar issues. This method does not check punctuation or spelling.
    /// </summary>
    /// <param name="text">Text to be checked.</param>
    /// <param name="lang">Language of passed text. Currently exists only for backward compatibility.</param>
    /// <returns></returns>
    public async Task<string> CheckGrammarAsync(string text, string lang)
    {
        try
        {
            var body = ConverseRequestBuilder
                .Create()
                .AddSystem(_systemPrompt)
                .AddMessage("user", [new { text }]);

            var json = JsonSerializer.Serialize(body, DefaultJsonSerializerOptions);
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