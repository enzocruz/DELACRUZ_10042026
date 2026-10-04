namespace FileProcessing.Api.Features.Files.Reports;

public interface IFileReport
{
    FileProcessingReport GenerateReport(CancellationToken cancellationToken);
     void RecordFile(FilesRecord fileRecord);
}