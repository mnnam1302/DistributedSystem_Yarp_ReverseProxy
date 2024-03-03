using AutoMapper;
using DistributedSystem.Contract.Services.V1.Product;
using Query.Domain.Entities;

namespace Query.Application.Mapper
{
    public class ServiceProfile : Profile
    {
        public ServiceProfile()
        {
            //CreateMap<ProductProjection, Response.ProductResponse>().ReverseMap();
        }
    }
}