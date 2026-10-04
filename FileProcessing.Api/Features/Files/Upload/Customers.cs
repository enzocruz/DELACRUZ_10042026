namespace FileProcessing.Api.Features.Files.Upload;

using System.Text.Json.Serialization;


//json object for customers
public record Customers
{
    [JsonRequired]
    public string CustomerName { get; set; } = string.Empty;
    [JsonRequired]
    public string CustomerId { get; set; } = string.Empty;
    [JsonRequired]
    public string CustomerEmail { get; set; } = string.Empty;
    [JsonRequired]
    public string CustomerPhone { get; set; } = string.Empty;
    [JsonRequired]
    public int CustomerAge { get; set; }
}