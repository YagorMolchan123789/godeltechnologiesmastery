using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Mastery.KeeFi.Business.Interfaces;
using Mastery.KeeFi.Api.Configurations;
using Mastery.KeeFi.Business.Dto;
using Mastery.KeeFi.Domain.Entities;

namespace Mastery.KeeFi.Api.Controllers
{
    [ApiController]
    [Route("/clients")]
    [Tags("Client")]
    public sealed class ClientsController : ControllerBase
    {
        private readonly IClientsService _clientsService;

        public ClientsController(IClientsService clientsService)
        {
            if (clientsService == null)
            {
                throw new ArgumentNullException(nameof(clientsService));
            }

            _clientsService = clientsService;
        }

        [HttpGet("", Name = "ListClients")]
        [ProducesResponseType(typeof(IEnumerable<ClientDto>), 200)]
        [ProducesResponseType(typeof(IDictionary<string, string>), 400)]
        [ProducesResponseType(500)]
        public async Task<ActionResult<IEnumerable<ClientDto>>> List(
            [FromQuery] int? skip,
            [FromQuery] int? take,
            [FromQuery] string[]? tags)
        {
            IEnumerable<ClientDto> result = await _clientsService.ListClientsAsync(skip, take, tags);
            return Ok(result);
        }

        [HttpGet("{clientId}", Name = "GetClient")]
        [ProducesResponseType(typeof(ClientDto), 200)]
        [ProducesResponseType(typeof(IDictionary<string, string>), 400)]
        [ProducesResponseType(typeof(IDictionary<string, string>), 404)]
        [ProducesResponseType(500)]
        public async Task<ActionResult<ClientDto>> Get([FromRoute] int clientId)
        {
            ClientDto result = await _clientsService.GetClientAsync(clientId);
            return Ok(result);
        }

        [HttpPost("", Name = "CreateClient")]
        [ProducesResponseType(typeof(Client), 201)]
        [ProducesResponseType(typeof(IDictionary<string, string>), 400)]
        [ProducesResponseType(500)]
        public async Task<ActionResult<Client>> Post([FromBody] ClientDto clientDto)
        {
            Client result = await _clientsService.CreateClientAsync(clientDto);
            return Created($"/clients/{result.Id}", result);
        }

        [HttpPut("{clientId}", Name = "UpdateClient")]
        [ProducesResponseType(typeof(Client), 200)]
        [ProducesResponseType(typeof(IDictionary<string, string>), 400)]
        [ProducesResponseType(typeof(IDictionary<string, string>), 404)]
        [ProducesResponseType(500)]
        public async Task<ActionResult<Client>> Put([FromRoute] int clientId, [FromBody] ClientDto clientDto)
        {
            Client result = await _clientsService.UpdateClientAsync(clientId, clientDto);
            return Ok(result);
        }

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
