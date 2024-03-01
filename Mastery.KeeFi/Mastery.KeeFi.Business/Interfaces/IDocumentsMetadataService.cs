using Mastery.KeeFi.Business.DTO;
using Mastery.KeeFi.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mastery.KeeFi.Business.Interfaces
{
    public interface IDocumentsMetadataService
    {
        Task<IEnumerable<DocumentMetadataDTO>> ListDocumentsAsync(int clientId, int? skip, int? take);

        Task<DocumentMetadataDTO> GetDocumentAsync(int clientId, int documentId);

        Task<DocumentMetadata> CreateDocumentAsync(int clientId, DocumentMetadataDTO documentMetadataDTO);

        Task<DocumentMetadata> UpdateDocumentAsync(int clientId, int documentId, DocumentMetadataDTO documentMetadataDTO);

        Task DeleteDocumentAsync(int clientId, int documentId);
    }
}
