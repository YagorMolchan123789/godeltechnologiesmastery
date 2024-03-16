using Mastery.KeeFi.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mastery.KeeFi.Data.Interfaces
{
    public interface IDocumentsMetadataRepository
    {
        List<DocumentMetadata> Documents { get; }

        List<DocumentMetadata> GetDocuments(int clientId, int? skip, int? take);

        DocumentMetadata GetDocument(int clientId, int documentId);

        void Add(DocumentMetadata document);

        void Update(DocumentMetadata document);

        void Remove(DocumentMetadata document);
    }
}
