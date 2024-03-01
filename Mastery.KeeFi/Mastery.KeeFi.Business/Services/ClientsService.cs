using AutoMapper;
using Mastery.KeeFi.Business.DTO;
using Mastery.KeeFi.Business.Interfaces;
using Mastery.KeeFi.Common.Exceptions;
using Mastery.KeeFi.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Mastery.KeeFi.Business.Services
{
    public class ClientsService : IClientsService
    {
        private readonly string _filePath;
        private readonly string _blobPath;
        private readonly IDocumentsMetadataService _documentsMetadataService;
        private readonly IFileService _fileService;
        private readonly IMapper _mapper;

        private readonly Regex _regexEnglishTags = new Regex("[a-zA-Z]");
        private readonly Regex _regexNumberTags = new Regex("[0-9]");
        private readonly Regex _regexSpecialCharactersTags = new Regex("[^a-zA-Z0-9]+");

        public ClientsService(string filePath, string blobPath, IDocumentsMetadataService documentsMetadataService,
            IFileService fileService, IMapper mapper)
        {
            _filePath = filePath;
            _blobPath = blobPath;
            _documentsMetadataService = documentsMetadataService;
            _fileService = fileService;
            _mapper = mapper;
        }

        public async Task<Client> CreateClientAsync(ClientDTO clientDTO)
        {
            Validate(clientDTO);

            var clientsJson = File.ReadAllText(_filePath);
            var clients = JsonSerializer.Deserialize<List<Client>>(clientsJson);

            var client = _mapper.Map<Client>(clientDTO);

            client.Id = (clients?.Count == 0) ? 1 : clients.Max(c => c.Id) + 1;
            clients?.Add(client);

            var serializedClients = JsonSerializer.Serialize<List<Client>>(clients);
            File.WriteAllText(_filePath, serializedClients);

            return client;
        }

        public async Task DeleteClientAsync(int clientId)
        {
            var clientsJson = File.ReadAllText(_filePath);
            var clients = JsonSerializer.Deserialize<List<Client>>(clientsJson);
            var client = clients?.FirstOrDefault(c => c.Id == clientId && !c.Tags.Contains("deleted"));

            if (client == null)
            {
                throw new DocumentApiEntityNotFoundException($"The client with Id={clientId} is not found");
            }

            var documents = await _documentsMetadataService.ListDocumentsAsync(clientId, null, null);

            if (documents.Any())
            {
                foreach (var document in documents)
                {
                    _documentsMetadataService?.DeleteDocumentAsync(clientId, document.Id);
                }
            }

            string targetPath = Path.Combine(_blobPath, client.Id.ToString());
            _fileService.DeleteDirectory(targetPath);

            client.Tags = client.Tags.Concat(new string[] { "deleted" }).ToArray();
            var serializedClients = JsonSerializer.Serialize<List<Client>>(clients);
            File.WriteAllText(_filePath, serializedClients);
        }

        public async Task<ClientDTO> GetClientAsync(int clientId)
        {
            var clientsJson = File.ReadAllText(_filePath);
            var clients = JsonSerializer.Deserialize<List<Client>>(clientsJson);
            var client = clients?.FirstOrDefault(c => c.Id == clientId && !c.Tags.Contains("deleted"));

            if (client == null)
            {
                throw new DocumentApiEntityNotFoundException($"The client with Id={clientId} is not found");
            }

            var clientDTO = _mapper.Map<ClientDTO>(client);

            return clientDTO;
        }

        public async Task<IEnumerable<ClientDTO>> ListClientsAsync(int? skip, int? take, string[]? tags)
        {
            if (skip < 0)
            {
                throw new DocumentApiValidationException("Skip must be more than 0");
            }
            if (take < 0)
            {
                throw new DocumentApiValidationException("Take must be more than 0");
            }

            var clientsJson = File.ReadAllText(_filePath);
            var clients = JsonSerializer.Deserialize<List<Client>>(clientsJson);

            if (!clients.Any())
            {
                throw new DocumentApiEntityNotFoundException("There are no clients");
            }

            var query = clients?.AsQueryable().Where(c => !c.Tags.Contains("deleted"));

            if (tags.Any())
            {
                var distinctedTags = tags.Distinct();
                Validate(tags);
                query = query?.Where(q => q.Tags.Any(t => distinctedTags.Contains(t)));
            }

            if (skip != null && skip > 0)
            {
                query = query?.Skip(skip.Value);
            }

            if (take > clients?.Count)
            {
                throw new DocumentApiValidationException("Take is more than count of the clients");
            }

            if (take != null && take > 0)
            {
                query = query?.Take(take.Value);
            }

            var clientDTOs = _mapper.Map<List<ClientDTO>>(query);

            return clientDTOs;
        }

        public async Task<Client> UpdateClientAsync(int clientId, ClientDTO clientDTO)
        {
            var clientsJson = File.ReadAllText(_filePath);
            var clients = JsonSerializer.Deserialize<List<Client>>(clientsJson);
            var clientNew = clients?.FirstOrDefault(c => c.Id == clientId && !c.Tags.Contains("deleted"));

            if (clientNew == null)
            {
                throw new DocumentApiEntityNotFoundException($"The client with Id={clientId} is not found");
            }

            Validate(clientDTO);

            clientNew.FirstName = clientDTO.FirstName;
            clientNew.LastName = clientDTO.LastName;
            clientNew.DateOfBirth = clientDTO.DateOfBirth;
            clientNew.Tags = clientDTO.Tags;

            var serializedClients = JsonSerializer.Serialize(clients);
            File.WriteAllText(_filePath, serializedClients);

            return clientNew;
        }

        private void Validate(ClientDTO clientDTO)
        {
            List<string> exceptionMessages = new List<string>();

            if (String.IsNullOrEmpty(clientDTO?.FirstName))
            {
                exceptionMessages.Add("Please, fill the FirstName out");
            }
            if (clientDTO?.FirstName?.Length > 50)
            {
                exceptionMessages.Add("The length of FirstName must be not more than 50 symbols");
            }

            if (String.IsNullOrEmpty(clientDTO?.LastName))
            {
                exceptionMessages.Add("Please, fill the LastName out");
            }
            if (clientDTO?.LastName?.Length > 50)
            {
                exceptionMessages.Add("The length of LastName must be not more than 50 symbols");
            }

            if (!clientDTO.DateOfBirth.HasValue)
            {
                throw new DocumentApiValidationException("Please, fill the DateOfBirth out");
            }
            if (GetAge(clientDTO) > 130)
            {
                exceptionMessages.Add("The client should not be older than 130 years");
            }
            if ((clientDTO.DateOfBirth?.DayNumber - DateOnly.FromDateTime(DateTime.Now).DayNumber) > 0)
            {
                exceptionMessages.Add("The client cannot have the Birth Date from the future");
            }

            if (clientDTO.Tags.Any())
            {
                Validate(clientDTO.Tags, exceptionMessages);
            }

            if (exceptionMessages.Any())
            {
                string exceptionMessage = string.Join(". ", exceptionMessages);
                throw new DocumentApiValidationException(exceptionMessage);
            }
        }

        private void Validate(string[]? tags, List<string>? exceptionMessages = null)
        {
            bool isTagsValidation = false;

            if (exceptionMessages is null)
            {
                exceptionMessages = new List<string>();
                isTagsValidation = true;
            }

            if (tags?.Length > 10)
            {
                exceptionMessages?.Add("The count of the tags be not more than 10");
            }
            if (tags?.Distinct().Count() != tags?.Count())
            {
                exceptionMessages?.Add("All the client's tags must be unique");
            }
            if (tags.Any(t => !_regexEnglishTags.IsMatch(t)))
            {
                exceptionMessages?.Add("All the tags must consist of English letters only");
            }
            if (tags.Any(t => _regexNumberTags.IsMatch(t)))
            {
                exceptionMessages?.Add("No tag must contain the digits");
            }
            if (tags.Any(t => _regexSpecialCharactersTags.IsMatch(t)))
            {
                exceptionMessages?.Add("No tag must contain the special numbers");
            }
            if (tags.Any(t => t.Length > 20))
            {
                exceptionMessages?.Add("The length of all the tags must be not more than 20 symbols");
            }

            if (exceptionMessages.Any() && isTagsValidation)
            {
                string exceptionMessage = string.Join(". ", exceptionMessages);
                throw new DocumentApiValidationException(exceptionMessage);
            }
        }

        private int GetAge(ClientDTO clientDTO)
        {
            var today = DateOnly.FromDateTime(DateTime.Now);
            var age = today.Year - clientDTO.DateOfBirth?.Year;

            if (clientDTO.DateOfBirth > today.AddYears(-age.Value))
            {
                age--;
            }

            return age.Value;
        }
    }
}
