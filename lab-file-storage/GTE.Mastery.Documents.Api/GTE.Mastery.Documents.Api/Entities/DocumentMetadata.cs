﻿using GTE.Mastery.Documents.Api.Attributes;
using Swashbuckle.AspNetCore.Annotations;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace GTE.Mastery.Documents.Api.Entities
{
    [SwaggerSchema(Title = "DocumentMetadata", Description = "Metadata for a document in the system.")]
    [DoNotModify]
    public class DocumentMetadata
    {
        [ReadOnly(true)]
        public int Id { get; set; }

        [SwaggerSchema(Title = "Client ID", Description = "The unique identifier of the client associated with the document.")]
        [ReadOnly(true)]
        public int? ClientId { get; set; }

        [Required]
        [SwaggerSchema(Title = "Document name", Description = "The name of the document.")]
        public string FileName { get; set; }

        [SwaggerSchema(Title = "Document title", Description = "The title of the document.")]
        public string? Title { get; set; }

        [SwaggerSchema(Title = "Document description", Description = "The description of the document.")]
        public string? Description { get; set; }

        [SwaggerSchema(Title = "Document size", Description = "The size of the document.")]
        public int ContentLength { get; set; }

        [SwaggerSchema(Title = "Document content type", Description = "The content type of the document.")]
        public string ContentType { get; set; }

        [SwaggerSchema(Title = "Document content MD5", Description = "The MD5 hash of the document content.")]
        public string ContentMd5 { get; set; }

        [SwaggerSchema(Title = "Document extra properties", Description = "The extra properties of the document.")]
        public Dictionary<string, string> Properties { get; set; } = new();
    }
}
