using CoSpeakerProxy;
using CoSpeakerProxy.Extensions;
using CoSpeakerProxy.Routing;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDefaultCors();
builder.Services.AddAuthentication(builder.Configuration);
builder.Services.AddAuthorization();
builder.Services.AddSingleton(sp =>
{
    var config = builder.Configuration;

    ArgumentNullException.ThrowIfNull(config["Jwt:Issuer"]);
    ArgumentNullException.ThrowIfNull(config["Jwt:Key"]);

    return new AppSettings
    {
        JwtIssuer = config["Jwt:Issuer"]!,
        JwtKey = config["Jwt:Key"]!
    };
});

var app = builder.Build();

app.UseCors();
app.UseAuthentication();
app.UseAuthorization();

app.MapWebApplicationRoutes(builder.Configuration);

app.Run();
