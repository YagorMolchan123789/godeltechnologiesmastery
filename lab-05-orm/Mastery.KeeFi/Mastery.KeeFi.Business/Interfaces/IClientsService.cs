using Mastery.KeeFi.Business.Dto;
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
        Task<IEnumerable<ClientDto>> ListClientsAsync(int? skip, int? take, string[]? tags);

        Task<ClientDto> GetClientAsync(int clientId);

        Task<Client> CreateClientAsync(ClientDto clientDto);

        Task<Client> UpdateClientAsync(int clientId, ClientDto clientDto);

        Task DeleteClientAsync(int clientId);
    }
}
