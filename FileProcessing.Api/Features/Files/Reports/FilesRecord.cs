namespace FileProcessing.Api.Features.Files.Reports;
//THis represents the file records tha have been processed and stored in the database. It contains the file name, record count, file size, created date, and modified date.
public record FilesRecord (
    string FileName,
    int TotalRecordCount,
    int AcceptedRecordCount,
    long FileSize,
    DateTime ProcessingStart,
    DateTime ProcessingEnd   
)
{
    public int RejectedRecordCount => TotalRecordCount - AcceptedRecordCount;
    public double ProccessingTimeMS=>(ProcessingEnd-ProcessingStart).TotalMilliseconds;
}