using AutoMapper;
using WebSite.Identity.Data;
using WebSite.Identity.JsonModels.Account;

namespace WebSite.Identity.Configurations
{
    public class MapperConfig : Profile
    {
        public MapperConfig() 
        {
            CreateMap<RegisterJsonModel, User>()
                .ForMember(to => to.UserName, from => from.MapFrom(src => src.Email))
                .ForMember(to => to.Email, from => from.MapFrom(src => src.Email))
                .ForMember(to => to.FirstName, from => from.MapFrom(src => src.FirstName))
                .ForMember(to => to.LastName, from => from.MapFrom(src => src.LastName));
        }
    }
}
