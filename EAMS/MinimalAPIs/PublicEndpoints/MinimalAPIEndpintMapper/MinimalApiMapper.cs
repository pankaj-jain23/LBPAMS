namespace LBPAMS.MinimalAPIs.PublicEndpoints.MinimalAPIEndpintMapper
{
    public static class MinimalApiMapper
    {
        public static void MapMinimalEndpoints(this WebApplication app)
        {
            app.MapGroup("/api/PublicEndpoint")
               .MapV1PublicEndpoint();
        }
    }
}
