using FileProcessing.Api.Features.Files.Reports;
using FileProcessing.Api.Features.Files.Upload;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;

public class UploadEndpointHandlerTest
{
    [Fact]
    public async Task TestHandleAsync_WhenProcessingFails_ReturnsInternalServerError()
    {
        // Arrange
        var file = new Mock<IFormFile>();
        file.Setup(x => x.FileName).Returns("customers.json");
        file.Setup(x => x.Length).Returns(100);

        var processor = new Mock<IFileProcessor>();
        processor.Setup(x => x.ProcessFileAsync(It.IsAny<IFormFile>(), It.IsAny<CancellationToken>()))
                 .ThrowsAsync(new InvalidOperationException("test error"));

        // Act
        var result = await UploadEndpointHandler.HandleAsync(
            file.Object, processor.Object, new FileReport(), CancellationToken.None,
            NullLogger<UploadEndpoint>.Instance);

        // Assert
        var problem = Assert.IsType<ProblemHttpResult>(result);
        Assert.Equal(StatusCodes.Status500InternalServerError, problem.StatusCode);
    }
}