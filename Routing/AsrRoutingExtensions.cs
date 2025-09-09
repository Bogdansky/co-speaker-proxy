using System.Data;
using System.IdentityModel.Tokens.Jwt;
using CoSpeakerProxy.Clients;
using CoSpeakerProxy.Models;
using Deepgram.Models.Listen.v1.REST;
using Results = Microsoft.AspNetCore.Http.Results;

namespace CoSpeakerProxy.Routing;

public static class AsrRoutingExtensions
{
    public static void MapAsrRoutes(this IEndpointRouteBuilder routes)
    {
        routes.MapPost("/asr/transcribe",
        async (
            DeepgramClientFactory factory,
            AppSettings config,
            TranscribeDto dto) =>
        {
            var client = factory.Create(new ConfigurationManager());

            var audioData = File.ReadAllBytes("./test_records/test.m4a");

            var response = await client.TranscribeFile(
                audioData,
                new PreRecordedSchema
                {
                    Model = config.Deepgram.Model,
                    Language = "be"
                }
            );

            if (response.Results is null
            || response.Results.Channels is null
            || response.Results.Channels.Count < 1
            || response.Results.Channels[0].Alternatives is null
            || response.Results.Channels[0].Alternatives.Count == 0)
            {
                throw new NoNullAllowedException("No transcription results");
            }

            return Results.Ok(new
                {
                    text = response.Results.Channels[0].Alternatives[0].Transcript,
                    words = new[] {
                    new { start = 0.00, end = 0.30, w = "Гэта" },
                    new { start = 0.31, end = 0.60, w = "мокавы" }
                }
                });
        }).RequireAuthorization();
    }
}