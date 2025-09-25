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

            endpoints.MapGet("/GetResultDeclarationForZPAndPsZone", async (
    [FromBody]RqByMasterIdsViewModel request, // Change parameter type to ViewModel
    IMapper mapper,
    IEamsService eamsService,
    CancellationToken cancellationToken) =>
            {
                // Map ViewModel → ServiceModel
                var serviceRequest = mapper.Map<RqByMasterIds>(request);

                // Call service with correct type
                var result = await eamsService.GetResultDeclarationForZPAndPsZone(serviceRequest, cancellationToken);

                return string.IsNullOrEmpty(result)
                    ? Results.NotFound("No result found.")
                    : Results.Json(result);
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
