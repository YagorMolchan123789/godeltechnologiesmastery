using GTE.Mastery.Documents.Api.Entities;
using GTE.Mastery.Documents.Api.Exceptions;
using System.Text.Json;
using System.Text.RegularExpressions;

namespace GTE.Mastery.Documents.Api.BusinessLogic
{
    public class ClientsService : IClientsService
    {
        private readonly string _filePath;
        private readonly string _blobPath;
        private readonly IDocumentsMetadataService _documentsMetadataService;
        private readonly IFileService _fileService;

        private readonly Regex _regexEnglishTags = new Regex("[a-zA-Z]");
        private readonly Regex _regexNumberTags = new Regex("[0-9]");
        private readonly Regex _regexSpecialCharactersTags = new Regex("[^a-zA-Z0-9]+");

        public ClientsService(string filePath, string blobPath, IDocumentsMetadataService documentsMetadataService,
            IFileService fileService)
        {
            _filePath = filePath;
            _blobPath = blobPath;
            _documentsMetadataService = documentsMetadataService;
            _fileService = fileService;
        }

        public async Task<IEnumerable<Client>> ListClientsAsync(int? skip, int? take, string[]? tags)
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

            return query?.ToList();
        }

        public async Task<Client> GetClientAsync(int clientId)
        {
            var clientsJson = File.ReadAllText(_filePath);
            var clients = JsonSerializer.Deserialize<List<Client>>(clientsJson);
            var client = clients?.FirstOrDefault(c => c.Id == clientId && !c.Tags.Contains("deleted"));

            if(client == null)
            {
                throw new DocumentApiEntityNotFoundException($"The client with Id={clientId} is not found");
            }

            return client;
        }

        public async Task<Client> CreateClientAsync(Client client)
        {
            Validate(client);

            var clientsJson = File.ReadAllText(_filePath);
            var clients = JsonSerializer.Deserialize<List<Client>>(clientsJson);

            client.Id = (clients?.Count == 0) ? 1 : clients.Max(c => c.Id) + 1;
            clients?.Add(client);

            var serializedClients = JsonSerializer.Serialize<List<Client>>(clients);
            File.WriteAllText(_filePath, serializedClients);

            return client;
        }

        public async Task<Client> UpdateClientAsync(int clientId, Client client)
        {
            var clientsJson = File.ReadAllText(_filePath);
            var clients = JsonSerializer.Deserialize<List<Client>>(clientsJson);
            var clientNew = clients?.FirstOrDefault(c => c.Id == clientId && !c.Tags.Contains("deleted"));

            if (clientNew == null)
            {
                throw new DocumentApiEntityNotFoundException($"The client with Id={clientId} is not found");
            }

            Validate(client);

            clientNew.FirstName = client.FirstName;
            clientNew.LastName = client.LastName;
            clientNew.DateOfBirth = client.DateOfBirth;
            clientNew.Tags = client.Tags;

            var serializedClients = JsonSerializer.Serialize(clients);
            File.WriteAllText(_filePath, serializedClients);

            return clientNew;
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

            if(documents.Any())
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

        private void Validate(Client? client)
        {
            List<string> exceptionMessages = new List<string>();                      

            if (String.IsNullOrEmpty(client?.FirstName))
            {
                exceptionMessages.Add("Please, fill the FirstName out");
            }
            if (client?.FirstName?.Length > 50)
            {
                exceptionMessages.Add("The length of FirstName must be not more than 50 symbols");
            }

            if (String.IsNullOrEmpty(client?.LastName))
            {
                exceptionMessages.Add("Please, fill the LastName out");
            }
            if (client?.LastName?.Length > 50)
            {
                exceptionMessages.Add("The length of LastName must be not more than 50 symbols");
            }

            if (!client.DateOfBirth.HasValue)
            {
                throw new DocumentApiValidationException("Please, fill the DateOfBirth out");
            }
            if (GetAge(client) > 130)
            {
                exceptionMessages.Add("The client should not be older than 130 years");
            }
            if ((client.DateOfBirth?.DayNumber - DateOnly.FromDateTime(DateTime.Now).DayNumber) > 0)
            {
                exceptionMessages.Add("The client cannot have the Birth Date from the future");
            }

            if (client.Tags.Any())
            {
                Validate(client.Tags, exceptionMessages);
            }

            if (exceptionMessages.Any())
            {
                string exceptionMessage = string.Join(". ", exceptionMessages);
                throw new DocumentApiValidationException(exceptionMessage);
            }
        }

        private void Validate(string[]? tags, List<string>? exceptionMessages=null)
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

        private int GetAge(Client client)
        {
            var today = DateOnly.FromDateTime(DateTime.Now);
            var age = today.Year - client.DateOfBirth?.Year;

            if (client.DateOfBirth > today.AddYears(-age.Value))
            {
                age--;
            }

            return age.Value;
        }
    }
}
