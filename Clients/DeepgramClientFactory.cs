using Deepgram;
using Deepgram.Clients.Interfaces.v1;

namespace CoSpeakerProxy.Clients;

public class DeepgramClientFactory(AppSettings settings)
{
    private volatile bool _initialized = false;

    public IListenRESTClient Create(IConfiguration configuration)
    {
        var apiKey = settings.Deepgram.ApiKey;
        if (string.IsNullOrWhiteSpace(apiKey))
        {
            apiKey = Environment.GetEnvironmentVariable("DEEPGRAM_API_KEY");

            ArgumentNullException.ThrowIfNull(apiKey, "Deepgram API key is not configured. Please set it in appsettings.json or as an environment variable 'DEEPGRAM_API_KEY'.");
        }

        if (!_initialized)
        {
            Library.Initialize();
            _initialized = true;
        }

        return ClientFactory.CreateListenRESTClient(apiKey);
    }
}