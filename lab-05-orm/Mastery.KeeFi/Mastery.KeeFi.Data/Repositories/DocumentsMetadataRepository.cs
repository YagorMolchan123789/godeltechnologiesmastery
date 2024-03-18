using Mastery.KeeFi.Data.Interfaces;
using Mastery.KeeFi.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Mastery.KeeFi.Data.Repositories
{
    public class DocumentsMetadataRepository : Repository<DocumentMetadata>, IDocumentsMetadataRepository
    {
        public DocumentsMetadataRepository(MainDbContext context) : base(context)
        {
        }

        public DocumentMetadata GetDocumentByClientId(int clientId, int documentId)
        {
            return _context.Set<DocumentMetadata>().Include(d => d.Properties)
                .FirstOrDefault(d => d.Id == documentId && d.ClientId == clientId);
        }

        public IEnumerable<DocumentMetadata> GetDocuments(int clientId, int? skip = null, int? take = null)
        {
            var query = _context.Set<DocumentMetadata>().Include(d => d.Properties).AsQueryable()
                .Where(d => d.ClientId == clientId);

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
    }
}
