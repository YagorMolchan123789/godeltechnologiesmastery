using AutoMapper;
using Mastery.KeeFi.Business.Dto;
using Mastery.KeeFi.Business.Interfaces;
using Mastery.KeeFi.Business.Exceptions;
using Mastery.KeeFi.Data.Interfaces;
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
        private readonly string _blobPath;

        private readonly IClientsRepository _clientRepository;
        private readonly IDocumentsMetadataRepository _documentMetadataRepository;

        private readonly IFileService _fileService;
        private readonly IMapper _mapper;

        private readonly Regex _regexEnglishTags = new Regex("[a-zA-Z]");
        private readonly Regex _regexNumberTags = new Regex("[0-9]");
        private readonly Regex _regexSpecialCharactersTags = new Regex("[^a-zA-Z0-9]+");

        public ClientsService(string blobPath, IClientsRepository clientsRepository,
            IDocumentsMetadataRepository documentsMetadataRepository, IFileService fileService, IMapper mapper)
        {
            if (string.IsNullOrEmpty(blobPath))
            {
                throw new ArgumentNullException(nameof(blobPath));  
            }

            if (clientsRepository == null)
            {
                throw new ArgumentNullException(nameof(clientsRepository));
            }

            if (documentsMetadataRepository == null)
            {
                throw new ArgumentNullException(nameof(documentsMetadataRepository));
            }

            if (fileService == null)
            {
                throw new ArgumentNullException(nameof(fileService));
            }

            if (mapper == null)
            {
                throw new ArgumentNullException(nameof(mapper));
            }

            _blobPath = blobPath;
            _clientRepository = clientsRepository;
            _documentMetadataRepository = documentsMetadataRepository;
            _fileService = fileService;
            _mapper = mapper;
        }

        public async Task<Client> CreateClientAsync(ClientDto clientDto)
        {
            Validate(clientDto);

            var clients = _clientRepository.GetAll();

            clientDto.Id = (clients?.Count() == 0) ? 1 : clients.Max(c => c.Id) + 1;

            var client = _mapper.Map<Client>(clientDto);
            
            _clientRepository.Add(client);
            _clientRepository.SaveChanges();

            return client;
        }

        public async Task DeleteClientAsync(int clientId)
        {
            var client = _clientRepository.GetClient(clientId);

            if (client == null)
            {
                throw new DocumentApiEntityNotFoundException($"The client with Id={clientId} is not found");
            }

            var documents =  _documentMetadataRepository.GetDocuments(clientId, null, null);

            if (documents.Any())
            {
                foreach (var document in documents)
                {
                    _documentMetadataRepository.Remove(document);
                }

                _documentMetadataRepository.SaveChanges();
            }

            string targetPath = Path.Combine(_blobPath, client.Id.ToString());
            _fileService.DeleteDirectory(targetPath);

            _clientRepository.Remove(client);
            _clientRepository.SaveChanges();
        }

        public async Task<ClientDto> GetClientAsync(int clientId)
        {
            var client = _clientRepository.GetClient(clientId);

            if (client == null)
            {
                throw new DocumentApiEntityNotFoundException($"The client with Id={clientId} is not found");
            }

            var clientDTO = _mapper.Map<ClientDto>(client);

            return clientDTO;
        }

        public async Task<IEnumerable<ClientDto>> ListClientsAsync(int? skip, int? take, string[]? tags)
        {
            if (skip < 0)
            {
                throw new DocumentApiValidationException("Skip must be more than 0");
            }
            if (take < 0)
            {
                throw new DocumentApiValidationException("Take must be more than 0");
            }

            var allCLients = _clientRepository.GetAll();
            
            if (take > allCLients?.Count())
            {
                throw new DocumentApiValidationException("Take is more than count of the clients");
            }

            if (!allCLients.Any())
            {
                throw new DocumentApiEntityNotFoundException("There are no clients");
            }

            if (tags.Any())
            {
                Validate(tags);
            }

            var clients = _clientRepository.GetClients(skip, take, tags);
            var clientDTOs = _mapper.Map<List<ClientDto>>(clients);

            return clientDTOs;
        }

        public async Task<Client> UpdateClientAsync(int clientId, ClientDto clientDto)
        {
            var client = _clientRepository.GetClient(clientId);

            if (client == null)
            {
                throw new DocumentApiEntityNotFoundException($"The client with Id={clientId} is not found");
            }

            Validate(clientDto);

            clientDto.Id = clientId;

            client.FirstName = clientDto.FirstName;
            client.LastName = clientDto.LastName;
            client.DateOfBirth = clientDto.DateOfBirth;
            client.Tags = clientDto.Tags;

            _clientRepository.Update(client);
            _clientRepository.SaveChanges();

            return client;
        }

        private void Validate(ClientDto clientDto)
        {
            List<string> exceptionMessages = new List<string>();

            if (String.IsNullOrEmpty(clientDto?.FirstName))
            {
                exceptionMessages.Add("Please, fill the FirstName out");
            }
            if (clientDto?.FirstName?.Length > 50)
            {
                exceptionMessages.Add("The length of FirstName must be not more than 50 symbols");
            }

            if (String.IsNullOrEmpty(clientDto?.LastName))
            {
                exceptionMessages.Add("Please, fill the LastName out");
            }
            if (clientDto?.LastName?.Length > 50)
            {
                exceptionMessages.Add("The length of LastName must be not more than 50 symbols");
            }

            if (!clientDto.DateOfBirth.HasValue)
            {
                throw new DocumentApiValidationException("Please, fill the DateOfBirth out");
            }
            if (GetAge(clientDto) > 130)
            {
                exceptionMessages.Add("The client should not be older than 130 years");
            }
            if ((clientDto.DateOfBirth?.DayNumber - DateOnly.FromDateTime(DateTime.Now).DayNumber) > 0)
            {
                exceptionMessages.Add("The client cannot have the Birth Date from the future");
            }

            if (clientDto.Tags.Any())
            {
                Validate(clientDto.Tags, exceptionMessages);
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

        private int GetAge(ClientDto clientDto)
        {
            var today = DateOnly.FromDateTime(DateTime.Now);
            var age = today.Year - clientDto.DateOfBirth?.Year;

            if (clientDto.DateOfBirth > today.AddYears(-age.Value))
            {
                age--;
            }

            return age.Value;
        }
    }
}
