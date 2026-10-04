namespace FileProcessing.Api.Features.Files.Reports;

public record FileProcessingReport (int TotalFiles, List<FilesRecord> FileRecords);

