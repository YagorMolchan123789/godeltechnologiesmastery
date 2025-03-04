﻿using Mastery.KeeFi.Business.Dto;
using Mastery.KeeFi.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mastery.KeeFi.Business.Interfaces
{
    public interface IDocumentsMetadataService
    {
        Task<IEnumerable<DocumentMetadataDto>> ListDocumentsAsync(int clientId, int? skip, int? take);

        Task<DocumentMetadataDto> GetDocumentAsync(int clientId, int documentId);

        Task<DocumentMetadataDto> CreateDocumentAsync(int clientId, DocumentMetadataDto documentMetadataDto);

        Task<DocumentMetadataDto> UpdateDocumentAsync(int clientId, int documentId, DocumentMetadataDto documentMetadataDto);

        Task DeleteDocumentAsync(int clientId, int documentId);
    }
}
