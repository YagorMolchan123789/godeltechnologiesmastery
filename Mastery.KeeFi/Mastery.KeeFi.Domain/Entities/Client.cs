using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Swashbuckle.AspNetCore.Annotations;


namespace Mastery.KeeFi.Domain.Entities
{
    [SwaggerSchema(Title = "Client", Description = "A client in the system.")]
    public class Client
    {
        [ReadOnly(true)]
        public int Id { get; set; }

        [SwaggerSchema(Title = "Client first name", Description = "The first name of the client.")]
        public string FirstName { get; set; }

        [SwaggerSchema(Title = "Client last name", Description = "The last name of the client.")]
        public string LastName { get; set; }

        [SwaggerSchema(Title = "Client date of birth", Description = "The date of birth of the client.")]
        public DateOnly? DateOfBirth { get; set; }

        [SwaggerSchema(Title = "Client tags", Description = "The tags of the client.")]
        public string[] Tags { get; set; } = Array.Empty<string>();
    }
}
