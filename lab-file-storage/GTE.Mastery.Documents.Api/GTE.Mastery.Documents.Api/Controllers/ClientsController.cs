using GTE.Mastery.Documents.Api.Attributes;
using GTE.Mastery.Documents.Api.Configurations;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace GTE.Mastery.Documents.Api.Controllers
{
    [ApiController]
    [Route("/clients")]
    [Tags("Client")]
    [DoNotModify]
    public sealed class ClientsController : ControllerBase
    {
        private readonly IClientsService _clientsService;
        private readonly IDocumentsMetadataService _documentsMetadataService;
        private readonly IFileService _fileService;

        public ClientsController(IOptions<DocumentStorageOptions> documentStorageConfig, IFileService fileService,
            IDocumentsMetadataService documentsMetadataService)
        {
            if (documentStorageConfig == null)
            {
                throw new ArgumentNullException(nameof(documentStorageConfig));
            }

            _fileService = fileService;
            _documentsMetadataService = documentsMetadataService;
            _clientsService = new ClientsService(documentStorageConfig.Value.ClientPath, documentStorageConfig.Value.DocumentBlobPath,
                _documentsMetadataService, _fileService);
        }

        /// <summary>
        /// Retrieves clients.
        /// </summary>
        /// <param name="skip">Pagination. The number of records to skip.</param>
        /// <param name="take">Pagination. The number of records to extract from the API.</param>
        /// <param name="tags">A list of tags to filter clients.</param>
        /// <returns>Clients.</returns>
        [HttpGet("", Name = "ListClients")]
        [ProducesResponseType(typeof(IEnumerable<Client>), 200)]
        [ProducesResponseType(typeof(IDictionary<string, string>), 400)]
        [ProducesResponseType(500)]
        public async Task<ActionResult<IEnumerable<Client>>> List(
            [FromQuery] int? skip,
            [FromQuery] int? take,
            [FromQuery] string[]? tags)
        {
            IEnumerable<Client> result = await _clientsService.ListClientsAsync(skip, take, tags);
            return Ok(result);
        }

        /// <summary>
        /// Retrieves a client by id.
        /// </summary>
        /// <param name="clientId">A client id.</param>
        /// <returns>Client.</returns>
        [HttpGet("{clientId}", Name = "GetClient")]
        [ProducesResponseType(typeof(Client), 200)]
        [ProducesResponseType(typeof(IDictionary<string, string>), 400)]
        [ProducesResponseType(typeof(IDictionary<string, string>), 404)]
        [ProducesResponseType(500)]
        public async Task<ActionResult<Client>> Get([FromRoute] int clientId)
        {
            Client result = await _clientsService.GetClientAsync(clientId);
            return Ok(result);
        }

        /// <summary>
        /// Creates a client.
        /// </summary>
        /// <param name="client">A client document.</param>
        /// <returns>Client.</returns>
        [HttpPost("", Name = "CreateClient")]
        [ProducesResponseType(typeof(Client), 201)]
        [ProducesResponseType(typeof(IDictionary<string, string>), 400)]
        [ProducesResponseType(500)]
        public async Task<ActionResult<Client>> Post([FromBody] Client client)
        {
            Client result = await _clientsService.CreateClientAsync(client);
            return Created($"/clients/{result.Id}", result);
        }

        /// <summary>
        /// Updates a client.
        /// </summary>
        /// <param name="clientId">A client id.</param>
        /// <param name="client">A client document.</param>
        /// <returns>Client.</returns>
        [HttpPut("{clientId}", Name = "UpdateClient")]
        [ProducesResponseType(typeof(Client), 200)]
        [ProducesResponseType(typeof(IDictionary<string, string>), 400)]
        [ProducesResponseType(typeof(IDictionary<string, string>), 404)]
        [ProducesResponseType(500)]
        public async Task<ActionResult<Client>> Put([FromRoute] int clientId, [FromBody] Client client)
        {
            Client result = await _clientsService.UpdateClientAsync(clientId, client);
            return Ok(result);
        }

        /// <summary>
        /// Removes a client.
        /// </summary>
        /// <param name="clientId">A client id.</param>
        /// <returns></returns>
        [HttpDelete("{clientId}", Name = "DeleteClient")]
        [ProducesResponseType(204)]
        [ProducesResponseType(typeof(IDictionary<string, string>), 400)]
        [ProducesResponseType(typeof(IDictionary<string, string>), 404)]
        [ProducesResponseType(500)]
        public async Task<ActionResult> Delete([FromRoute] int clientId)
        {
            await _clientsService.DeleteClientAsync(clientId);
            return NoContent();
        }
    }
}
