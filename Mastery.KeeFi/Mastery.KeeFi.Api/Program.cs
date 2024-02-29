using Mastery.KeeFi.Api.ExceptionHandling;
using Mastery.KeeFi.BusinessLogic;
using Mastery.KeeFi.BusinessLogic.Interfaces;
using Mastery.KeeFi.Common.Configurations;
using Mastery.KeeFi.Data;
using Mastery.KeeFi.Data.Interfaces;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
builder.Services.AddProblemDetails();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();

// Configure options using the Configuration API
builder.Services.Configure<DocumentStorageOptions>(builder.Configuration.GetSection(DocumentStorageOptions.ConfigKey));

var clientPath = builder.Configuration.GetSection(DocumentStorageOptions.ConfigKey).GetValue(typeof(string), "ClientPath");
var documentPath = builder.Configuration.GetSection(DocumentStorageOptions.ConfigKey).GetValue(typeof(string), "DocumentPath");
var documentBlobPath = builder.Configuration.GetSection(DocumentStorageOptions.ConfigKey).GetValue(typeof(string), "DocumentBlobPath");

builder.Services.AddScoped<IFileService, FIleService>();

builder.Services.AddScoped<IDocumentsMetadataService>(d =>
    new DocumentsMetadataService(documentPath.ToString()));

builder.Services.AddScoped<IClientsService>(c =>
    new ClientsService(clientPath.ToString(), documentBlobPath.ToString(), c.GetRequiredService<IDocumentsMetadataService>(),
    c.GetRequiredService<IFileService>()));

builder.Services.AddScoped<IDocumentsContentService>(d =>
    new DocumentsContentService(documentBlobPath.ToString(), d.GetRequiredService<IDocumentsMetadataService>(),
    d.GetRequiredService<IClientsService>(), d.GetRequiredService<IFileService>()));

// Register and configure Swagger generator services
builder.Services.AddSwaggerGen(c =>
{
    // Define a Swagger document for a specific version of the API
    // "v1" is the version identifier for this Swagger document
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "GTE Mastery Document API", // The title of the API
        Description = "A basic API for document manipulation defined explicitly for GTE Mastery program", // A short description of the API
        Version = "v1" // The version of the API
    });

    // Determine the file name for the XML documentation
    //string? xmlFileName = typeof(Program).Assembly.GetName().Name;

    // Build the path to the XML documentation file
    //string filePath = Path.Combine(AppContext.BaseDirectory, $"{xmlFileName}.xml");

    // Include XML comments in the Swagger documentation, enhancing the documentation with comments from the code
    //c.IncludeXmlComments(filePath);

    // Enable annotations in Swagger, allowing the use of attributes in the code to further customize the Swagger documentation
    c.EnableAnnotations();
});

WebApplication app = builder.Build();

using var scope = app.Services.CreateScope();
var fileService = scope.ServiceProvider.GetRequiredService<IFileService>();

fileService.CreateFile(clientPath.ToString());

fileService.CreateFile(documentPath.ToString());

fileService.CreateDirectory(documentBlobPath.ToString());

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    // Enable the middleware for exception handling
    app.UseCustomExceptionHandler();

    // Configure Swagger middleware
    app.UseSwagger(c =>
        // Set the route template for the Swagger documentation
        // - "docs/": The base path for the Swagger UI
        // - "{documentName}": A placeholder for the Swagger document name
        // - "swagger.{json|yaml}": Specifies that the documentation can be accessed in either JSON or YAML format
        c.RouteTemplate = "swagger/{documentName}/swagger.{json|yaml}");

    // Configure Swagger UI, the interface for interacting with the Swagger documentation
    app.UseSwaggerUI(c =>
    {
        // Set the route prefix for accessing the Swagger UI
        // This means the Swagger UI can be accessed at the path "/docs"
        c.RoutePrefix = "swagger";

        // Define a Swagger endpoint
        // - "v1/swagger.json": The path to the Swagger JSON file for this endpoint
        // - "v1": The name of the endpoint as it will appear in the Swagger UI
        // This is where the API documentation for version 1 of the API will be accessible
        c.SwaggerEndpoint("v1/swagger.json", "v1");
    });
}

//app.UseHttpsRedirection();

//app.UseAuthorization();

app.MapControllers();

app.Run();
