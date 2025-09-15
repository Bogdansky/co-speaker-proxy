namespace CoSpeakerProxy.Models;

public class ConverseRequest
{
    public List<SystemMessage> System { get; set; }
    public List<UserMessage> Messages { get; set; }
    public InferenceConfig InferenceConfig { get; set; }
}

public class SystemMessage
{
    public required string Text { get; set; }
}

public class UserMessage
{
    public required string Role { get; set; }
    // could be array of texts or objects such as images and documents
    public required object[] Content { get; set; }
}

public record InferenceConfig
{
    public int MaxTokens { get; set; }
    public double Temperature { get; set; }
    public double TopP { get; set; }
}