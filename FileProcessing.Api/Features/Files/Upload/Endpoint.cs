using System.Text.Json;
using FileProcessing.Api.Endpoints;
using FileProcessing.Api.Features.Files.Reports;
using Microsoft.AspNetCore.Mvc;

namespace FileProcessing.Api.Features.Files.Upload;

public class UploadEndpoint :IEndpoint
{

    public void Map(IEndpointRouteBuilder app)
    {
        app.MapPost("/api/files/upload",UploadEndpointHandler.HandleAsync)
        .WithName("UploadFile")
        .DisableAntiforgery()
        .WithTags("Files");
    }
    
}

public  static class UploadEndpointHandler
{
    public static  async Task<IResult> HandleAsync(IFormFile file,[FromServices] IFileProcessor fileProcessor,
       [FromServices] IFileReport fileReport, CancellationToken cancellationToken)
    {
        if (file == null || file.Length == 0)
        {
            return Results.BadRequest("No file uploaded.");
        }
        if (!file.FileName.EndsWith(".json", StringComparison.OrdinalIgnoreCase))
        {
            return Results.BadRequest("Invalid file format. Only Json files are allowed.");
        }
        try{ 
        DateTime ProcessStated=DateTime.UtcNow;
    
        var result = await fileProcessor.ProcessFileAsync(file,cancellationToken);

        DateTime ProcessedEnd=DateTime.UtcNow;

        fileReport.RecordFile(new FilesRecord(
            file.FileName,
            result.RecordsProcessed,
            result.RecordsAccepted,
            file.Length,
            ProcessStated,
            ProcessedEnd
        ));
        return Results.Ok(result); 
        }
        catch(JsonException ex)
        {
            return Results.BadRequest("Invalid JSON format.");
        }catch(Exception ex)
        {
            return Results.BadRequest("An error occurred while processing the file.");
        }
    }
}