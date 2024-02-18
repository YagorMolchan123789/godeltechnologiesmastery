
namespace GTE.Mastery.Documents.Api.BusinessLogic
{
    public class DocumentsMetadataService : IDocumentsMetadataService
    {

        private string _filePath;

        public DocumentsMetadataService(string filePath)
        {
            _filePath = filePath;
        }

        public Task<DocumentMetadata> CreateDocumentAsync(int clientId, DocumentMetadata documentMetadata)
        {
            throw new NotImplementedException();
        }

        public Task DeleteDocumentAsync(int clientId, int documentId)
        {
            throw new NotImplementedException();
        }

        public Task<DocumentMetadata> GetDocumentAsync(int clientId, int documentId)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<DocumentMetadata>> ListDocumentsAsync(int clientId, int? skip, int? take)
        {
            throw new NotImplementedException();
        }

        public Task<DocumentMetadata> UpdateDocumentAsync(int clientId, int documentId, DocumentMetadata documentMetadata)
        {
            throw new NotImplementedException();
        }
    }
}
