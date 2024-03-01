using Mastery.KeeFi.Business.DTO;
using Mastery.KeeFi.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mastery.KeeFi.Business.Interfaces
{
    public record ReceiveDocumentResponse(Stream Content, DocumentMetadataDTO Metadata);

    public interface IDocumentsContentService
    {
        Task UploadDocumentAsync(int clientId, int documentId, MemoryStream content);

        Task<ReceiveDocumentResponse> ReceiveDocumentAsync(int clientId, int documentId);
    }
}
