

namespace FileProcessing.Api.Features.Files.Upload;

public record FileProcessingResults(
    string FileName, int RecordsProcessed,int RecordsAccepted , List<Customers> Customers)
{
    public int RecordsRejected => RecordsProcessed - RecordsAccepted;
}
