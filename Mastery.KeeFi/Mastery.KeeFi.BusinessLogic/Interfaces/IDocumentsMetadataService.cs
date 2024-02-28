using Mastery.KeeFi.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mastery.KeeFi.BusinessLogic.Interfaces
{
    public interface IDocumentsMetadataService
    {
        Task<IEnumerable<DocumentMetadata>> ListDocumentsAsync(int clientId, int? skip, int? take);

        Task<DocumentMetadata> GetDocumentAsync(int clientId, int documentId);

        Task<DocumentMetadata> CreateDocumentAsync(int clientId, DocumentMetadata documentMetadata);

        Task<DocumentMetadata> UpdateDocumentAsync(int clientId, int documentId, DocumentMetadata documentMetadata);

        Task DeleteDocumentAsync(int clientId, int documentId);
    }
}
