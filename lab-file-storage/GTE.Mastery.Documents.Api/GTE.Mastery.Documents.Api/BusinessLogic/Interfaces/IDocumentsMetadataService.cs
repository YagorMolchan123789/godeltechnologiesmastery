using GTE.Mastery.Documents.Api.Attributes;

namespace GTE.Mastery.Documents.Api.BusinessLogic.Interfaces
{
    [DoNotModify]
    public interface IDocumentsMetadataService
    {
        /// <summary>
        /// Retrieves a list of document metadata for a specified client, with optional pagination.
        /// </summary>
        /// <param name="clientId">The unique identifier of the client.</param>
        /// <param name="skip">The number of records to skip for pagination. If no value specified, should rely on 0. Boundaries are [0; int.MaxValue).</param>
        /// <param name="take">The number of records to take for pagination. If no value specified, should rely on 10. Boundaries are (0; 20].</param>
        Task<IEnumerable<DocumentMetadata>> ListDocumentsAsync(int clientId, int? skip, int? take);

        /// <summary>
        /// Retrieves metadata for the document of the client.
        /// </summary>
        /// <param name="clientId">The unique identifier of the client.</param>
        /// <param name="documentId">The unique identifier of the document.</param>
        Task<DocumentMetadata> GetDocumentAsync(int clientId, int documentId);

        /// <summary>
        /// Creates a new document metadata record for a specified client.
        /// </summary>
        /// <param name="clientId">The unique identifier of the client.</param>
        /// <param name="documentMetadata">The document metadata to create.</param>
        Task<DocumentMetadata> CreateDocumentAsync(int clientId, DocumentMetadata documentMetadata);

        /// <summary>
        /// Updates the metadata of the document for the client.
        /// </summary>
        /// <param name="clientId">The unique identifier of the client.</param>
        /// <param name="documentId">The unique identifier of the document to update.</param>
        /// <param name="documentMetadata">The updated document metadata.</param>
        Task<DocumentMetadata> UpdateDocumentAsync(int clientId, int documentId, DocumentMetadata documentMetadata);

        /// <summary>
        /// Deletes the metadata record of the document for the client.
        /// </summary>
        /// <param name="clientId">The unique identifier of the client.</param>
        /// <param name="documentId">The unique identifier of the document to delete.</param>
        Task DeleteDocumentAsync(int clientId, int documentId);

        Task<IEnumerable<DocumentMetadata>> GetDocumentsAsync(int clientId);
    }
}
