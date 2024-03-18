using Mastery.KeeFi.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mastery.KeeFi.Data.Interfaces
{
    public interface IDocumentsMetadataRepository : IRepository<DocumentMetadata>
    {
        IEnumerable<DocumentMetadata> GetDocuments(int clientId, int? skip, int? take);

        DocumentMetadata GetDocumentByClientId(int clientId, int documentId);
    }
}
