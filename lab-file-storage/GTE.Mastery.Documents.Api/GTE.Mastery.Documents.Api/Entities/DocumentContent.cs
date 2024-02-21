using System.Runtime.CompilerServices;

namespace GTE.Mastery.Documents.Api.Entities
{
    public class DocumentContent
    {
        public int Id { get; set; }

        public DocumentMetadata Metadata { get; set; }

        public byte[] Blob { get; set; }

        public string Md5Hash { get; set; }
    }
}
