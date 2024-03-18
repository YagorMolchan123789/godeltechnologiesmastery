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
            CreateMap<DocumentMetadata, DocumentMetadataDto>()
                .ForMember(d => d.Id, opt => opt.MapFrom(src => src.Id))
                .ForMember(d => d.ClientId, opt => opt.MapFrom(src => src.ClientId))
                .ForMember(d => d.FileName, opt => opt.MapFrom(src => src.FileName))
                .ForMember(d => d.Title, opt => opt.MapFrom(src => src.Title))
                .ForMember(d => d.Description, opt => opt.MapFrom(src => src.Description))
                .ForMember(d => d.ContentLength, opt => opt.MapFrom(src => src.ContentLength))
                .ForMember(d => d.ContentMd5, opt => opt.MapFrom(src => src.ContentMd5))
                .ForMember(d => d.ContentType, opt => opt.MapFrom(src => src.ContentType))
                .ForMember(d => d.Properties, opt => opt.MapFrom(src => src.Properties.ToDictionary(p => p.Key, p => p.Value)));

            CreateMap<DocumentMetadataDto, DocumentMetadata>()
                .ForMember(d => d.FileName, opt => opt.MapFrom(src => src.FileName))
                .ForMember(d => d.Title, opt => opt.MapFrom(src => src.Title))
                .ForMember(d => d.Description, opt => opt.MapFrom(src => src.Description))
                .ForMember(d => d.ContentLength, opt => opt.MapFrom(src => src.ContentLength))
                .ForMember(d => d.ContentMd5, opt => opt.MapFrom(src => src.ContentMd5))
                .ForMember(d => d.ContentType, opt => opt.MapFrom(src => src.ContentType))
                .ForMember(d => d.Properties, opt => opt.MapFrom(src => src.Properties.Select(p => new DocumentMetadataProperty { Key = p.Key,
                    Value = p.Value, DocumentId = src.Id})));
        }
    }
}
