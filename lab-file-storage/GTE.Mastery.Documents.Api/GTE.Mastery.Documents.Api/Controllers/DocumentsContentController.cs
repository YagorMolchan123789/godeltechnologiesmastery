using GTE.Mastery.Documents.Api.Attributes;
using GTE.Mastery.Documents.Api.Configurations;
using GTE.Mastery.Documents.Api.Exceptions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace GTE.Mastery.Documents.Api.Controllers
{
    [ApiController]
    [Route("/clients/{clientId}/documents/{documentId}/content")]
    [Tags("Document")]
    [DoNotModify]
    public sealed class DocumentsContentController : ControllerBase
    {
        private readonly IClientsService _clientsService;
        private readonly IDocumentsContentService _documentsContentService;
        private readonly IDocumentsMetadataService _documentsMetadataService;
        private readonly IFileService _fileService;

        public DocumentsContentController(IOptions<DocumentStorageOptions> documentStorageConfig, IFileService fileService,
            IDocumentsMetadataService documentsMetadataService, IClientsService clientsService)
        {
            if (documentStorageConfig == null)
            {
                throw new ArgumentNullException(nameof(documentStorageConfig));
            }

            _fileService = fileService;
            _documentsMetadataService = documentsMetadataService;
            _clientsService = clientsService;
            _documentsContentService = new DocumentsContentService(documentStorageConfig.Value.DocumentBlobPath,
                _documentsMetadataService, _clientsService, _fileService);
        }

        /// <summary>
        /// Uploads document content to the storage.
        /// </summary>
        /// <param name="clientId">A client id.</param>
        /// <param name="documentId">A document id.</param>
        /// <param name="file">File content.</param>
        /// <returns></returns>
        [HttpPost("")]
        [ProducesResponseType(200)]
        [ProducesResponseType(typeof(IDictionary<string, string>), 400)]
        [ProducesResponseType(typeof(IDictionary<string, string>), 404)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> Upload(
            [FromRoute] int clientId,
            [FromRoute] int documentId,
            IFormFile file)
        {
            var buffer = new byte[file.Length];
            using (var content = new MemoryStream(buffer))
            {
                await file.CopyToAsync(content);
                content.Position = 0;

                await _documentsContentService.UploadDocumentAsync(clientId, documentId, content);
            }

            return Ok();
        }

        /// <summary>
        /// Downloads document content from the storage.
        /// </summary>
        /// <param name="clientId">A client id.</param>
        /// <param name="documentId">A document id.</param>
        /// <returns>Document content.</returns>
        [HttpGet("")]
        [ProducesResponseType(200)]
        [ProducesResponseType(typeof(IDictionary<string, string>), 400)]
        [ProducesResponseType(typeof(IDictionary<string, string>), 404)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> Download([FromRoute] int clientId, [FromRoute] int documentId)
        {
            ReceiveDocumentResponse documentContent = await _documentsContentService.ReceiveDocumentAsync(clientId, documentId);
            return File(documentContent.Content, documentContent.Metadata.ContentType, documentContent.Metadata.FileName);
        }
    }
}
