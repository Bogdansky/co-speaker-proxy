using System.Text;
using System.Text.Json.Nodes;
using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Mvc;
using CoSpeakerProxy.Models;

namespace CoSpeakerProxy.Routing;

public static partial class GrammarRoutingExtensions
{
    public static void MapGrammarRoutes(this IEndpointRouteBuilder routes)
    {
        routes.MapPost("/grammar/check", async ([FromServices] IHttpClientFactory cf, GrammarDto dto) =>
        {
            return await CheckGrammar(cf, dto);
        }).RequireAuthorization();
    }

    private static async Task<object> CheckGrammar(IHttpClientFactory cf, GrammarDto dto)
    {
        try
        {
            var raw = dto.Text ?? string.Empty;
            var normalized = NormalizeTranscript(raw);

            var lang = dto.Lang?.ToLowerInvariant() switch
            {
                "en" or "en-us" => "en-US",
                "en-gb" => "en-GB",
                _ => "be"
            };

            var http = cf.CreateClient("languagetool");
            http.BaseAddress = new Uri("http://localhost:8010");
            var form = new FormUrlEncodedContent(new Dictionary<string, string>
            {
                ["language"] = lang,
                ["text"] = normalized,
                ["enabledOnly"] = "false",
                ["disabledCategories"] = "STYLE" // меньше «стилистики» для речи
            });

            var resp = await http.PostAsync("v2/check", form);
            if (!resp.IsSuccessStatusCode)
                return Results.Problem($"LT error {resp.StatusCode}: {await resp.Content.ReadAsStringAsync()}");

            var json = JsonNode.Parse(await resp.Content.ReadAsStringAsync());
            var matches = json?["matches"]?.AsArray() ?? [];

            var issues = matches.Select(m => new
            {
                rule = m?["rule"]?["id"]?.GetValue<string>() ?? "",
                message = m?["message"]?.GetValue<string>() ?? "",
                offset = m?["offset"]?.GetValue<int>() ?? 0,
                length = m?["length"]?.GetValue<int>() ?? 0,
                replacements = m?["replacements"]?.AsArray()
                    .Select(r => r?["value"]?.GetValue<string>())
                    .Where(v => !string.IsNullOrEmpty(v))
                    .Take(3).ToArray() ?? []
            }).ToList();

            // Применяем первую подсказку (если есть), начиная с конца
            var sb = new StringBuilder(normalized);
            foreach (var m in issues.OrderByDescending(i => i.offset))
            {
                if (m.replacements.Length == 0) continue;
                if (m.offset < 0 || m.length < 0 || m.offset + m.length > sb.Length) continue;
                sb.Remove(m.offset, m.length);
                sb.Insert(m.offset, m.replacements[0]);
            }

            return Results.Ok(new { suggested = sb.ToString(), issues, original = raw, normalized });
        }
        catch (Exception ex)
        {
            return Results.Problem($"Exception: {ex.Message}");
        }
    }

    private static string NormalizeTranscript(string s)
    {
        if (string.IsNullOrWhiteSpace(s)) return s;
        var t = s;
        // 1) Spaces around punctuation
        t = SpacesBeforeCommaRegex().Replace(t, ",");
        t = SpacesBeforeDotRegex().Replace(t, ".");
        t = CommaWithoutSpaceAfterRegex().Replace(t, ", "); // comma without space after
        t = MultipleSpacesRegex().Replace(t, " "); // double spaces

        // 2) Remove typical speech fillers (approximate list, can be adjusted for 'be')
        t = SpeechFillersRegex().Replace(t, "");
        t = MultipleSpacesRegex().Replace(t, " ");

        // 3) Proper capitalization at the start of sentences (if not already capitalized)
        t = SentenceStartCapitalizationRegex().Replace(t,
            m => m.Groups[1].Value + m.Groups[2].Value.ToUpper());

        return t.Trim();
    }

    [GeneratedRegex(@"\s+,", RegexOptions.Compiled)]
    private static partial Regex SpacesBeforeCommaRegex();

    [GeneratedRegex(@"\s+\.", RegexOptions.Compiled)]
    private static partial Regex SpacesBeforeDotRegex();

    [GeneratedRegex(@",(?=\S)", RegexOptions.Compiled)]
    private static partial Regex CommaWithoutSpaceAfterRegex();

    [GeneratedRegex(@"\s{2,}", RegexOptions.Compiled)]
    private static partial Regex MultipleSpacesRegex();

    [GeneratedRegex(@"\b(э|ээ|ну|мм+)\b", RegexOptions.IgnoreCase | RegexOptions.Compiled)]
    private static partial Regex SpeechFillersRegex();

    [GeneratedRegex(@"(^|[.!?]\s+)([а-яa-z])", RegexOptions.CultureInvariant | RegexOptions.Compiled)]
    private static partial Regex SentenceStartCapitalizationRegex();
}