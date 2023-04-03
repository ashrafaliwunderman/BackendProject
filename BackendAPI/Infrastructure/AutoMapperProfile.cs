using AutoMapper;
using BackendAPI.Model;
using System.Data;

namespace BackendAPI.Infrastructure
{
    public class AutoMapperProfile : Profile
    {
        public AutoMapperProfile()
        {
            CreateMap<Core.Model.Product, Product>().ReverseMap();
            CreateMap<Core.Model.Product, ProductEdit>().ReverseMap();

            CreateMap<Core.Model.Order, Model.Order>().ReverseMap();
            CreateMap<Core.Model.OrderProduct, Model.OrderProduct>().ReverseMap();

        }
    }
}
