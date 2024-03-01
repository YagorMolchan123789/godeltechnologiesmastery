using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Swashbuckle.AspNetCore.Annotations;

namespace Mastery.KeeFi.Domain.Entities
{
    public class DocumentMetadata
    {
        public int Id { get; set; }

        public int? ClientId { get; set; }
        
        public required string FileName { get; set; }

        public string? Title { get; set; }

        public string? Description { get; set; }

        public int ContentLength { get; set; }

        public string ContentType { get; set; }
        
        public string ContentMd5 { get; set; }

        public Dictionary<string, string> Properties { get; set; }
    }
}
