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
        private readonly IDocumentsMetadataService _documentsMetadataService;
        private readonly IClientsService _clientsService;
        private readonly IFileService _fileService;

        private readonly int _maxContentLength = 1000000;

        public DocumentsContentService(string blobPath, IDocumentsMetadataService documentsMetadataService, IClientsService clientsService, IFileService fileService)
        {
            _blobPath = blobPath;
            _documentsMetadataService = documentsMetadataService;
            _clientsService = clientsService;
            _fileService = fileService;
        }

        public async Task UploadDocumentAsync(int clientId, int documentId, MemoryStream content)
        {
            var client = await _clientsService.GetClientAsync(clientId);
            var document = await _documentsMetadataService.GetDocumentAsync(clientId, documentId);

            if ( client == null)
            {
                throw new DocumentApiEntityNotFoundException("The client with such Id is not found");
            }
            if (document == null)
            {
                throw new DocumentApiEntityNotFoundException("The document with such Id is not found");
            }            

            ValidateUpload(content);

            var contentMd5Hash = CalculateMd5Hash(content);

            document.ContentLength = (int)content.Length;
            document.ContentMd5 = contentMd5Hash;

            DocumentMetadata metadata = await _documentsMetadataService.UpdateDocumentAsync(clientId, documentId, document);
            string targetDirectory = _blobPath + "/" + client.ToString();
            string targetPath = targetDirectory + "/" + metadata.FileName;

            _fileService.CreateDirectory(targetDirectory);

            FileStream uploadStream = new FileStream(targetPath, FileMode.Create, FileAccess.Write);
            uploadStream.Write(content.ToArray(), 0, metadata.ContentLength);
            uploadStream.Close();
            
        }

        public async Task<ReceiveDocumentResponse> ReceiveDocumentAsync(int clientId, int documentId)
        {
            var client = await _clientsService.GetClientAsync(clientId);
            var document = await _documentsMetadataService.GetDocumentAsync(clientId, documentId);

            if (client == null)
            {
                throw new DocumentApiEntityNotFoundException("The client with such Id is not found");
            }
            if (document == null)
            {
                throw new DocumentApiEntityNotFoundException("The document with such Id is not found");
            }

            string targetDirectory = _blobPath + "/" + client.ToString();
            string targetPath = targetDirectory + "/" + document.FileName;

            if (_fileService.Exists(targetPath) == false)
            {
                throw new DocumentApiEntityNotFoundException("The file does not exist");
            }

            MemoryStream content = new MemoryStream();
            byte[] buffer = new byte[document.ContentLength];

            FileStream metadataStream = new FileStream(targetPath, FileMode.Open, FileAccess.Read);
            metadataStream.Read(buffer, 0, buffer.Length);
            content.Write(buffer, 0, buffer.Length);

            ValidateDownload(content, document);
            FileStream downloadStream = new FileStream(string.Concat(_blobPath, "/", document.FileName), FileMode.Create, FileAccess.Write);
            content.Position = 0;
            downloadStream.Write(content.ToArray(), 0, document.ContentLength);

            metadataStream.Close();
            downloadStream.Close();

            ReceiveDocumentResponse receiveDocumentResponse = new(content, document);
            return receiveDocumentResponse;
        }

        private void ValidateUpload(MemoryStream content)
        {
            List<string> exceptionMessages = new List<string>();            

            if (content.ToArray().Length > _maxContentLength)
            {
                exceptionMessages.Add($"The storage does not support files larger than {_maxContentLength * Math.Pow(10, -6)} in size");
            }

            if (exceptionMessages.Any())
            {
                string exceptionMessage = string.Join(". ", exceptionMessages);
                throw new DocumentApiValidationException(exceptionMessage);
            }
        }

        private void ValidateDownload(MemoryStream content, DocumentMetadata metadata)
        {
            List<string> exceptionMessages = new List<string>();
            var md5Hash = CalculateMd5Hash(content);

            if (md5Hash != metadata.ContentMd5)
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
            return Convert.ToHexString(MD5.Create().ComputeHash(content.ToArray())).ToLower();
        }

    }
}
