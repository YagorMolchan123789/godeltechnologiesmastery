using Mastery.KeeFi.Data.Interfaces;
using Mastery.KeeFi.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Mastery.KeeFi.Data.Repositories
{
    public class DocumentsMetadataRepository : IDocumentsMetadataRepository
    {
        private readonly string _filePath;
        private List<DocumentMetadata> _documents;

        public DocumentsMetadataRepository(string filePath)
        {
            if (filePath == null)
            {
                throw new ArgumentNullException(nameof(filePath));
            }

            _filePath = filePath;
        }

        public List<DocumentMetadata> Documents
        {
            get
            {
                if (_documents == null)
                {
                    _documents = JsonSerializer.Deserialize<List<DocumentMetadata>>(File.ReadAllText(_filePath));
                }

                return _documents;
            }
            set
            {
                _documents = value;
            }
        }

        public void Add(DocumentMetadata document)
        {
            Documents.Add(document);
        }

        public IEnumerable<DocumentMetadata> GetAll()
        {
            return Documents;
        }

        public DocumentMetadata GetDocument(int clientId, int documentId)
        {
            return Documents.FirstOrDefault(d => d.Id == documentId && d.ClientId == clientId && !d.Properties.ContainsKey("deleted"));
        }

        public List<DocumentMetadata> GetDocuments(int clientId, int? skip, int? take)
        {
            var query = Documents.AsQueryable().Where(d => d.ClientId == clientId && !d.Properties.ContainsKey("deleted"));

            if (skip != null && skip > 0)
            {
                query = query?.Skip(skip.Value);
            }

            if (take != null && take > 0)
            {
                query = query?.Take(take.Value);
            }

            return query?.ToList();
        }

        public void Remove(DocumentMetadata document)
        {
            document.Properties["deleted"] = "true";
            Update(document);
        }

        public void SaveChanges()
        {
            File.WriteAllText(_filePath, JsonSerializer.Serialize(_documents, new JsonSerializerOptions
            {
                AllowTrailingCommas = true
            }));
        }

        public void Update(DocumentMetadata document)
        {
            var index = Documents.FindIndex(c => c.Id == document.Id);

            if (index != -1)
            {
                Documents[index] = document;
            }
        }
    }
}
