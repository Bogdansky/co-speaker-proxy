using CoSpeakerProxy;
using CoSpeakerProxy.Clients;
using CoSpeakerProxy.Extensions;
using CoSpeakerProxy.Routing;
using Deepgram;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDefaultCors();
builder.Services.AddAuthentication(builder.Configuration);
builder.Services.AddAuthorization();
builder.Services.AddSingleton(sp =>
{
    var config = builder.Configuration;

    ArgumentNullException.ThrowIfNull(config["Jwt:Issuer"]);
    ArgumentNullException.ThrowIfNull(config["Jwt:Key"]);
    ArgumentNullException.ThrowIfNull(config["Deepgram:ApiKey"]);
    ArgumentNullException.ThrowIfNull(config["Deepgram:Model"]);

    return new AppSettings
    {
        JwtIssuer = config["Jwt:Issuer"]!,
        JwtKey = config["Jwt:Key"]!,
        Deepgram = new DeepgramSettings(config["Deepgram:ApiKey"]!, config["Deepgram:Model"]!)
    };
});

#region Deepgram Client
Library.Initialize();
builder.Services.AddSingleton<DeepgramClientFactory>();
#endregion

var app = builder.Build();

app.UseCors();
app.UseAuthentication();
app.UseAuthorization();

app.MapWebApplicationRoutes(builder.Configuration);

app.Run();
