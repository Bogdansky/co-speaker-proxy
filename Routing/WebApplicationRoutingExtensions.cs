namespace CoSpeakerProxy.Routing;

public static class WebApplicationRoutingExtensions
{
    public static void MapWebApplicationRoutes(this IEndpointRouteBuilder routes, ConfigurationManager configuration)
    {
        routes.MapAuthRoutes(configuration);
        routes.MapAsrRoutes();
        routes.MapGrammarRoutes();
    }
}