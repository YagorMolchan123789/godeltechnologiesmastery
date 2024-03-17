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
    public class ClientsRepository :Repository<Client>, IClientsRepository
    {
        public ClientsRepository(MainDbContext context) : base(context)
        {
        }

        public IEnumerable<Client> GetClients(int? skip = null, int? take = null, string[]? tags = null)
        {
            var query = _context.Set<Client>().AsQueryable();

            if (tags!=null && tags.Any())
            {
                var distinctedTags = tags.Distinct();
                query = _context.Set<Client>().AsEnumerable().Where(q => q.Tags.Any(t => distinctedTags.Contains(t)))
                    .AsQueryable();
            }

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
