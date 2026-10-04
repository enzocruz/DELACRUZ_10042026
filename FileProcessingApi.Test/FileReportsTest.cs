using System.Text.Json;
using FileProcessing.Api.Features.Files.Reports;
using FileProcessing.Api.Features.Files.Upload;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

public class FileReportTests
{
  
   
    [Fact]
    public async Task TestFileReport_WhenFileIsProcessed_ReturnsExpectedResults()
    {
        // Arrange
        var processingResults = new FileProcessingResults("test.json", 5, 3, new List<Customers>
        {
            new Customers { CustomerName = "John Doe", CustomerAge = 25 },
            new Customers { CustomerName = "Jane Smith", CustomerAge = 30 },
            new Customers { CustomerName = "Alice Johnson", CustomerAge = 22 }
        });
        // Act
        var fileReport=new FileReport();

        fileReport.RecordFile(new FilesRecord(
            processingResults.FileName,
            processingResults.RecordsProcessed,
            processingResults.RecordsAccepted,
            100, // Assuming file size is 100 bytes for testing
            DateTime.UtcNow,
            DateTime.UtcNow
        ));
        
        var result = fileReport.GenerateReport(CancellationToken.None);
        // Assert
        Assert.NotNull(result);
        Assert.Equal(1, result.TotalFiles);

    }
    [Fact]
    public async Task TestFileReport_WhenFileProceesed_ReturnRecordDetials()
    {
        // Arrange
        var fileReport=new FileReport();
        var record=new FilesRecord("a.json",11,5,100,DateTime.Now,DateTime.Now);

        //Act
        fileReport.RecordFile(record);

        var result= fileReport.GenerateReport(CancellationToken.None);

        //Assert

        var filesRecord=Assert.Single(result.FileRecords);
        Assert.Equal("a.json",filesRecord.FileName);
        Assert.Equal(11,filesRecord.TotalRecordCount);
        Assert.Equal(5,filesRecord.AcceptedRecordCount);
        Assert.Equal(6,filesRecord.RejectedRecordCount);
        Assert.Equal(100,filesRecord.FileSize);

    }

    [Fact]
    public async Task TestFileReport_WhenMultipleFileProceesed_ReturnRecordDetials()
    {
        // Arrange
        var fileReport=new FileReport();
        var record1=new FilesRecord("a.json",11,5,100,DateTime.Now,DateTime.Now);
         var record2=new FilesRecord("b.json",11,5,50,DateTime.Now,DateTime.Now);
        //Act
        fileReport.RecordFile(record1);
        fileReport.RecordFile(record2);

        var result= fileReport.GenerateReport(CancellationToken.None);

        //Assert

       
        Assert.Equal(2,result.TotalFiles);
        Assert.Contains(result.FileRecords,r=>r.FileName.Equals("a.json"));
        Assert.Contains(result.FileRecords,r=>r.FileName.Equals("a.json"));
       
    }

     [Fact]
    public async Task TestFileReport_WhenFileReportIs_ReturnRecordDetials()
    {
        // Arrange
        var fileReport=new FileReport();
        var record1=new FilesRecord("a.json",11,5,100,DateTime.Now,DateTime.Now);
         var record2=new FilesRecord("b.json",11,5,50,DateTime.Now,DateTime.Now);
        //Act
        fileReport.RecordFile(record1);
        fileReport.RecordFile(record2);

        var result= fileReport.GenerateReport(CancellationToken.None);

        //Assert

       
        Assert.Equal(2,result.TotalFiles);
        Assert.Contains(result.FileRecords,r=>r.FileName.Equals("a.json"));
        Assert.Contains(result.FileRecords,r=>r.FileName.Equals("a.json"));
       
    }

}