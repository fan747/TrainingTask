using Microsoft.EntityFrameworkCore;
using OrderService.Application.Abstractions;
using OrderService.Application.DTOs;
using OrderService.Application.Handlers;
using OrderService.Application.Profiles;
using OrderService.Application.Results;
using OrderService.Infrastructure.Options;
using OrderService.Infrastructure.Persistence;
using OrderService.Infrastructure.Persistence.RabbitMQ;
using OrderService.Infrastructure.Repositories;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(cfg =>
{
    cfg.SwaggerDoc("v1", new() { Title = "OrderService.API", Version = "v1", Description = "Сервис заказов"});  
});

builder.Services.AddMediatR(cfg =>
{
    cfg.RegisterServicesFromAssembly(typeof(CreateOrderCommandHandler).Assembly);
});

builder.Services.AddDbContext<OrderDbContext>(cfg =>
{
    cfg.UseInMemoryDatabase(databaseName: "Orders");
});

builder.Services.AddAutoMapper(cfg =>
{
    cfg.AddProfile<OrderProfile>();
});
builder.Services.Configure<RabbitMqOptions>(builder.Configuration.GetSection(RabbitMqOptions.SectionName));

builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
builder.Services.AddSingleton<IQueuePublisher<CheckProductDto, Result<CheckProductDto>>, RabbitMqPublisher<CheckProductDto, Result<CheckProductDto>>>();


var app = builder.Build();

app.MapControllers();
app.UseSwagger();
app.UseSwaggerUI();

app.UseHttpsRedirection();

app.Run();
