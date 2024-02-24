using GTE.Mastery.Documents.Api.Attributes;
using Swashbuckle.AspNetCore.Annotations;
using System.ComponentModel;

namespace GTE.Mastery.Documents.Api.Entities
{
    [SwaggerSchema(Title = "Client", Description = "A client in the system.")]
    [DoNotModify]
    public sealed class Client
    {
        [ReadOnly(true)]
        public int Id { get; set; }

        [SwaggerSchema(Title = "Client first name", Description = "The first name of the client.")]
        public string? FirstName { get; set; }

        [SwaggerSchema(Title = "Client last name", Description = "The last name of the client.")]
        public string? LastName { get; set; }

        [SwaggerSchema(Title = "Client date of birth", Description = "The date of birth of the client.")]
        public DateOnly? DateOfBirth { get; set; }

        [SwaggerSchema(Title = "Client tags", Description = "The tags of the client.")]
        public string[] Tags { get; set; } = Array.Empty<string>();
        
    }
}
