﻿using AutoMapper;
using Mastery.KeeFi.Api.Configurations;
using Mastery.KeeFi.Business.Dto;
using Mastery.KeeFi.Business.Interfaces;
using Mastery.KeeFi.Domain.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace Mastery.KeeFi.Api.Controllers
{
    [ApiController]
    [Route("/clients/{clientId}/documents")]
    [Tags("Document")]
    public sealed class DocumentsMetadataController : ControllerBase
    {
        private readonly IDocumentsMetadataService _documentsMetadataService;

        public DocumentsMetadataController(IDocumentsMetadataService documentsMetadataService)
        {
            if (documentsMetadataService == null)
            {
                throw new ArgumentNullException(nameof(documentsMetadataService));
            }

            _documentsMetadataService = documentsMetadataService;
        }
        
        [HttpGet("", Name = "ListDocuments")]
        [ProducesResponseType(typeof(DocumentMetadata), 200)]
        [ProducesResponseType(typeof(IDictionary<string, string>), 400)]
        [ProducesResponseType(500)]
        public async Task<ActionResult<IEnumerable<DocumentMetadataDto>>> List(
            [FromRoute] int clientId,
            [FromQuery] int? skip,
            [FromQuery] int? take)
        {
            IEnumerable<DocumentMetadataDto> result = await _documentsMetadataService.ListDocumentsAsync(clientId, skip, take);
            return Ok(result);
        }

        [HttpPost("", Name = "CreateDocument")]
        [ProducesResponseType(typeof(DocumentMetadata), 201)]
        [ProducesResponseType(typeof(IDictionary<string, string>), 400)]
        [ProducesResponseType(typeof(IDictionary<string, string>), 404)]
        [ProducesResponseType(500)]
        public async Task<ActionResult<DocumentMetadataDto>> Post(
            [FromRoute] int clientId,
            [FromBody] DocumentMetadataDto documentDto)
        {
            DocumentMetadataDto result = await _documentsMetadataService.CreateDocumentAsync(clientId, documentDto);
            return Created($"/clients/{clientId}/documents/{result.Id}", result);
        }

        [HttpGet("{documentId}", Name = "GetDocument")]
        [ProducesResponseType(typeof(DocumentMetadata), 200)]
        [ProducesResponseType(typeof(IDictionary<string, string>), 400)]
        [ProducesResponseType(typeof(IDictionary<string, string>), 404)]
        [ProducesResponseType(500)]
        public async Task<ActionResult<DocumentMetadataDto>> Get([FromRoute] int clientId, [FromRoute] int documentId)
        {
            DocumentMetadataDto result = await _documentsMetadataService.GetDocumentAsync(clientId, documentId);
            return Ok(result);
        }

        [HttpPut("{documentId}", Name = "UpdateDocument")]
        [ProducesResponseType(typeof(DocumentMetadata), 200)]
        [ProducesResponseType(typeof(IDictionary<string, string>), 400)]
        [ProducesResponseType(typeof(IDictionary<string, string>), 404)]
        [ProducesResponseType(500)]
        public async Task<ActionResult<DocumentMetadata>> Put(
            [FromRoute] int clientId,
            [FromRoute] int documentId,
            [FromBody] DocumentMetadataDto documentDto)
        {
            DocumentMetadataDto result = await _documentsMetadataService.UpdateDocumentAsync(clientId, documentId, documentDto);
            return Ok(result);
        }

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
