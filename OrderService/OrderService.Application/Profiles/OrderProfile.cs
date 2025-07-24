using AutoMapper;
using OrderService.Application.DTOs;
using OrderService.Application.Handlers;

namespace OrderService.Application.Profiles;

public class OrderProfile : Profile
{
    public OrderProfile()
    {
        CreateMap<Order, OrderDto>();
    }
}