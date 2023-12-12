namespace Outbox.Order.Routing;

public static class RouteExtension
{
    public static void AddMinimalRoutes(this WebApplication app, IConfiguration configuration)
    {
        OrderRoute.MapOrderControllers(app, configuration);
    }
}