namespace CoSpeakerProxy.Models.Builders;

public static class ConverseRequestBuilder
{
    private const int MaxTokens = 1024;
    private static readonly InferenceConfig _defaultInferenceConfig = new() { MaxTokens = MaxTokens, Temperature = 0.0, TopP = 0.9 };

    public static ConverseRequest Create()
    {
        var request = new ConverseRequest
        {
            InferenceConfig = _defaultInferenceConfig
        };
        return request;
    }
    
    public static ConverseRequest AddSystem(this ConverseRequest request, string systemPrompt)
    {
        request.System ??= [];
        request.System.Add(new() { Text = systemPrompt });
        return request;
    }

    public static ConverseRequest AddMessage(this ConverseRequest request, string userRole, object[] content)
    {
        request.Messages ??= [];
        request.Messages.Add(new () { Role = userRole, Content = content });
        return request;
    }
}