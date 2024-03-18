using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;

namespace Mastery.KeeFi.Domain.Entities
{
    public class DocumentMetadataProperty
    {
        public int Id { get; set; }

        public string Key { get; set; }

        public string Value { get; set; }

        public int DocumentId { get; set; }

        public DocumentMetadata Document { get; set; }

    }
}
