using Mastery.KeeFi.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mastery.KeeFi.Data.Interfaces
{
    public interface IClientsRepository : IRepository<Client>
    {
        IEnumerable<Client> GetClients(int? skip, int? take, string[]? tags);
    }
}
