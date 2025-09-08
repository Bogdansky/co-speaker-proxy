using CoSpeakerProxy.Models;

namespace CoSpeakerProxy.Routing;

public static class GrammarRoutingExtensions
{
    public static void MapGrammarRoutes(this IEndpointRouteBuilder routes)
    {
        routes.MapPost("/grammar/check", (GrammarDto dto) =>
        {
            var text = dto.Text ?? string.Empty;
            var suggested = text
                .Replace(" ,", ",")
                .Replace(" .", ".")
                .Replace("  ", " ");

            var issues = new List<object>();
            if (text.Contains(" ,"))
                issues.Add(new { rule = "PUNCT_SPACE_BEFORE_COMMA", message = "Лішні прабел перад коскай." });
            if (text.Contains("  "))
                issues.Add(new { rule = "DOUBLE_SPACE", message = "Падвоены прабел." });

            return Results.Ok(new { suggested, issues });
        }).RequireAuthorization();
    }
}