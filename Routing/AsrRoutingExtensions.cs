using System.IdentityModel.Tokens.Jwt;
using CoSpeakerProxy.Models;

namespace CoSpeakerProxy.Routing;

public static class AsrRoutingExtensions
{
    public static void MapAsrRoutes(this IEndpointRouteBuilder routes)
    {
        routes.MapPost("/asr/transcribe", (TranscribeDto dto) =>
        {
            var sample = dto.Lang == "en"
                ? "This is a mock English transcript. I would like to practice my pronunciation."
                : "Гэта мокавы транскрыпт па-беларуску. Я хачу трэніраваць маё вымаўленне.";

            return Results.Ok(new
            {
                text = sample,
                words = new[] {
                    new { start = 0.00, end = 0.30, w = "Гэта" },
                    new { start = 0.31, end = 0.60, w = "мокавы" }
                }
            });
        }).RequireAuthorization();
    }
}