using AutoMapper;
using EmployeePortal.API.Dtos;
using EmployeePortal.API.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using EmployeePortal.API.Helper;

namespace EmployeePortal.API.App_Start
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            Mapper.CreateMap<User, UserForListDto>()
                .ForMember(dest => dest.PhotoUrl, opt => 
                opt.MapFrom(src =>
                src.Photos.FirstOrDefault(p => (bool)p.IsMain).Url))
                .ForMember(dest => dest.Age, opt =>
              opt.MapFrom(src => src.DateOfBirth.CalculateAge()));
            Mapper.CreateMap<User, UserForDetailedDto>()
                .ForMember(dest => dest.PhotoUrl, opt =>
                opt.MapFrom(src =>
                src.Photos.FirstOrDefault(p => (bool)p.IsMain).Url))
                .ForMember(dest => dest.Age, opt =>
              opt.MapFrom(src => src.DateOfBirth.CalculateAge()));
            Mapper.CreateMap<Photo, PhotosForDetailedDto>();
        }
    }
}