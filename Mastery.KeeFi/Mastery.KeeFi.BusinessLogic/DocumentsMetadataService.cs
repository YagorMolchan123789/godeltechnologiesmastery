using Mastery.KeeFi.BusinessLogic.Interfaces;
using Mastery.KeeFi.Common.Exceptions;
using Mastery.KeeFi.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Mastery.KeeFi.BusinessLogic
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

        public DocumentsMetadataService(string filePath)
        {
            _filePath = filePath;
        }

        public async Task<DocumentMetadata> CreateDocumentAsync(int clientId, DocumentMetadata documentMetadata)
        {
            Validate(documentMetadata);

            var documentsJson = File.ReadAllText(_filePath);
            var documents = JsonSerializer.Deserialize<List<DocumentMetadata>>(documentsJson);

            documentMetadata.Id = (documents?.Count == 0) ? 1 : documents.Max(d => d.Id) + 1;
            documentMetadata.ClientId = clientId;
            documents.Add(documentMetadata);

            var serializedDocuments = JsonSerializer.Serialize<List<DocumentMetadata>>(documents, new JsonSerializerOptions
            {
                AllowTrailingCommas = true
            });

            File.WriteAllText(_filePath, serializedDocuments);
            return documentMetadata;
        }

        public async Task DeleteDocumentAsync(int clientId, int documentId)
        {
            var documentsJson = File.ReadAllText(_filePath);
            var documents = JsonSerializer.Deserialize<List<DocumentMetadata>>(documentsJson);
            var document = documents?.FirstOrDefault(d => d.ClientId == clientId && d.Id == documentId &&
                !d.Properties.ContainsKey("deleted"));

            if (document == null)
            {
                throw new DocumentApiEntityNotFoundException($"The document with Id={documentId} and ClientId={clientId} is not found");
            }

            document.Properties["deleted"] = "true";
            var serializedDocuments = JsonSerializer.Serialize<List<DocumentMetadata>>(documents);
            File.WriteAllText(_filePath, serializedDocuments);
        }

        public async Task<DocumentMetadata> GetDocumentAsync(int clientId, int documentId)
        {
            var documentsJson = File.ReadAllText(_filePath);
            var documents = JsonSerializer.Deserialize<List<DocumentMetadata>>(documentsJson);
            var document = documents?.FirstOrDefault(d => d.ClientId == clientId && d.Id == documentId &&
                !d.Properties.ContainsKey("deleted"));

            if (document == null)
            {
                throw new DocumentApiEntityNotFoundException($"The document with Id={documentId} and ClientId={clientId} is not found");
            }

            return document;
        }

        public async Task<IEnumerable<DocumentMetadata>> ListDocumentsAsync(int clientId, int? skip, int? take)
        {
            if (skip < 0)
            {
                throw new DocumentApiValidationException("Skip must be more than 0");
            }
            if (take < 0)
            {
                throw new DocumentApiValidationException("Take must be more than 0");
            }

            var documentsJson = File.ReadAllText(_filePath);
            var deserializedDocuments = JsonSerializer.Deserialize<List<DocumentMetadata>>(documentsJson);
            var query = deserializedDocuments?.AsQueryable().Where(d => d.ClientId == clientId &&
                !d.Properties.ContainsKey("deleted"));

            if (query?.Any() == false)
            {
                throw new DocumentApiEntityNotFoundException($"The client with Id={clientId} is not found");
            }

            if (skip != null && skip > 0)
            {
                query = query?.Skip(skip.Value);
            }

            if (take > deserializedDocuments?.Count)
            {
                throw new DocumentApiValidationException("Take is more than count of the documents");
            }

            if (take != null && take > 0)
            {
                query = query?.Take(take.Value);
            }

            var documents = query?.ToList();

            if (documents?.Any() == false)
            {
                throw new DocumentApiValidationException($"There are no documents that blong to the client with Id={clientId}");
            }

            return documents;
        }

        public async Task<DocumentMetadata> UpdateDocumentAsync(int clientId, int documentId, DocumentMetadata documentMetadata)
        {
            var documentsJson = File.ReadAllText(_filePath);
            var documents = JsonSerializer.Deserialize<List<DocumentMetadata>>(documentsJson);
            var document = documents?.FirstOrDefault(d => d.ClientId == clientId && d.Id == documentId &&
                !d.Properties.ContainsKey("deleted"));

            if (document == null)
            {
                throw new DocumentApiEntityNotFoundException($"Document not found id = {documentId}  clientId={clientId}");
            }

            Validate(documentMetadata);

            document.FileName = documentMetadata.FileName;
            document.Title = documentMetadata.Title;
            document.Description = documentMetadata.Description;
            document.Properties = documentMetadata.Properties;
            document.ContentLength = documentMetadata.ContentLength;
            document.ContentType = documentMetadata.ContentType;
            document.ContentMd5 = documentMetadata.ContentMd5;

            var serializedDocuments = JsonSerializer.Serialize(documents);
            File.WriteAllText(_filePath, serializedDocuments);

            return document;
        }

        private void Validate(DocumentMetadata documentMetadata)
        {
            List<string> exceptionMessages = new List<string>();

            if (string.IsNullOrEmpty(documentMetadata.FileName))
            {
                exceptionMessages.Add("Please, fill the FileName out");
            }
            if (documentMetadata.FileName.Length > 255)
            {
                exceptionMessages.Add("The length of the FileName must be not more than 255 symbols");
            }
            if (!_regexFileName.IsMatch(documentMetadata.FileName))
            {
                exceptionMessages.Add("The FileName must consist of English alphanumeric letters and digits, ., -, _");
            }

            if (string.IsNullOrEmpty(documentMetadata.Title))
            {
                exceptionMessages.Add("Plese, fill the Title out");
            }
            if (documentMetadata?.Title?.Length > 150)
            {
                exceptionMessages.Add("The length of the Title must be more than 150 symbols");
            }

            if (documentMetadata?.Description?.Length > 400)
            {
                exceptionMessages.Add("The length of the Description must be mot more than 400 symbols");
            }

            if (documentMetadata?.ContentLength == 0)
            {
                exceptionMessages.Add("The ContentLength must be more than 0");
            }
            if (!_regexContentLength.IsMatch(documentMetadata.ContentLength.ToString()))
            {
                exceptionMessages.Add("The ContentLength must contain only of the numbers");
            }

            if (documentMetadata?.Properties.Keys.Count > 10)
            {
                exceptionMessages.Add("The count of keys must be more than 0");
            }
            if (documentMetadata?.Properties.Keys.Distinct().Count() != documentMetadata?.Properties.Count)
            {
                exceptionMessages.Add("All the keys must be unique");
            }
            if ((bool)(documentMetadata?.Properties.Keys.Any(k => k.Length > 20)))
            {
                exceptionMessages.Add("The length of the key of the Properties must be not more than 20 symbols");
            }
            if (documentMetadata.Properties.Keys.Any(k => !_regexProperties.IsMatch(k)))
            {
                exceptionMessages.Add("All the keys of the Properites must consist of English letters only");
            }

            if (string.IsNullOrEmpty(documentMetadata.ContentType))
            {
                exceptionMessages.Add("Please, fill the ContentType out");
            }
            if (documentMetadata.ContentType.Length > 100)
            {
                exceptionMessages.Add("The length of the ContentType must be not more than 100 symbols");
            }
            if (!_contentTypes.Contains(documentMetadata.ContentType))
            {
                exceptionMessages.Add("The ContentType is unknown");
            }

            if (string.IsNullOrEmpty(documentMetadata.ContentMd5))
            {
                exceptionMessages.Add("Please, fill the ContenMD5 out");
            }
            if (documentMetadata.ContentMd5.Length < 32)
            {
                exceptionMessages.Add("The ContentMD5 is too short");
            }
            if (documentMetadata.ContentMd5.Length > 32)
            {
                exceptionMessages.Add("The ContentMD5 is too long");
            }
            if (!_regexHexademicalNumbers.IsMatch(documentMetadata.ContentMd5))
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
