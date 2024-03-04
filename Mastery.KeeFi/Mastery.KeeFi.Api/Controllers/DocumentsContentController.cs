using Mastery.KeeFi.Api.Configurations;
using Mastery.KeeFi.Business.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Mastery.KeeFi.Business.Services;

namespace Mastery.KeeFi.Api.Controllers
{
    [ApiController]
    [Route("/clients/{clientId}/documents/{documentId}/content")]
    [Tags("Document")]
    public class DocumentsContentController : ControllerBase
    {
        private readonly IDocumentsContentService _documentsContentService;

        public DocumentsContentController(IDocumentsContentService documentsContentService)
        {
            if (documentsContentService == null)
            {
                throw new ArgumentNullException(nameof(documentsContentService));
            }

            _documentsContentService = documentsContentService;
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
