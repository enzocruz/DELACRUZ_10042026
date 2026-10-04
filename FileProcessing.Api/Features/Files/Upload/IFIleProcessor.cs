

namespace FileProcessing.Api.Features.Files.Upload;

 public interface IFileProcessor
{
    Task<FileProcessingResults> ProcessFileAsync(IFormFile file,CancellationToken cancellationToken);
}