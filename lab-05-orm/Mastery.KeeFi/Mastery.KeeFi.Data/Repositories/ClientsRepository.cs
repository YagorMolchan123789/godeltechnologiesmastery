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
    public class ClientsRepository : IClientsRepository
    {
        private readonly KeeFiDbContext _context;

        public ClientsRepository(KeeFiDbContext context)
        {
            if (context is null)
            {
                throw new ArgumentNullException(nameof(context));
            }

            _context = context;
        }

        public List<Client> Clients
        {
            get
            {
                return _context.Set<Client>().ToList();
            }
        }

        public void Add(Client client)
        {
            _context.Add(client);
        }

        public Client? GetClient(int id)
        {
            return Clients.FirstOrDefault(c => c.Id == id);
        }

        public List<Client> GetClients(int? skip, int? take, string[]? tags)
        {
            var query = Clients.AsQueryable();

            if (tags.Any())
            {
                var distinctedTags = tags.Distinct();
                query = query?.Where(q => q.Tags.Any(t => distinctedTags.Contains(t)));
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

        public void Remove(Client client)
        {
            _context.Remove(client);
        }

        public void Update(Client client)
        {
            _context.Update(client);
        }
    }
}
