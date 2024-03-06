using AutoMapper;
using Mastery.KeeFi.Business.Dto;
using Mastery.KeeFi.Business.Interfaces;
using Mastery.KeeFi.Business.Exceptions;
using Mastery.KeeFi.Data.Interfaces;
using Mastery.KeeFi.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace Mastery.KeeFi.Business.Services
{
    public class DocumentsMetadataService : IDocumentsMetadataService
    {
        private readonly string[] _contentTypes =
        [
            "application/pdf",
            "text/html",
            "image/jpeg",
            "image/png",
            "application/msword",
            "application/vnd.openxmlformats-officedocument.wordprocessingml.document"
        ];

        private readonly Regex _regexFileName = new Regex("^[a-zA-Z0-9_.-]*$");
        private readonly Regex _regexProperties = new Regex("[a-zA-Z]");
        private readonly Regex _regexHexademicalNumbers = new Regex("[0-9a-fA-F]+");
        private readonly Regex _regexContentLength = new Regex("^[+]?\\d+([.]\\d+)?$");

        private readonly IClientsRepository _clientsRepository;
        private readonly IDocumentsMetadataRepository _documentsMetadataRepository;
        private readonly IMapper _mapper;
        private readonly ILogger<DocumentsMetadataService> _logger;

        public DocumentsMetadataService(IClientsRepository clientsRepository,
            IDocumentsMetadataRepository documentsMetadataRepository, IMapper mapper,
            ILogger<DocumentsMetadataService> logger)
        {
            if (clientsRepository == null)
            {
                throw new ArgumentNullException(nameof(clientsRepository));
            }

            if (documentsMetadataRepository == null)
            {
                throw new ArgumentNullException(nameof(documentsMetadataRepository));   
            }

            if (mapper == null)
            {
                throw new ArgumentNullException(nameof(mapper));
            }

            if (logger == null)
            {
                throw new ArgumentNullException(nameof(logger));
            }
            
            _clientsRepository = clientsRepository;
            _documentsMetadataRepository = documentsMetadataRepository;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<DocumentMetadata> CreateDocumentAsync(int clientId, DocumentMetadataDto documentMetadataDto)
        {
            Validate(documentMetadataDto);

            var documents = _documentsMetadataRepository.GetAll();

            var documentMetadata = _mapper.Map<DocumentMetadata>(documentMetadataDto);

            var client = _clientsRepository.GetClient(clientId);

            if (client == null)
            {
                var exception = new DocumentApiEntityNotFoundException($"The client with Id={clientId} is not found");
                _logger.LogError(exception, exception.Message, exception.StackTrace);
                throw exception;
            }

            documentMetadata.Id = (documents?.Count() == 0) ? 1 : documents.Max(d => d.Id) + 1;
            documentMetadata.ClientId = clientId;
           
            _documentsMetadataRepository.Add(documentMetadata);
            _documentsMetadataRepository.SaveChanges();

            _logger.LogInformation($"The document with Id={documentMetadata.Id} and ClientId={clientId} has been successfully created");

            return documentMetadata;
        }

        public async Task DeleteDocumentAsync(int clientId, int documentId)
        {
            var document = _documentsMetadataRepository.GetDocument(clientId, documentId);

            if (document == null)
            {
                var exception =new DocumentApiEntityNotFoundException($"The document with Id={documentId} and ClientId={clientId} is not found");
                _logger.LogError(exception, exception.Message, exception.StackTrace);
                throw exception;
            }

            _documentsMetadataRepository.Remove(document);
            _documentsMetadataRepository.SaveChanges();

            _logger.LogInformation($"The document with Id={documentId} and ClientId={clientId} has been successfully removed");
        }

        public async Task<DocumentMetadataDto> GetDocumentAsync(int clientId, int documentId)
        {
            var client = _clientsRepository.GetClient(clientId);

            if (client == null)
            {
                var exception = new DocumentApiEntityNotFoundException($"The client with Id={clientId} may be deleted");
                _logger.LogError(exception, exception.Message, exception.StackTrace);
                throw exception;
            }

            var document = _documentsMetadataRepository.GetDocument(clientId, documentId);

            if (document == null)
            {
                var exception = new DocumentApiEntityNotFoundException($"The document with Id={documentId} and ClientId={clientId} is not found");
                _logger.LogError(exception, exception.Message, exception.StackTrace);
                throw exception;
            }

            var documentDTO = _mapper.Map<DocumentMetadataDto>(document);   

            return documentDTO;
        }

        public async Task<IEnumerable<DocumentMetadataDto>> ListDocumentsAsync(int clientId, int? skip, int? take)
        {
            if (skip < 0)
            {
                var exception = new DocumentApiValidationException("Skip must be more than 0");
                _logger.LogError(exception, exception.Message, exception.StackTrace);
                throw exception;
            }
            if (take < 0)
            {
                var exception = new DocumentApiValidationException("Take must be more than 0");
                _logger.LogError(exception, exception.Message, exception.StackTrace);
                throw exception;
            }

            var deserializedDocuments = _documentsMetadataRepository.GetAll();

            if (take > deserializedDocuments?.Count())
            {
                var exception = new DocumentApiValidationException("Take is more than count of the documents");
                _logger.LogError(exception, exception.Message, exception.StackTrace);
                throw exception;
            }

            var documents = _documentsMetadataRepository.GetDocuments(clientId, skip, take);

            if (documents?.Any() == false)
            {
                var exception = new DocumentApiEntityNotFoundException($"There are no documents that blong to the client with Id={clientId}");
                _logger.LogError(exception, exception.Message, exception.StackTrace);
                throw exception;
            }

            var documentDTOs = _mapper.Map<List<DocumentMetadataDto>>(documents);

            return documentDTOs;
        }

        public async Task<DocumentMetadata> UpdateDocumentAsync(int clientId, int documentId, DocumentMetadataDto documentMetadataDto)
        {
            var document = _documentsMetadataRepository.GetDocument(clientId, documentId);

            if (document == null)
            {
                var exception = new DocumentApiEntityNotFoundException($"Document not found id = {documentId}  clientId={clientId}");
                _logger.LogError(exception, exception.Message, exception.StackTrace);
                throw exception;
            }

            Validate(documentMetadataDto);
            
            document.FileName = documentMetadataDto.FileName;
            document.Title = documentMetadataDto.Title;
            document.Description = documentMetadataDto.Description;
            document.ContentLength = documentMetadataDto.ContentLength;
            document.ContentType = documentMetadataDto.ContentType;
            document.ContentMd5 = documentMetadataDto.ContentMd5;
            document.Properties = documentMetadataDto.Properties;

            _documentsMetadataRepository.Update(document);
            _documentsMetadataRepository.SaveChanges();

            _logger.LogInformation($"The document with ClientId={clientId} and Id={documentId} has been successfully updated");

            return document;
        }

        private void Validate(DocumentMetadataDto documentMetadataDto)
        {
            List<string> exceptionMessages = new List<string>();

            if (string.IsNullOrEmpty(documentMetadataDto.FileName))
            {
                exceptionMessages.Add("Please, fill the FileName out");
            }
            if (documentMetadataDto.FileName.Length > 255)
            {
                exceptionMessages.Add("The length of the FileName must be not more than 255 symbols");
            }
            if (!_regexFileName.IsMatch(documentMetadataDto.FileName))
            {
                exceptionMessages.Add("The FileName must consist of English alphanumeric letters and digits, ., -, _");
            }

            if (string.IsNullOrEmpty(documentMetadataDto.Title))
            {
                exceptionMessages.Add("Plese, fill the Title out");
            }
            if (documentMetadataDto?.Title?.Length > 150)
            {
                exceptionMessages.Add("The length of the Title must be more than 150 symbols");
            }

            if (documentMetadataDto?.Description?.Length > 400)
            {
                exceptionMessages.Add("The length of the Description must be mot more than 400 symbols");
            }

            if (documentMetadataDto?.ContentLength == 0)
            {
                exceptionMessages.Add("The ContentLength must be more than 0");
            }
            if (!_regexContentLength.IsMatch(documentMetadataDto.ContentLength.ToString()))
            {
                exceptionMessages.Add("The ContentLength must contain only of the numbers");
            }

            if (documentMetadataDto?.Properties.Keys.Count > 10)
            {
                exceptionMessages.Add("The count of keys must be more than 0");
            }
            if (documentMetadataDto?.Properties.Keys.Distinct().Count() != documentMetadataDto?.Properties.Count)
            {
                exceptionMessages.Add("All the keys must be unique");
            }
            if ((bool)(documentMetadataDto?.Properties.Keys.Any(k => k.Length > 20)))
            {
                exceptionMessages.Add("The length of the key of the Properties must be not more than 20 symbols");
            }
            if (documentMetadataDto.Properties.Keys.Any(k => !_regexProperties.IsMatch(k)))
            {
                exceptionMessages.Add("All the keys of the Properites must consist of English letters only");
            }

            if (string.IsNullOrEmpty(documentMetadataDto.ContentType))
            {
                exceptionMessages.Add("Please, fill the ContentType out");
            }
            if (documentMetadataDto.ContentType.Length > 100)
            {
                exceptionMessages.Add("The length of the ContentType must be not more than 100 symbols");
            }
            if (!_contentTypes.Contains(documentMetadataDto.ContentType))
            {
                exceptionMessages.Add("The ContentType is unknown");
            }

            if (string.IsNullOrEmpty(documentMetadataDto.ContentMd5))
            {
                exceptionMessages.Add("Please, fill the ContenMD5 out");
            }
            if (documentMetadataDto.ContentMd5.Length < 32)
            {
                exceptionMessages.Add("The ContentMD5 is too short");
            }
            if (documentMetadataDto.ContentMd5.Length > 32)
            {
                exceptionMessages.Add("The ContentMD5 is too long");
            }
            if (!_regexHexademicalNumbers.IsMatch(documentMetadataDto.ContentMd5))
            {
                exceptionMessages.Add("The ContentMD5 must consist of hexademical numbers only");
            }

            if (exceptionMessages.Any())
            {
                string exceptionMessage = string.Join(". ", exceptionMessages);
                var exception = new DocumentApiValidationException(exceptionMessage);
                _logger.LogError(exception, exception.Message, exception.StackTrace);
                throw exception;
            }
        }
    }
}
