namespace CoSpeakerProxy.Routing;

public static class WebApplicationRoutingExtensions
{
    public static void MapWebApplicationRoutes(this IEndpointRouteBuilder routes)
    {
        routes.MapAuthRoutes();
        routes.MapAsrRoutes();
        routes.MapGrammarRoutes();
    }
}