﻿using AutoMapper;
using Mastery.KeeFi.Business.DTO;
using Mastery.KeeFi.Business.Interfaces;
using Mastery.KeeFi.Common.Exceptions;
using Mastery.KeeFi.Data.Interfaces;
using Mastery.KeeFi.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Mastery.KeeFi.Business.Services
{
    public class DocumentsMetadataService : IDocumentsMetadataService
    {
        private string _filePath;

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

        public DocumentsMetadataService(string filePath, IClientsRepository clientsRepository,
            IDocumentsMetadataRepository documentsMetadataRepository, IMapper mapper)
        {
            _filePath = filePath;
            _clientsRepository = clientsRepository;
            _documentsMetadataRepository = documentsMetadataRepository;
            _mapper = mapper;
        }

        public async Task<DocumentMetadata> CreateDocumentAsync(int clientId, DocumentMetadataDto documentMetadataDto)
        {
            Validate(documentMetadataDto);

            var documents = _documentsMetadataRepository.GetAll();

            var documentMetadata = _mapper.Map<DocumentMetadata>(documentMetadataDto);

            var client = _clientsRepository.GetClient(clientId);

            if (client == null)
            {
                throw new DocumentApiEntityNotFoundException($"The client with Id={clientId} is not found");
            }

            documentMetadata.Id = (documents?.Count() == 0) ? 1 : documents.Max(d => d.Id) + 1;
            documentMetadata.ClientId = clientId;
           
            _documentsMetadataRepository.Add(documentMetadata);
            _documentsMetadataRepository.SaveChanges();

            return documentMetadata;
        }

        public async Task DeleteDocumentAsync(int clientId, int documentId)
        {
            var document = _documentsMetadataRepository.GetDocument(clientId, documentId);

            if (document == null)
            {
                throw new DocumentApiEntityNotFoundException($"The document with Id={documentId} and ClientId={clientId} is not found");
            }

            _documentsMetadataRepository.Remove(document);
            _documentsMetadataRepository.SaveChanges();
        }

        public async Task<DocumentMetadataDto> GetDocumentAsync(int clientId, int documentId)
        {
            var client = _clientsRepository.GetClient(clientId);

            if (client == null)
            {
                throw new DocumentApiEntityNotFoundException($"The client with Id={clientId} may be deleted");
            }

            var document = _documentsMetadataRepository.GetDocument(clientId, documentId);

            if (document == null)
            {
                throw new DocumentApiEntityNotFoundException($"The document with Id={documentId} and ClientId={clientId} is not found");
            }

            var documentDTO = _mapper.Map<DocumentMetadataDto>(document);   

            return documentDTO;
        }

        public async Task<IEnumerable<DocumentMetadataDto>> ListDocumentsAsync(int clientId, int? skip, int? take)
        {
            if (skip < 0)
            {
                throw new DocumentApiValidationException("Skip must be more than 0");
            }
            if (take < 0)
            {
                throw new DocumentApiValidationException("Take must be more than 0");
            }

            var deserializedDocuments = _documentsMetadataRepository.GetAll();

            if (take > deserializedDocuments?.Count())
            {
                throw new DocumentApiValidationException("Take is more than count of the documents");
            }

            var documents = _documentsMetadataRepository.GetDocuments(clientId, skip, take);

            if (documents?.Any() == false)
            {
                throw new DocumentApiEntityNotFoundException($"There are no documents that blong to the client with Id={clientId}");
            }

            var documentDTOs = _mapper.Map<List<DocumentMetadataDto>>(documents);

            return documentDTOs;
        }

        public async Task<DocumentMetadata> UpdateDocumentAsync(int clientId, int documentId, DocumentMetadataDto documentMetadataDto)
        {
            var document = _documentsMetadataRepository.GetDocument(clientId, documentId);

            if (document == null)
            {
                throw new DocumentApiEntityNotFoundException($"Document not found id = {documentId}  clientId={clientId}");
            }

            Validate(documentMetadataDto);
            documentMetadataDto.Id = documentId;
            documentMetadataDto.ClientId = clientId;

            var documentNew = _mapper.Map<DocumentMetadata>(documentMetadataDto);

            _documentsMetadataRepository.Update(documentNew);
            _documentsMetadataRepository.SaveChanges();

            return documentNew;
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
                throw new DocumentApiValidationException(exceptionMessage);
            }
        }
    }
}
