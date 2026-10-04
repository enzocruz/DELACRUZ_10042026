using FileProcessing.Api.Features.Files;
using FileProcessing.Api.Features.Files.Reports;
using FileProcessing.Api.Features.Files.Upload;
using FileProcessing.Api.Middleware;
using Microsoft.OpenApi;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

builder.Services.AddProblemDetails();
//add swagger
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new Microsoft.OpenApi.OpenApiInfo
    {
        Title = "File Processing API",
        Version = "v1",
        Description = "An API for processing files and generating reports."
    });

    options.AddSecurityDefinition("ApiKey", new OpenApiSecurityScheme
    {
        Description = "API Key needed to access the endpoints. X-API-KEY: Your_API_Key",
        In = ParameterLocation.Header,
        Name = "X-API-KEY",
        Type = SecuritySchemeType.ApiKey,
       
    });
    
    options.AddSecurityRequirement(document => new OpenApiSecurityRequirement
    {
        [new OpenApiSecuritySchemeReference("ApiKey", document)] = []
    });
       
});
//add services to the container
builder.Services.AddScoped<IFileProcessor, FileProcessor>();
builder.Services.AddSingleton<IFileReport, FileReport>();
var app = builder.Build();

app.UseExceptionHandler();
// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();   
}
//add swagger to run both on dev or production but it should not be. 
app.UseSwagger();
app.UseSwaggerUI();


app.UseHttpsRedirection();
app.UseMiddleware<ApiMiddleware>();
//add endpoints to the container
app.MapFilesEndpoint();

app.Run();

