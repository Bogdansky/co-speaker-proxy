namespace CoSpeakerProxy;

public class AppSettings
{
    public required string JwtIssuer { get; set; }
    public required string JwtKey { get; set; }
    public required DeepgramSettings Deepgram { get; set; }
    public required AmazonBedrock AmazonBedrock { get; set; }
}

public record DeepgramSettings(string ApiKey, string Model);
public record AmazonBedrock(string BaseUrl, string Model, string ApiKey);