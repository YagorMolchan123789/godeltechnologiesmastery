using Bogus;
using GTE.Mastery.Documents.Api.Exceptions;
using System.Text.Json;
using System.Security.Cryptography;
using System.Reflection.Metadata;
using GTE.Mastery.Documents.Api.Entities;
using DocumentContent = GTE.Mastery.Documents.Api.Entities.DocumentContent;

namespace GTE.Mastery.Documents.Api.BusinessLogic
{
    public class DocumentsContentService : IDocumentsContentService
    {
        private readonly string _blobPath;
        private readonly string _contentPath;
        private readonly IDocumentsMetadataService _documentsMetadataService;

        private readonly int _maxContentLength = 1000000;

        public DocumentsContentService(string blobPath, string contentPath, IDocumentsMetadataService documentsMetadataService)
        {
            _blobPath = blobPath;
            _contentPath = contentPath;
            _documentsMetadataService = documentsMetadataService;
        }

        public async Task UploadDocumentAsync(int clientId, int documentId, MemoryStream content)
        {
            var document = await _documentsMetadataService.GetDocumentAsync(clientId, documentId);

            if (document == null)
            {
                throw new DocumentApiEntityNotFoundException("The document with such Id is not found");
            }

            DocumentMetadata documentMetadata = new DocumentMetadata
            {
                FileName = document.FileName,
                Title = document.Title,
                ContentType = document.ContentType,
                ContentLength = (int)content.Length,
                ContentMd5 = CalculateMd5Hash(content)
            };

            DocumentMetadata metadata = await _documentsMetadataService.CreateDocumentAsync(clientId, documentMetadata);

            var contentsJson = File.ReadAllText(_contentPath);
            var contents = JsonSerializer.Deserialize<List<DocumentContent>>(contentsJson);

            DocumentContent documentContent = new DocumentContent
            {
                Id = (contents?.Count == 0) ? 1 : contents.Max(c => c.Id) +1,
                Metadata = metadata,
                Blob = content.ToArray(),
                Md5Hash = metadata.ContentMd5
            };

            Validate(documentContent);
            contents.Add(documentContent);
            var serializedContents = JsonSerializer.Serialize(contents);
            File.WriteAllText(_contentPath, serializedContents);
        }

        public async Task<ReceiveDocumentResponse> ReceiveDocumentAsync(int clientId, int documentId)
        {
            var contentsJson = File.ReadAllText(_contentPath);
            var contents = JsonSerializer.Deserialize<List<DocumentContent>>(contentsJson);
            DocumentContent documentContent = contents.FirstOrDefault(c => c.Metadata.ClientId == clientId &&
                c.Id == documentId);

            if (documentContent == null)
            {
                throw new DocumentApiEntityNotFoundException("The document with such Id is not found");
            }

            Validate(documentContent);

            string targetPath = _blobPath + "\\" + documentContent.Metadata?.FileName;

            MemoryStream blobStream = new MemoryStream();
            blobStream.Write(documentContent.Blob, 0, documentContent.Metadata.ContentLength);
            blobStream.Position = 0;

            FileStream metadataStream = new FileStream(targetPath, FileMode.Create, FileAccess.Write);
            metadataStream.Write(blobStream.ToArray(), 0, documentContent.Metadata.ContentLength);
                      

            ReceiveDocumentResponse receiveDocumentResponse = new(blobStream, documentContent.Metadata);
            return receiveDocumentResponse;
        }

        private void Validate(DocumentContent documentContent)
        {
            List<string> exceptionMessages = new List<string>();

            if (documentContent.Metadata.ContentLength > _maxContentLength)
            {
                exceptionMessages.Add($"The storage does not support files larger than {_maxContentLength * Math.Pow(10, -6)} in size");
            }
            if (documentContent.Blob.Length != documentContent.Metadata.ContentLength)
            {
                exceptionMessages.Add("A size of the file must be equal to the size of the uploaded blob");
            }
            if (documentContent.Md5Hash != documentContent.Metadata.ContentMd5)
            {
                exceptionMessages.Add("Md5 hash of the file does not match the md5 hash in metadata");
            }
            
            if (exceptionMessages.Any())
            {
                string exceptionMessage = string.Join(". ", exceptionMessages);
                throw new DocumentApiValidationException(exceptionMessage);
            }
        }

        private string CalculateMd5Hash(MemoryStream content)
        {
            MD5 md5 = MD5.Create();
            byte[] hashBytes = md5.ComputeHash(content);
            return Convert.ToHexString(hashBytes).ToLower();
        }

    }
}
