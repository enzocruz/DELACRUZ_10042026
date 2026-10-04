using System.Text.Json;
using FileProcessing.Api.Features.Files.Reports;
using FileProcessing.Api.Features.Files.Upload;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

public class FileProcessorTests
{
  
   

    [Fact]
    public async Task TestProcessFileAsync_WhenFileIsNull_ThrowsArgumentException()
    {
        // Arrange
        IFormFile file = null!;
        // Act
        var procesor=new FileProcessor(Mock.Of<ILogger<FileProcessor>>());

        var result=procesor.ProcessFileAsync(file,CancellationToken.None);
        
        // Assert
        var exception = await Assert.ThrowsAsync<ArgumentException>(() => result);

        Assert.Equal("File cannot be null.", exception.Message);
    }

    [Fact]
    public async Task TestProcessFileAsync_WhenFileIsEmpty_ThrowsArgumentException()
    {
        // Arrange
        var file = new Mock<IFormFile>();

        file.Setup(x => x.Length).Returns(0);
        
        var fileProcessor = new FileProcessor(Mock.Of<ILogger<FileProcessor>>());
        // Act
        var result = fileProcessor.ProcessFileAsync(file.Object, CancellationToken.None);

        // Assert
        var exception = await Assert.ThrowsAsync<ArgumentException>(() => result);

        Assert.Equal("No file uploaded.", exception.Message);
    }

    [Fact]
    public async Task TestProcessFileAsync_WhenFileIsNotJson_ThrowsArgumentException()
    {
        // Arrange
        var file = new Mock<IFormFile>();

        file.Setup(x => x.Length).Returns(100);
        file.Setup(x => x.FileName).Returns("customers.txt");

        // Act
        var fileProcessor = new FileProcessor(Mock.Of<ILogger<FileProcessor>>());
        var result = fileProcessor.ProcessFileAsync(file.Object, CancellationToken.None);
    
        var exception = await Assert.ThrowsAsync<ArgumentException>(() => result);
      
        Assert.Equal("Invalid file format. Only Json files are allowed.", exception.Message);
    }

    [Fact]

    public async Task TestProcessFileAsync_WhenFileIsInvalidUnmapped_ThrowsJsonException()
    {
        // Arrange
        var file = new Mock<IFormFile>();
        var fileName = "customers.json";
        var fileContent ="""
            [
                {
                    "CustomerName": "John Doe",
                    "CustomerId": "CUST-10003",
                    "CustomerEmail": "john.doe@example.com",
                    "CustomerPhone": "+63 917 123 4569",
                    "CustomerAge": 25,
                    "FirstName":"John"
                }
            ]
        """;
        var fileStream = new MemoryStream(System.Text.Encoding.UTF8.GetBytes(fileContent));

        file.Setup(x => x.Length).Returns(fileStream.Length);
        file.Setup(x => x.FileName).Returns(fileName);
        file.Setup(x => x.OpenReadStream()).Returns(fileStream);

        var fileProcessor = new FileProcessor(Mock.Of<ILogger<FileProcessor>>());

        // Act
        var result =  fileProcessor.ProcessFileAsync(file.Object, CancellationToken.None);

        var exception = await Assert.ThrowsAsync<JsonException>(() => result);
        // Assert
        Assert.Equal("Invalid JSON format in the uploaded file.", exception.Message);
    }
    [Fact]
    public async Task TestProcessFileAsync_WhenFileIsInvalidMissingPropeties_ThrowsJsonException()
    {
        // Arrange
        var file = new Mock<IFormFile>();
        var fileName = "customers.json";
        var fileContent ="""
             [
                
                {
                    "CustomerName": "John Smith",
                    "CustomerId": "CUST-10001"
                
                },
                {
                    "CustomerName": "Jane Doe",
                    "CustomerIdS": "CUST-10002"
                    
                },
                {
                    "CustomerName": "John Doe"
                
                }
            ]
            
        """;
        var fileStream = new MemoryStream(System.Text.Encoding.UTF8.GetBytes(fileContent));

        file.Setup(x => x.Length).Returns(fileStream.Length);
        file.Setup(x => x.FileName).Returns(fileName);
        file.Setup(x => x.OpenReadStream()).Returns(fileStream);

        var fileProcessor = new FileProcessor(Mock.Of<ILogger<FileProcessor>>());

        // Act
        var result =  fileProcessor.ProcessFileAsync(file.Object, CancellationToken.None);

        var exception = await Assert.ThrowsAsync<JsonException>(() => result);
        // Assert
        Assert.Equal("Invalid JSON format in the uploaded file.", exception.Message);
    }

    [Fact]

    public async Task TestProcessFileAsync_WhenFileIsValid_ReturnsExpectedResults()
    {
        // Arrange
        var file = new Mock<IFormFile>();
        var fileName = "customers.json";
        var fileContent ="""
          [
            {
                "CustomerName": "John Smith",
                "CustomerId": "CUST-10001",
                "CustomerEmail": "john.smith@example.com",
                "CustomerPhone": "+63 917 123 4567",
                "CustomerAge": 20
            },
            {
                "CustomerName": "Jane Doe",
                "CustomerId": "CUST-10002",
                "CustomerEmail": "jane.doe@example.com",
                "CustomerPhone": "+63 917 123 4568",
                "CustomerAge": 20
            },
            {
                "CustomerName": "John Doe",
                "CustomerId": "CUST-10003",
                "CustomerEmail": "john.doe@example.com",
                "CustomerPhone": "+63 917 123 4569",
                "CustomerAge": 25
            }
            ]
        """;
        var fileStream = new MemoryStream(System.Text.Encoding.UTF8.GetBytes(fileContent));

        file.Setup(x => x.Length).Returns(fileStream.Length);
        file.Setup(x => x.FileName).Returns(fileName);
        file.Setup(x => x.OpenReadStream()).Returns(fileStream);

        var fileProcessor = new FileProcessor(Mock.Of<ILogger<FileProcessor>>());

        // Act
        var result =  await fileProcessor.ProcessFileAsync(file.Object, CancellationToken.None);

      
        // Assert
        Assert.Equal("customers.json", result.FileName);
        Assert.Equal(3, result.RecordsProcessed);
        Assert.Equal(3, result.RecordsAccepted);
        Assert.Equal(0, result.RecordsRejected);
    }
    [Fact]
     public async Task TestProcessFileAsync_WhenFileIsValid_ReturnsExpectedFilteredResults()
    {
        // Arrange
        var file = new Mock<IFormFile>();
        var fileName = "customers.json";
        var fileContent ="""
           [
                {
                    "CustomerName": "John Smith",
                    "CustomerId": "CUST-10001",
                    "CustomerEmail": "john.smith@example.com",
                    "CustomerPhone": "+63 917 123 4567",
                    "CustomerAge": 20
                },
                {
                    "CustomerName": "Jane Doe",
                    "CustomerId": "CUST-10002",
                    "CustomerEmail": "jane.doe@example.com",
                    "CustomerPhone": "+63 917 123 4568",
                    "CustomerAge": 17
                },
                {
                    "CustomerName": "John Doe",
                    "CustomerId": "CUST-10003",
                    "CustomerEmail": "john.doe@example.com",
                    "CustomerPhone": "+63 917 123 4569",
                    "CustomerAge": 25
                }
            ]
               
        """;
        var fileStream = new MemoryStream(System.Text.Encoding.UTF8.GetBytes(fileContent));

        file.Setup(x => x.Length).Returns(fileStream.Length);
        file.Setup(x => x.FileName).Returns(fileName);
        file.Setup(x => x.OpenReadStream()).Returns(fileStream);

        var fileProcessor = new FileProcessor(Mock.Of<ILogger<FileProcessor>>());

        // Act
        var result =  await fileProcessor.ProcessFileAsync(file.Object, CancellationToken.None);

      
        // Assert
        Assert.Equal("customers.json", result.FileName);
        Assert.Equal(3, result.RecordsProcessed);
        Assert.Equal(2, result.RecordsAccepted);
        Assert.Equal(1, result.RecordsRejected);
    }
}