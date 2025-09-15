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
            var client = factory.Create();

            var audioData = File.ReadAllBytes("./test_records/test.m4a");

            var response = await client.TranscribeFile(
                audioData,
                new PreRecordedSchema
                {
                    Model = config.Deepgram.Model,
                    Language = "be"
                }
            );

            if (!IsResponseValid(response))
            {
                throw new NoNullAllowedException("No transcription results");
            }

            return Results.Ok(new
            {
                text = response.Results.Channels[0].Alternatives[0].Transcript,
                words = response.Results.Channels[0].Alternatives[0].Words?.Select(w => new
                {
                    Word = w.HeardWord,
                    w.Start,
                    w.End,
                    w.Confidence
                })
            });
        }).RequireAuthorization();
    }
    
    private static bool IsResponseValid(SyncResponse response)
    {
        return response.Results != null
            && response.Results.Channels != null
            && response.Results.Channels.Count > 0
            && response.Results.Channels[0].Alternatives != null
            && response.Results.Channels[0].Alternatives!.Count > 0;
    }
}