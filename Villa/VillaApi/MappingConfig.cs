using AutoMapper;
using VillaApi.Dtos;
using VillaApi.Models;

namespace VillaApi
{
    public class MappingConfig : Profile
    {
        public MappingConfig()
        {
            CreateMap<Villa, VillaDto>();
            CreateMap<VillaDto, Villa>();

            CreateMap<VillaDto, VillaCreateDto>();
            CreateMap<VillaDto,VillaUpdateDto>();
            CreateMap<VillaNumber, VillaNumberDto>().ReverseMap();
            CreateMap<VillaNumberDto, VillaNumberCreateDto>().ReverseMap();
            CreateMap<VillaNumberDto, VillaNumberUpdateDto>().ReverseMap();


        }
    }
}
