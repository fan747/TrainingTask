namespace OrderService.Application.DTOs;

public record OrderDto(
    int Id,
    int ProductId,
    int Quantity
);
