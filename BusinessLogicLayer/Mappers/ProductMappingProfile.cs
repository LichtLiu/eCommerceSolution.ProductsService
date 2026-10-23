using AutoMapper;
using BusinessLogicLayer.DTO;
using DataAccessLayer.Entities;

namespace BusinessLogicLayer.Mappers;

public class ProductMappingProfile : Profile
{
    public ProductMappingProfile()
    {
        // Entity -> Response:欄位名稱一致,AutoMapper 依名稱自動對應(包含 record 的建構子參數)
        CreateMap<Product, ProductResponse>();

        // AddRequest -> Entity:Request 沒有 ProductID,主鍵不由 client 決定
        CreateMap<ProductAddRequest, Product>()
            .ForMember(dest => dest.ProductID, opt => opt.Ignore());

        // UpdateRequest -> Entity:四個欄位加 ProductID 全部對得上
        CreateMap<ProductUpdateRequest, Product>();
    }
}
