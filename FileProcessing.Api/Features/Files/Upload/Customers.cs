namespace FileProcessing.Api.Features.Files.Upload;
//json object for customers
public record Customers
{
    public string CustomerName { get; set; } = string.Empty;
    public string CustomerId { get; set; } = string.Empty;
    public string CustomerEmail { get; set; } = string.Empty;
    public string CustomerPhone { get; set; } = string.Empty;
    public int CustomerAge { get; set; }
}