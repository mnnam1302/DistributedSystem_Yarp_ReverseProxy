using AutoMapper;

namespace Query.Application.Mapper;

public class ServiceProfile : Profile
{
    public ServiceProfile()
    {
        // DocumentId to Id
        //CreateMap<ProductProjection, Response.ProductResponse>()
        //    .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.DocumentId)).ReverseMap();
        //CreateMap<List<ProductProjection>, List<Response.ProductResponse>>().ReverseMap();

        //CreateMap<ProductProjection, ProductResponse>()
        //    .ConstructUsing(src => new ProductResponse(
        //        src.DocumentId,
        //        src.Name,
        //        src.Price,
        //        src.Description));
    }
}
