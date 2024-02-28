using Mastery.KeeFi.BusinessLogic.Interfaces;
using Mastery.KeeFi.BusinessLogic;
using Mastery.KeeFi.Common.Configurations;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace Mastery.KeeFi.Api.Controllers
{
    [ApiController]
    [Route("/clients/{clientId}/documents/{documentId}/content")]
    [Tags("Document")]
    public class DocumentsContentController : ControllerBase
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
