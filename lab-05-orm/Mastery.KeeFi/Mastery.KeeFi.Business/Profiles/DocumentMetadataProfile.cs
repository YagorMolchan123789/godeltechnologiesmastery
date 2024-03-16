using AutoMapper;
using Mastery.KeeFi.Business.Dto;
using Mastery.KeeFi.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mastery.KeeFi.Business.Profiles
{
    public class DocumentMetadataProfile : Profile
    {
        public DocumentMetadataProfile() 
        {
            CreateMap<DocumentMetadata, DocumentMetadataDto>();
            CreateMap<DocumentMetadataDto, DocumentMetadata>();
        }
    }
}
