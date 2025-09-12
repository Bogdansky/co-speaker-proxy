using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;

namespace CoSpeakerProxy.Extensions
{
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        /// Adds a default CORS policy that allows any origin, method, and header.
        /// </summary>
        /// <param name="services"></param>
        /// <remarks>Should be updated with actual origins, methods, and headers</remarks>
        /// <returns></returns>
        public static IServiceCollection AddDefaultCors(this IServiceCollection services)
        {
            services.AddCors(options =>
            {
                options.AddDefaultPolicy(builder =>
                {
                    builder.AllowAnyOrigin()
                           .AllowAnyMethod()
                           .AllowAnyHeader();
                });
            });
            return services;
        }

        public static IServiceCollection AddAuthentication(this IServiceCollection services, ConfigurationManager configuration)
        {
            var jwtIssuer = configuration["Jwt:Issuer"];
            var jwtKey = configuration["Jwt:Key"];

            ArgumentNullException.ThrowIfNull(jwtIssuer);
            ArgumentNullException.ThrowIfNull(jwtKey);

            var signingKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey));

            services
                .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(opts =>
                {
                    opts.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuer = true,
                        ValidateAudience = false,
                        ValidateIssuerSigningKey = true,
                        ValidIssuer = jwtIssuer,
                        IssuerSigningKey = signingKey,
                        ClockSkew = TimeSpan.FromSeconds(5)
                    };
                });
            return services;
        }

        public static IServiceCollection AddAppSettings(this IServiceCollection services, ConfigurationManager config)
        {
            services.AddSingleton(sp =>
            {
                CheckJwtSettings();
                CheckDeepgramSettings();
                CheckBedrockSettings();

                return new AppSettings
                {
                    JwtIssuer = config["Jwt:Issuer"]!,
                    JwtKey = config["Jwt:Key"]!,
                    Deepgram = new DeepgramSettings(config["Deepgram:ApiKey"]!, config["Deepgram:Model"]!),
                    AmazonBedrock = new AmazonBedrock(config["AmazonBedrock:BaseUrl"]!, config["AmazonBedrock:Model"]!, config["AmazonBedrock:ApiKey"]!)
                };
            });
            return services;

            void CheckJwtSettings()
            {
                ArgumentNullException.ThrowIfNull(config["Jwt:Issuer"]);
                ArgumentNullException.ThrowIfNull(config["Jwt:Key"]);
            }

            void CheckDeepgramSettings()
            {
                ArgumentNullException.ThrowIfNull(config["Deepgram:ApiKey"]);
                ArgumentNullException.ThrowIfNull(config["Deepgram:Model"]);
            }

            void CheckBedrockSettings()
            {
                ArgumentNullException.ThrowIfNull(config["AmazonBedrock:BaseUrl"]);
                ArgumentNullException.ThrowIfNull(config["AmazonBedrock:Model"]);
                ArgumentNullException.ThrowIfNull(config["AmazonBedrock:ApiKey"]);
            }
        }
    }
}