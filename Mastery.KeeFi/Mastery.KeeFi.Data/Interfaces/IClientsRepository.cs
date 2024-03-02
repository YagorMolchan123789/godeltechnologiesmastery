using Mastery.KeeFi.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mastery.KeeFi.Data.Interfaces
{
    public interface IClientsRepository
    {
        IEnumerable<Client> GetAll();

        List<Client> GetClients(int? skip, int? take, string[]? tags);

        Client GetClient(int id);

        void Add(Client client);

        void Update(Client client);

        void Remove(Client client);

        void SaveChanges();
    }
}
