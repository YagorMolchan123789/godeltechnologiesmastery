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
        private KeeFiDbContext _context;

        public DocumentsMetadataRepository(KeeFiDbContext context)
        {
            if (context is null)
            {
                throw new ArgumentNullException(nameof(context));
            }

            _context = context;
        }

        public List<DocumentMetadata> Documents
        {
            get
            {
                return _context.Set<DocumentMetadata>().ToList();
            }
        }

        public void Add(DocumentMetadata document)
        {
            _context.Add(document);
        }

        public DocumentMetadata GetDocument(int clientId, int documentId)
        {
            return Documents.FirstOrDefault(d => d.Id == documentId && d.ClientId == clientId);
        }

        public List<DocumentMetadata> GetDocuments(int clientId, int? skip, int? take)
        {
            var query = Documents.AsQueryable().Where(d => d.ClientId == clientId);

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
            _context.Remove(document);
        }

        public void Update(DocumentMetadata document)
        {
            _context.Update(document);
        }
    }
}
