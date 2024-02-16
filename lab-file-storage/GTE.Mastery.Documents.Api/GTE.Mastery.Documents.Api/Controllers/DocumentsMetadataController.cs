using GTE.Mastery.Documents.Api.Attributes;
using GTE.Mastery.Documents.Api.Configurations;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace GTE.Mastery.Documents.Api.Controllers
{
    [ApiController]
    [Route("/clients/{clientId}/documents")]
    [Tags("Document")]
    [DoNotModify]
    public sealed class DocumentsMetadataController : ControllerBase
    {
        private readonly IDocumentsMetadataService _documentsMetadataService;

        public DocumentsMetadataController(IOptions<DocumentStorageOptions> documentStorageConfig)
        {
            if (documentStorageConfig == null)
            {
                throw new ArgumentNullException(nameof(documentStorageConfig));
            }

            _documentsMetadataService = new DocumentsMetadataService(documentStorageConfig.Value.DocumentPath);
        }

        /// <summary>
        /// Retrieves documents for a given client.
        /// </summary>
        /// <param name="clientId">A client id.</param>
        /// <param name="skip">Pagination. The number of records to skip.</param>
        /// <param name="take">Pagination. The number of records to extract from the API.</param>
        /// <returns>Documents.</returns>
        [HttpGet("", Name = "ListDocuments")]
        [ProducesResponseType(typeof(DocumentMetadata), 200)]
        [ProducesResponseType(typeof(IDictionary<string, string>), 400)]
        [ProducesResponseType(500)]
        public async Task<ActionResult<IEnumerable<DocumentMetadata>>> List(
            [FromRoute] int clientId,
            [FromQuery] int? skip,
            [FromQuery] int? take)
        {
            IEnumerable<DocumentMetadata> result = await _documentsMetadataService.ListDocumentsAsync(clientId, skip, take);
            return Ok(result);
        }

        /// <summary>
        /// Creates a document for a given client.
        /// </summary>
        /// <param name="clientId">A client id.</param>
        /// <param name="document">A document.</param>
        /// <returns>Document.</returns>
        [HttpPost("", Name = "CreateDocument")]
        [ProducesResponseType(typeof(DocumentMetadata), 201)]
        [ProducesResponseType(typeof(IDictionary<string, string>), 400)]
        [ProducesResponseType(typeof(IDictionary<string, string>), 404)]
        [ProducesResponseType(500)]
        public async Task<ActionResult<DocumentMetadata>> Post(
            [FromRoute] int clientId,
            [FromBody] DocumentMetadata document)
        {
            DocumentMetadata result = await _documentsMetadataService.CreateDocumentAsync(clientId, document);
            return Created($"/clients/{clientId}/documents/{result.Id}", result);
        }

        /// <summary>
        /// Retrieves a document metadata for a given client.
        /// </summary>
        /// <param name="clientId">A client id.</param>
        /// <param name="documentId">A document id.</param>
        /// <returns>Document.</returns>
        [HttpGet("{documentId}", Name = "GetDocument")]
        [ProducesResponseType(typeof(DocumentMetadata), 200)]
        [ProducesResponseType(typeof(IDictionary<string, string>), 400)]
        [ProducesResponseType(typeof(IDictionary<string, string>), 404)]
        [ProducesResponseType(500)]
        public async Task<ActionResult<DocumentMetadata>> Get([FromRoute] int clientId, [FromRoute] int documentId)
        {
            DocumentMetadata result = await _documentsMetadataService.GetDocumentAsync(clientId, documentId);
            return Ok(result);
        }

        /// <summary>
        /// Updates a document for a given client.
        /// </summary>
        /// <param name="clientId">A client id.</param>
        /// <param name="documentId">A document id.</param>
        /// <param name="document">A document.</param>
        /// <returns>Document.</returns>
        [HttpPut("{documentId}", Name = "UpdateDocument")]
        [ProducesResponseType(typeof(DocumentMetadata), 200)]
        [ProducesResponseType(typeof(IDictionary<string, string>), 400)]
        [ProducesResponseType(typeof(IDictionary<string, string>), 404)]
        [ProducesResponseType(500)]
        public async Task<ActionResult<DocumentMetadata>> Put(
            [FromRoute] int clientId,
            [FromRoute] int documentId,
            [FromBody] DocumentMetadata document)
        {
            DocumentMetadata result = await _documentsMetadataService.UpdateDocumentAsync(clientId, documentId, document);
            return Ok(result);
        }

        /// <summary>
        /// Deletes a document for a given client.
        /// </summary>
        /// <param name="clientId">A client id.</param>
        /// <param name="documentId">A document id.</param>
        /// <returns></returns>
        [HttpDelete("{documentId}", Name = "DeleteDocument")]
        [ProducesResponseType(204)]
        [ProducesResponseType(typeof(IDictionary<string, string>), 400)]
        [ProducesResponseType(typeof(IDictionary<string, string>), 404)]
        [ProducesResponseType(500)]
        public async Task<ActionResult> Delete([FromRoute] int clientId, [FromRoute] int documentId)
        {
            await _documentsMetadataService.DeleteDocumentAsync(clientId, documentId);
            return NoContent();
        }
    }
}
