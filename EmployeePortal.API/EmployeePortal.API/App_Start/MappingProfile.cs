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
            Mapper.CreateMap<UserForUpdateDto, User>();
            Mapper.CreateMap<Photo, PhotoForReturnDto>();
            Mapper.CreateMap<PhotoForCreationDto, Photo>();
            Mapper.CreateMap<UserForRegisterDto, User>();
            Mapper.CreateMap<MessageForCreationDto, Message>().ReverseMap();
            Mapper.CreateMap<Message, MessageToReturnDto>();
            //    .ForMember(m => m.SenderPhotoUrl, opt => opt.MapFrom(u => u.User.Photos.FirstOrDefault(p => (bool)p.IsMain).Url))
            //.ForMember(m => m.RecipientPhotoUrl, opt => opt.MapFrom(u => u.User1.Photos.FirstOrDefault(p => (bool)p.IsMain).Url));
            //.ForMember(m => m.SenderKnownAs, opt => opt.MapFrom(u => u.User.KnownAs))
            //.ForMember(m => m.RecipientKnownAs, opt => opt.MapFrom(u => u.User1.KnownAs)); 
        }
    }
}