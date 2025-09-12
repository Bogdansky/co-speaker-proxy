using System.Text;
using System.Text.Json.Nodes;
using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Mvc;
using CoSpeakerProxy.Models;
using CoSpeakerProxy.Services;

namespace CoSpeakerProxy.Routing;

public static partial class GrammarRoutingExtensions
{
    public static void MapGrammarRoutes(this IEndpointRouteBuilder routes)
    {
        routes.MapPost("/grammar/check", async ([FromServices] GrammarService grammarService, GrammarDto dto) =>
        {
            return await CheckGrammar(grammarService, dto);
        }).RequireAuthorization();
    }

    private static async Task<IResult> CheckGrammar(GrammarService grammarService, GrammarDto dto)
    {
        try
        {
            var raw = dto.Text ?? string.Empty;
            var normalized = NormalizeTranscript(raw);

            var responseText = await grammarService.CheckGrammarAsync(normalized, dto.Lang);
            return Results.Ok(responseText);
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