using FileProcessing.Api.Features.Files;
using FileProcessing.Api.Features.Files.Reports;
using FileProcessing.Api.Features.Files.Upload;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

//add services to the container
builder.Services.AddScoped<IFileProcessor, FileProcessor>();
builder.Services.AddSingleton<IFileReport, FileReport>();
var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

//add endpoints to the container
app.MapFilesEndpoint();

app.Run();

