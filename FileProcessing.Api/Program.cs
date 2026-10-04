using FileProcessing.Api.Features.Files;
using FileProcessing.Api.Features.Files.Reports;
using FileProcessing.Api.Features.Files.Upload;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();
//add swagger
builder.Services.AddSwaggerGen();
//add services to the container
builder.Services.AddScoped<IFileProcessor, FileProcessor>();
builder.Services.AddSingleton<IFileReport, FileReport>();
var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();   
}
//add swagger to run both on dev or production but it should not be. 
app.UseSwagger();
app.UseSwaggerUI();


app.UseHttpsRedirection();

//add endpoints to the container
app.MapFilesEndpoint();

app.Run();

