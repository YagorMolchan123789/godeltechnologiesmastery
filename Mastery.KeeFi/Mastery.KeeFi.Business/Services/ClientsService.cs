﻿using AutoMapper;
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
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Validations;
using Mastery.KeeFi.Business.Extensions;
using NLog;

namespace Mastery.KeeFi.Business.Services
{
    public class ClientsService : IClientsService
    {
        private readonly string _blobPath;

        private readonly IClientsRepository _clientRepository;
        private readonly IDocumentsMetadataRepository _documentMetadataRepository;

        private readonly IFileService _fileService;
        private readonly IMapper _mapper;
        private readonly ILogger<ClientsService> _logger;

        private readonly Regex _regexEnglishTags = new Regex("[a-zA-Z]");
        private readonly Regex _regexNumberTags = new Regex("[0-9]");
        private readonly Regex _regexSpecialCharactersTags = new Regex("[^a-zA-Z0-9]+");

        public ClientsService(string blobPath, IClientsRepository clientsRepository,
            IDocumentsMetadataRepository documentsMetadataRepository, IFileService fileService, 
            ILogger<ClientsService> logger, IMapper mapper)
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

            if (logger == null)
            {
                throw new ArgumentNullException(nameof(logger));
            }

            if (mapper == null)
            {
                throw new ArgumentNullException(nameof(mapper));
            }

            _blobPath = blobPath;
            _clientRepository = clientsRepository;
            _documentMetadataRepository = documentsMetadataRepository;
            _fileService = fileService;
            _logger = logger;
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

            _logger.LogInformation($"The client with Id = {client.Id}  has been successfully created");

            return client;
        }

        public async Task DeleteClientAsync(int clientId)
        {
            var client = _clientRepository.GetClient(clientId);

            if (client == null)
            {
                var exception = new DocumentApiEntityNotFoundException($"The client with Id={clientId} is not found");
                var stackTrace = Environment.StackTrace.GetString();
                var logMessage = string.Concat("  StackTrace: ", stackTrace);
                _logger.LogError(exception, logMessage);
                throw exception;
            }

            var documents =  _documentMetadataRepository.GetDocuments(clientId, null, null);

            if (documents.Any())
            {
                foreach (var document in documents)
                {
                    _documentMetadataRepository.Remove(document);
                }

                _documentMetadataRepository.SaveChanges();
                _logger.LogInformation($"All the documents of the client with Id={clientId} have been removed");
            }

            string targetPath = Path.Combine(_blobPath, client.Id.ToString());
            _fileService.DeleteDirectory(targetPath);

            _clientRepository.Remove(client);
            _clientRepository.SaveChanges();

            _logger.LogInformation($"The client with Id={clientId} has been successfully removed");
        }

        public async Task<ClientDto> GetClientAsync(int clientId)
        {
            var client = _clientRepository.GetClient(clientId);

            if (client == null)
            {
                var exception = new DocumentApiEntityNotFoundException($"The client with Id={clientId} is not found");
                var stackTrace = Environment.StackTrace.GetString();
                var logMessage = string.Concat("  StackTrace: ", stackTrace);
                _logger.LogError(exception, logMessage);
                throw exception;
            }

            var clientDTO = _mapper.Map<ClientDto>(client);

            return clientDTO;
        }

        public async Task<IEnumerable<ClientDto>> ListClientsAsync(int? skip, int? take, string[]? tags)
        {
            if (skip < 0)
            {
                var exception = new DocumentApiValidationException("Skip must be more than 0");
                var stackTrace = Environment.StackTrace.GetString();
                var logMessage = string.Concat("  StackTrace: ", stackTrace);
                _logger.LogError(exception, logMessage);
                throw exception;
            }
            if (take < 0)
            {
                var exception = new DocumentApiValidationException("Take must be more than 0");
                var stackTrace = Environment.StackTrace.GetString();
                var logMessage = string.Concat("  StackTrace: ", stackTrace);
                _logger.LogError(exception, logMessage);
                throw exception;
            }

            var allCLients = _clientRepository.GetAll();
            
            if (take > allCLients?.Count())
            {
                var exception = new DocumentApiValidationException("Take is more than count of the clients");
                var stackTrace = Environment.StackTrace.GetString();
                var logMessage = string.Concat("  StackTrace: ", stackTrace);
                _logger.LogError(exception, logMessage);
                throw exception;
            }

            if (!allCLients.Any())
            {
                var exception = new DocumentApiEntityNotFoundException("There are no clients");
                var stackTrace = Environment.StackTrace.GetString();
                var logMessage = string.Concat("  StackTrace: ", stackTrace);
                _logger.LogError(exception, logMessage);
                throw exception;
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
                var exception = new DocumentApiEntityNotFoundException($"The client with Id={clientId} is not found");
                var stackTrace = Environment.StackTrace.GetString();
                var logMessage = string.Concat("  StackTrace: ", stackTrace);
                _logger.LogError(exception, logMessage);
                throw exception;
            }

            Validate(clientDto);

            clientDto.Id = clientId;

            client.FirstName = clientDto.FirstName;
            client.LastName = clientDto.LastName;
            client.DateOfBirth = clientDto.DateOfBirth;
            client.Tags = clientDto.Tags;

            _clientRepository.Update(client);
            _clientRepository.SaveChanges();

            _logger.LogInformation($"The client with Id = {clientId} has been successfully updated");

            return client;
        }

        private void Validate(ClientDto clientDto)
        {
            List<string> exceptionMessages = new List<string>();

            if (string.IsNullOrEmpty(clientDto?.FirstName))
            {
                exceptionMessages.Add("Please, fill the FirstName out");
            }
            if (clientDto?.FirstName?.Length > 50)
            {
                exceptionMessages.Add("The length of FirstName must be not more than 50 symbols");
            }

            if (string.IsNullOrEmpty(clientDto?.LastName))
            {
                exceptionMessages.Add("Please, fill the LastName out");
            }
            if (clientDto?.LastName?.Length > 50)
            {
                exceptionMessages.Add("The length of LastName must be not more than 50 symbols");
            }

            if (!clientDto.DateOfBirth.HasValue)
            {
                var exception = new DocumentApiValidationException("Please, fill the DateOfBirth out");
                var stackTrace = Environment.StackTrace.GetString();
                var logMessage = string.Concat("  StackTrace: ", stackTrace);
                _logger.LogError(exception, logMessage);
                throw exception;
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
                var exception = new DocumentApiValidationException(exceptionMessage);
                var stackTrace = Environment.StackTrace.GetString();
                var logMessage = string.Concat("  StackTrace: ", stackTrace);
                _logger.LogError(exception, logMessage);
                throw exception;
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
                var exception = new DocumentApiValidationException(exceptionMessage);
                var stackTrace = Environment.StackTrace.GetString();
                var logMessage = string.Concat("  StackTrace: ", stackTrace);
                _logger.LogError(exception, logMessage);
                throw exception;
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
