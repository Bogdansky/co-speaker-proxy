using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using CoSpeakerProxy.Models;
using Microsoft.IdentityModel.Tokens;

namespace CoSpeakerProxy.Routing;

public static class AuthRoutingExtensions
{
    public static void MapAuthRoutes(this IEndpointRouteBuilder routes, ConfigurationManager configuration)
    {
        var jwtKey = configuration["Jwt:Key"];
        
        ArgumentNullException.ThrowIfNull(jwtKey, nameof(jwtKey));

        var signingKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey));

        routes.MapPost("/auth/token", (AppSettings config, DeviceDto dto) =>
        {
            var handler = new JwtSecurityTokenHandler();
            var token = handler.WriteToken(new JwtSecurityToken(
                issuer: config.JwtIssuer,
                audience: null,
                claims: [new Claim("device", dto.DeviceId ?? "unknown")],
                expires: DateTime.UtcNow.AddHours(8),
                signingCredentials: new SigningCredentials(signingKey, SecurityAlgorithms.HmacSha256)
            ));
            return Results.Ok(new { token });
        });
    }
}