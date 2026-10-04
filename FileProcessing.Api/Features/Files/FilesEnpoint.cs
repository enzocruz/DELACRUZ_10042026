

using FileProcessing.Api.Features.Files.Reports;
using FileProcessing.Api.Features.Files.Upload;

namespace FileProcessing.Api.Features.Files;


public static class FilesEndpoint
{
    public static void MapFilesEndpoint(this IEndpointRouteBuilder app)
    {
        
         new UploadEndpoint().Map(app);
         new ReportsEndpoint().Map(app);
            
    }
}