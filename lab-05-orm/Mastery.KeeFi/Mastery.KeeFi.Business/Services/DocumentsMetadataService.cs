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
using System.Reflection.Metadata;

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

        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public DocumentsMetadataService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            if (unitOfWork == null)
            {
                throw new ArgumentNullException(nameof(unitOfWork));
            }

            if (mapper == null)
            {
                throw new ArgumentNullException(nameof(mapper));
            }
            
            _unitOfWork = unitOfWork;
            _mapper = mapper;      
        }

        public async Task<DocumentMetadataDto> CreateDocumentAsync(int clientId, DocumentMetadataDto documentMetadataDto)
        {
            Validate(documentMetadataDto);

            var documentMetadata = _mapper.Map<DocumentMetadata>(documentMetadataDto);

            var client = _unitOfWork.Clients.Get(clientId);

            if (client == null)
            {
                var exception = new DocumentApiEntityNotFoundException($"The client with Id={clientId} is not found");
                throw exception;
            }

            documentMetadata.Client = client;

            _unitOfWork.Documents.Add(documentMetadata);
            _unitOfWork.SaveChanges();

            var documentDTO = _mapper.Map<DocumentMetadataDto>(documentMetadata);

            return documentDTO;
        }

        public async Task DeleteDocumentAsync(int clientId, int documentId)
        {
            var document = _unitOfWork.Documents.GetDocumentByClientId(clientId, documentId);

            if (document == null)
            {
                var exception =new DocumentApiEntityNotFoundException($"The document with Id={documentId} and ClientId={clientId} is not found");
                throw exception;
            }

            _unitOfWork.Documents.Remove(document);
            _unitOfWork.SaveChanges();
        }

        public async Task<DocumentMetadataDto> GetDocumentAsync(int clientId, int documentId)
        {
            var client = _unitOfWork.Clients.Get(clientId);

            if (client == null)
            {
                var exception = new DocumentApiEntityNotFoundException($"The client with Id={clientId} may be deleted");
                throw exception;
            }

            var document = _unitOfWork.Documents.GetDocumentByClientId(clientId, documentId);

            if (document == null)
            {
                var exception = new DocumentApiEntityNotFoundException($"The document with Id={documentId} and ClientId={clientId} is not found");
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
                throw exception;
            }
            if (take < 0)
            {
                var exception = new DocumentApiValidationException("Take must be more than 0");
                throw exception;
            }

            var allDocuments = _unitOfWork.Documents.GetDocuments(clientId, null, null);

            if (take > allDocuments?.Count())
            {
                var exception = new DocumentApiValidationException("Take is more than count of the documents");
                throw exception;
            }

            var documents = _unitOfWork.Documents.GetDocuments(clientId, skip, take);

            if (documents?.Any() == false)
            {
                var exception = new DocumentApiEntityNotFoundException($"There are no documents that belong to the client with Id={clientId}");
                throw exception;
            }

            var documentDTOs = _mapper.Map<List<DocumentMetadataDto>>(documents);

            return documentDTOs;
        }

        public async Task<DocumentMetadataDto> UpdateDocumentAsync(int clientId, int documentId, DocumentMetadataDto documentMetadataDto)
        {
            var document = _unitOfWork.Documents.GetDocumentByClientId(clientId, documentId);

            if (document == null)
            {
                var exception = new DocumentApiEntityNotFoundException($"Document not found id = {documentId}  clientId={clientId}");
                throw exception;
            }

            Validate(documentMetadataDto);
            
            document.FileName = documentMetadataDto.FileName;
            document.Title = documentMetadataDto.Title;
            document.Description = documentMetadataDto.Description;
            document.ContentLength = documentMetadataDto.ContentLength;
            document.ContentType = documentMetadataDto.ContentType;
            document.ContentMd5 = documentMetadataDto.ContentMd5;
            document.Properties = documentMetadataDto.Properties.Select(p => new DocumentMetadataProperty { DocumentId = documentId, Key = p.Key, Value = p.Value })
                .ToList();

            _unitOfWork.Documents.Update(document);
            _unitOfWork.SaveChanges();

            var documentDTO = _mapper.Map<DocumentMetadataDto>(document);

            return documentDTO;
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
                throw exception;
            }
        }
    }
}
