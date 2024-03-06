using Mastery.KeeFi.Business.Dto;
using Mastery.KeeFi.Business.Interfaces;
using Mastery.KeeFi.Business.Exceptions;
using Mastery.KeeFi.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace Mastery.KeeFi.Business.Services
{
    public class DocumentsContentService : IDocumentsContentService
    {
        private readonly string _blobPath;
        private readonly IDocumentsMetadataService _documentsMetadataService;
        private readonly IClientsService _clientsService;
        private readonly IFileService _fileService;

        private readonly ILogger<DocumentsContentService> _logger;

        private readonly int _maxContentLength = 1048576;

        public DocumentsContentService(string blobPath, IDocumentsMetadataService documentsMetadataService,
            IClientsService clientsService, IFileService fileService,
            ILogger<DocumentsContentService> logger)
        {
            if (string.IsNullOrEmpty(blobPath))
            {
                throw new ArgumentNullException(nameof(blobPath));  
            }

            if (documentsMetadataService == null)
            {
                throw new ArgumentNullException(nameof(documentsMetadataService));
            }

            if (clientsService == null)
            {
                throw new ArgumentNullException(nameof(clientsService));
            }

            if (fileService == null)
            {
                throw new ArgumentNullException(nameof(fileService));
            }

            if (logger == null)
            {
                throw new ArgumentNullException(nameof(logger));    
            }

            _blobPath = blobPath;
            _documentsMetadataService = documentsMetadataService;
            _clientsService = clientsService;
            _fileService = fileService;
            _logger = logger;
        }

        public async Task UploadDocumentAsync(int clientId, int documentId, MemoryStream content)
        {
            _logger.LogWarning($"The file storage supports the maximal size of files in {_maxContentLength} bytes");

            var client = await _clientsService.GetClientAsync(clientId);
            var document = await _documentsMetadataService.GetDocumentAsync(clientId, documentId);

            string targetDirectory = Path.Combine(_blobPath, client.Id.ToString());
            string sourcePath = Path.Combine(targetDirectory, document.FileName);

            if (_fileService.Exists(sourcePath))
            {   
                var exception = new DocumentApiValidationException($"This file {document.FileName} already exists");
                _logger.LogError(exception, exception.Message, exception.StackTrace);
                throw exception;
            }

            ValidateUpload(content);

            var contentMd5Hash = CalculateMd5Hash(content);

            document.ContentLength = (int)content.Length;
            document.ContentMd5 = contentMd5Hash;

            DocumentMetadata metadata = await _documentsMetadataService.UpdateDocumentAsync(clientId, documentId, document);

            string targetPath = Path.Combine(targetDirectory, metadata.FileName);

            _fileService.CreateDirectory(targetDirectory);

            FileStream uploadStream = new FileStream(targetPath, FileMode.Create, FileAccess.Write);
            uploadStream.Write(content.ToArray(), 0, metadata.ContentLength);
            uploadStream.Close();

            _logger.LogInformation($"The file {document.FileName} is uploaded");
        }

        public async Task<ReceiveDocumentResponse> ReceiveDocumentAsync(int clientId, int documentId)
        {
            _logger.LogWarning("The md5 hash of saved file must match md5 hash in metadata");

            var client = await _clientsService.GetClientAsync(clientId);
            var document = await _documentsMetadataService.GetDocumentAsync(clientId, documentId);

            string targetDirectory = Path.Combine(_blobPath, client.Id.ToString());
            string targetPath = Path.Combine(targetDirectory, document.FileName);

            if (!_fileService.Exists(targetPath))
            {
                var exception = new DocumentApiEntityNotFoundException($"The file {document.FileName} does not exist");
                _logger.LogError(exception, exception.Message, exception.StackTrace);
                throw exception;
            }

            MemoryStream content = new MemoryStream();
            byte[] buffer = new byte[document.ContentLength];

            FileStream metadataStream = new FileStream(targetPath, FileMode.Open, FileAccess.Read);

            metadataStream.Read(buffer, 0, buffer.Length);

            content.Write(buffer, 0, buffer.Length);

            ValidateDownload(content, document);
            FileStream downloadStream = new FileStream(Path.Combine(_blobPath, document.FileName), FileMode.Create, FileAccess.Write);
            content.Position = 0;
            downloadStream.Write(content.ToArray(), 0, document.ContentLength);

            metadataStream.Close();
            downloadStream.Close();

            _logger.LogInformation($"The file {document.FileName} is downloaded. ClientId={clientId}. DocumentId={documentId}");

            ReceiveDocumentResponse receiveDocumentResponse = new(content, document);
            return receiveDocumentResponse;
        }

        private void ValidateUpload(MemoryStream content)
        {
            if (content.ToArray().Length > _maxContentLength)
            {
                var exception = new DocumentApiValidationException($"The storage does not support files larger than {_maxContentLength * Math.Pow(10, -6)} M in size");
                _logger.LogError(exception, exception.Message, exception.StackTrace);
                throw exception;
            }
        }

        private void ValidateDownload(MemoryStream content, DocumentMetadataDto metadata)
        {
            var md5Hash = CalculateMd5Hash(content);

            if (md5Hash != metadata.ContentMd5)
            {
                var exception = new DocumentApiValidationException("Md5 hash of the file does not match the md5 hash in metadata");
                _logger.LogError(exception, exception.Message, exception.StackTrace);
                throw exception;
            }
        }

        private string CalculateMd5Hash(MemoryStream content)
        {
            return Convert.ToHexString(MD5.Create().ComputeHash(content.ToArray())).ToLower();
        }
    }
}
