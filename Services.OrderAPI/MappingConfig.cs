using AutoMapper;
using Services.OrderAPI.Models;
using Services.OrderAPI.Models.DTO;

namespace Services.OrderAPI
{
    public class MappingConfig
    {
        public static MapperConfiguration RegisterMaps()
        {
            var mappingConfig = new MapperConfiguration(config =>
            {
                config.CreateMap<OrderMasterDto,CartHeaderDto>().
                ForMember(dest=>dest.CartTotal,u=>u.MapFrom(src=>src.OrderTotal)).ReverseMap();

                config.CreateMap<CartDetailsDto, OrderDetailsDto>().
                ForMember(dest => dest.ProductName, u => u.MapFrom(src => src.Product.Name)).
                ForMember(dest => dest.Price, u => u.MapFrom(src => src.Product.Price));

                config.CreateMap<OrderDetailsDto, CartDetailsDto>();
               
                config.CreateMap<OrderMaster, OrderMasterDto>().ReverseMap();
                config.CreateMap<OrderDetails, OrderDetailsDto>().ReverseMap();
            });
            return mappingConfig;
        }
    }
}
