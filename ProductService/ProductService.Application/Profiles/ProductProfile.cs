using AutoMapper;
using ProductService.Application.DTOs;
using ProductService.Domain.Entities;

namespace ProductService.Application.Profiles;

public class ProductProfile : Profile
{
    public ProductProfile()
    {
        CreateMap<Product, CreateProductDto>();
        CreateMap<CreateProductDto, Product>()
            .ConstructUsing(dto => Product.Create(dto.Name, dto.Quantity, dto.Price));
        CreateMap<Product, ProductDto>()
            .ConstructUsing(p=> new ProductDto(p.Id, p.Name, p.Quantity, p.Price));
    }    
}