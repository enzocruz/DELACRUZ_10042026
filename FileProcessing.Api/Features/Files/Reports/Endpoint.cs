
using FileProcessing.Api.Endpoints;
using Microsoft.AspNetCore.Mvc;

namespace FileProcessing.Api.Features.Files.Reports;

public class ReportsEndpoint: IEndpoint
{
    public void Map(IEndpointRouteBuilder app)
    {
        app.MapGet("/api/reports",ReportsHandler.HandleAsync )
        .WithName("GetFileProcessingReport")
        .WithTags("Reports");
    }

}

public static class ReportsHandler
{
    public static async Task<IResult>  HandleAsync ([FromServices] IFileReport fileReport, CancellationToken cancellationToken) 
    {
        var report = fileReport.GenerateReport(cancellationToken);
        return Results.Ok(report);
    }
}