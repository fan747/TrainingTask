namespace OrderService.Application.DTOs;

public record CreateOrderDto(
    string IdempotencyKey,
    CheckProductDto CheckProduct
    );