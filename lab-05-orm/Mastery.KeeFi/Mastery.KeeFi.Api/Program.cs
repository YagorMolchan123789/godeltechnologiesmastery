using Mastery.KeeFi.Api.ExceptionHandling;
using Mastery.KeeFi.Api.Configurations;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using AutoMapper;
using Mastery.KeeFi.Business.Profiles;
using Mastery.KeeFi.Business.Interfaces;
using Mastery.KeeFi.Business.Services;
using Mastery.KeeFi.Data;
using Mastery.KeeFi.Data.Interfaces;
using Mastery.KeeFi.Data.Repositories;
using Mastery.KeeFi.Domain.Entities;
using Newtonsoft.Json;
using System;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers()
    .AddNewtonsoftJson(options => options.SerializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore);

builder.Services.AddProblemDetails();

builder.Services.AddDbContext<MainDbContext>(options =>
    options.UseSqlServer(builder.Configuration["ConnectionStings:DefaultConnection"]));

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();

// Configure options using the Configuration API
builder.Services.Configure<DocumentStorageOptions>(builder.Configuration.GetSection(DocumentStorageOptions.ConfigKey));

var documentBlobPath = builder.Configuration.GetSection(DocumentStorageOptions.ConfigKey).GetValue(typeof(string), "DocumentBlobPath");

var mapperConfig = new MapperConfiguration( mc =>
{
    mc.AddProfile(new ClientProfile());
    mc.AddProfile(new DocumentMetadataProfile());
});

IMapper mapper = mapperConfig.CreateMapper();
builder.Services.AddSingleton(mapper);

builder.Services.AddScoped<IFileService, FileService>();

builder.Services.AddScoped<IRepository<Client>>(g =>
    new Repository<Client>(g.GetRequiredService<MainDbContext>()));

builder.Services.AddScoped<IRepository<DocumentMetadata>>(g =>
    new Repository<DocumentMetadata>(g.GetRequiredService<MainDbContext>()));

builder.Services.AddScoped<IClientsRepository, ClientsRepository>();
builder.Services.AddScoped<IDocumentsMetadataRepository, DocumentsMetadataRepository>();

builder.Services.AddScoped<IUnitOfWork>(d =>
 new UnitOfWork(d.GetRequiredService<MainDbContext>()));

builder.Services.AddScoped<IDocumentsMetadataService>(d =>
    new DocumentsMetadataService(d.GetRequiredService<IUnitOfWork>(), mapper));

builder.Services.AddScoped<IClientsService>(c =>
    new ClientsService(documentBlobPath.ToString(), c.GetRequiredService<IUnitOfWork>(), 
    c.GetRequiredService<IFileService>(), mapper));

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

    c.EnableAnnotations();
});

WebApplication app = builder.Build();

using var scope = app.Services.CreateScope();

var dbContext = scope.ServiceProvider.GetRequiredService<MainDbContext>();
dbContext.Database.Migrate();

var fileService = scope.ServiceProvider.GetRequiredService<IFileService>();
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

app.MapControllers();

app.Run();
