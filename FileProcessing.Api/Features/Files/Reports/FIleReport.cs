

using System.Collections.Concurrent;

namespace FileProcessing.Api.Features.Files.Reports;

public class FileReport : IFileReport
{
    private readonly ConcurrentBag<FilesRecord> _fileRecords = new();

    public FileProcessingReport GenerateReport( CancellationToken cancellationToken)
    {
        return new FileProcessingReport(_fileRecords.Count, _fileRecords.ToList());
    }

    public void RecordFile(FilesRecord fileRecord)
    {
        _fileRecords.Add(fileRecord);
    }
}