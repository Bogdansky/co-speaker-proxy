namespace CoSpeakerProxy;

public class AppSettings
{
    public required string JwtIssuer { get; set; }
    public required string JwtKey { get; set; }
    public required DeepgramSettings Deepgram { get; set; }
}

public record DeepgramSettings(string ApiKey, string Model);