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
    public class ClientsRepository : IClientsRepository
    {
        private readonly string _filePath;
        private List<Client> _clients;

        public ClientsRepository(string filePath)
        {
            if (string.IsNullOrEmpty(filePath))
            {
                throw new ArgumentNullException(nameof(filePath));
            }

            _filePath = filePath;
        }

        public List<Client> Clients
        {
            get
            {
                if (_clients == null)
                {
                    _clients = JsonSerializer.Deserialize<List<Client>>(File.ReadAllText(_filePath));
                }

                return _clients;
            }

            set
            {
                _clients = value;
            }
        }

        public void Add(Client client)
        {
            Clients.Add(client);
        }

        public IEnumerable<Client> GetAll()
        {
            return Clients;    
        }

        public Client GetClient(int id)
        {
            return Clients.FirstOrDefault(c => c.Id == id && !c.Tags.Contains("deleted"));
        }

        public List<Client> GetClients(int? skip, int? take, string[]? tags)
        {
            var query = Clients.AsQueryable().Where(c => !c.Tags.Contains("deleted"));

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
            client.Tags = client.Tags.Concat(new string[] { "deleted" }).ToArray();
            Update(client);
        }

        public void SaveChanges()
        {
            File.WriteAllText(_filePath, JsonSerializer.Serialize(Clients));
        }

        public void Update(Client client)
        {
            var index = Clients.FindIndex(c => c.Id == client.Id); 

            if (index != -1)
            {
                Clients[index] = client;
            }
        }
    }
}
