using Mastery.KeeFi.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mastery.KeeFi.BusinessLogic.Interfaces
{
    public  interface IClientsService
    {
        Task<IEnumerable<Client>> ListClientsAsync(int? skip, int? take, string[]? tags);

        Task<Client> GetClientAsync(int clientId);

        Task<Client> CreateClientAsync(Client client);

        Task<Client> UpdateClientAsync(int clientId, Client client);

        Task DeleteClientAsync(int clientId);

    }
}
