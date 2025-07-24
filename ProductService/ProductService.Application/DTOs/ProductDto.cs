namespace ProductService.Application.DTOs;

public record ProductDto(
    int Id,
    string? Name = null,
    int? Quantity = null,
    decimal? Price = null   
);