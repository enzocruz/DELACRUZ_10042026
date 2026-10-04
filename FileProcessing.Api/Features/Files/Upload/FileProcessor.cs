

using System.Text.Json;
using System.Text.Json.Serialization;

namespace FileProcessing.Api.Features.Files.Upload;

public class FileProcessor : IFileProcessor
{
    private readonly ILogger<FileProcessor> _logger;

    public FileProcessor(ILogger<FileProcessor> logger)
    {
        _logger = logger;
    }
     private readonly JsonSerializerOptions options= new() {
        UnmappedMemberHandling = JsonUnmappedMemberHandling.Disallow
    };
    public async Task<FileProcessingResults> ProcessFileAsync(IFormFile file ,CancellationToken cancellationToken)
    {
        if(file == null)
        {
            _logger.LogWarning("File is null.");
            throw new ArgumentException( "File cannot be null.");
        }
        if(file.Length == 0)
        {
            _logger.LogWarning("File is empty.");
            throw new ArgumentException("No file uploaded.");
        }

        if(file.FileName == null || !file.FileName.EndsWith(".json"))
        {
            _logger.LogWarning("Invalid file format. Only Json files are allowed.");
            throw new ArgumentException("Invalid file format. Only Json files are allowed.");
        }
      
        using var stream = file.OpenReadStream();
        
        try
        {
           var customers = await JsonSerializer.DeserializeAsync<List<Customers>>(stream,options, cancellationToken: cancellationToken);
            if (customers == null)
            {
                _logger.LogWarning("No customers found in the file {FileName}", file.FileName);
                throw new JsonException("No customers found in the file.");
            }
            //fileter by age over 18
            var filteredCustomers = customers.Where(c => c.CustomerAge >= 18).ToList();
            return new FileProcessingResults(file.FileName, customers.Count, filteredCustomers.Count, filteredCustomers);
        }
        catch (JsonException ex)
        {
            _logger.LogWarning(ex, "Invalid JSON format in file {FileName}", file.FileName);
            throw new JsonException("Invalid JSON format in the uploaded file.");
        }    
       
    }

}