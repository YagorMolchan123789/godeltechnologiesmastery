using GTE.Mastery.Documents.Api.Attributes;

namespace GTE.Mastery.Documents.Api.BusinessLogic.Interfaces
{
    [DoNotModify]
    public record ReceiveDocumentResponse(Stream Content, DocumentMetadata Metadata);

    [DoNotModify]
    public interface IDocumentsContentService
    {
        /// <summary>
        /// Uploads a document's content for a specified client and document.
        /// </summary>
        /// <param name="clientId">The unique identifier of the client associated with the document.</param>
        /// <param name="documentId">The unique identifier of the document to upload.</param>
        /// <param name="content">The content of the document.</param>
        Task UploadDocumentAsync(int clientId, int documentId, MemoryStream content);

        /// <summary>
        /// Downloads the content of a specified document for a client.
        /// </summary>
        /// <param name="clientId">The unique identifier of the client associated with the document.</param>
        /// <param name="documentId">The unique identifier of the document to download.</param>
        Task<ReceiveDocumentResponse> ReceiveDocumentAsync(int clientId, int documentId);
    }
}
