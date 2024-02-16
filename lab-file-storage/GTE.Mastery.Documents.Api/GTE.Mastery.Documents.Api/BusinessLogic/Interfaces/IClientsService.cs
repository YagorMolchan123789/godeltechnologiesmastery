using GTE.Mastery.Documents.Api.Attributes;

namespace GTE.Mastery.Documents.Api.BusinessLogic.Interfaces
{
    [DoNotModify]
    public interface IClientsService
    {
        /// <summary>
        /// Retrieves a list of clients, with optional pagination and filtering by tags.
        /// </summary>
        /// <param name="skip">The number of records to skip for pagination. If no value specified, should rely on 0. Boundaries are [0; int.MaxValue).</param>
        /// <param name="take">The number of records to take for pagination. If no value specified, should rely on 10. Boundaries are (0; 20].</param>
        /// <param name="tags">Tags to filter the clients. Maximum 10 tags could be specified in the filter.</param>
        Task<IEnumerable<Client>> ListClientsAsync(int? skip, int? take, string[]? tags);

        /// <summary>
        /// Fetches a single client by their unique identifier.
        /// </summary>
        /// <param name="clientId">The unique identifier of the client.</param>
        Task<Client> GetClientAsync(int clientId);

        /// <summary>
        /// Creates a new client record.
        /// </summary>
        /// <param name="client">The client information to create.</param>
        Task<Client> CreateClientAsync(Client client);

        /// <summary>
        /// Updates an existing client's information, identified by their unique identifier.
        /// </summary>
        /// <param name="clientId">The unique identifier of the client to update.</param>
        /// <param name="client">The updated client information.</param>
        Task<Client> UpdateClientAsync(int clientId, Client client);

        /// <summary>
        /// Deletes a client record, identified by their unique identifier.
        /// </summary>
        /// <param name="clientId">The unique identifier of the client to delete.</param>
        Task DeleteClientAsync(int clientId);
    }
}
