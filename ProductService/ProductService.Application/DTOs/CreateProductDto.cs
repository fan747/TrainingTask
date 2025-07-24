namespace ProductService.Application.DTOs;

public record CreateProductDto(
    string Name,
    int Quantity,
    decimal Price   
    );