using CoSpeakerProxy;
using CoSpeakerProxy.Extensions;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDefaultCors();
builder.Services.AddAuthentication(builder.Configuration);
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

app.Run();
