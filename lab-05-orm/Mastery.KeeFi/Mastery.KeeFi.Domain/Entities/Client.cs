using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Swashbuckle.AspNetCore.Annotations;


namespace Mastery.KeeFi.Domain.Entities
{
    public class Client
    {
        public int Id { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public DateOnly? DateOfBirth { get; set; }

        public List<string> Tags { get; set; }

        public virtual ICollection<DocumentMetadata>? Documents { get; set; }
    }
}
