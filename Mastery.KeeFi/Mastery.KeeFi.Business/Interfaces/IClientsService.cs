using Mastery.KeeFi.Business.DTO;
using Mastery.KeeFi.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mastery.KeeFi.Business.Interfaces
{
    public interface IClientsService
    {
        Task<IEnumerable<ClientDTO>> ListClientsAsync(int? skip, int? take, string[]? tags);

        Task<ClientDTO> GetClientAsync(int clientId);

        Task<Client> CreateClientAsync(ClientDTO clientDTO);

        Task<Client> UpdateClientAsync(int clientId, ClientDTO clientDTO);

        Task DeleteClientAsync(int clientId);
    }
}
