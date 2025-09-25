using AutoMapper;
using EAMS_ACore.Interfaces;
using EAMS_ACore.ServiceModels;
using LBPAMS.ViewModels.PublicModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.OpenApi.Models;

namespace LBPAMS.MinimalAPIs.PublicEndpoints
{
    public static class PublicEndpoint
    {
        public static RouteGroupBuilder MapV1PublicEndpoint(this RouteGroupBuilder endpoints)
        {

            endpoints.MapPost("/GetResultDeclarationForZPAndPsZone", async (
    [FromBody]RqByMasterIdsViewModel request, // Change parameter type to ViewModel
    IMapper mapper,
    IEamsService eamsService,
    CancellationToken cancellationToken) =>
            {
                try
                {
                    // ✅ Map body DTO -> domain model
                    var serviceRequest = mapper.Map<RqByMasterIds>(request);

                    var result = await eamsService.GetResultDeclarationForZPAndPsZone(serviceRequest, cancellationToken);
                    return Results.Text(result, "application/json");
                }
                catch (Exception ex)
                {
                    return Results.Problem($"Error retrieving dashboard data: {ex.Message}");
                }
            })
.WithName("GetResultDeclarationForZPAndPsZone")
    .WithOpenApi(operation => new(operation)
    {
        Summary = "Fetch Result Declaration for ZP & PS Zone",
        Description = "Fetches election result declaration details (rounds, booths, candidates, votes). " +
                      "Data is built via a PostgreSQL function for high performance and returned as JSON.",
        Tags = new List< OpenApiTag>
        {
            new() { Name = "ResultDeclaration" }
        }
    });



            return endpoints;
        }
    }
}
