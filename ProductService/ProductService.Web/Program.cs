using Microsoft.EntityFrameworkCore;
using ProductService.Application.Abstractions;
using ProductService.Application.DTOs;
using ProductService.Application.Handlers;
using ProductService.Application.Profiles;
using ProductService.Infrastructure.Options;
using ProductService.Infrastructure.Persistence;
using ProductService.Infrastructure.Persistence.RabbitMQ;
using ProductService.Infrastructure.Repositories;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(cfg =>
{
    cfg.SwaggerDoc("v1", new() { Title = "ProductService.API", Version = "v1", Description = "Сервис продуктов"});  
    
});



builder.Services.AddDbContext<ProductDbContext>(cfg =>
{
    cfg.UseInMemoryDatabase(databaseName: "Products");
});

builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();

builder.Services.AddAutoMapper(cfg =>
{
    cfg.AddProfile<ProductProfile>();   
});
builder.Services.AddMediatR(cfg =>
{
    cfg.RegisterServicesFromAssembly(typeof(GetProductByIdQueryHandler).Assembly);
    cfg.RegisterServicesFromAssembly(typeof(CreateProductCommandHandler).Assembly);
});

builder.Services.Configure<RabbitMqOptions>(builder.Configuration.GetSection(RabbitMqOptions.SectionName));

builder.Services.AddHostedService<RabbitMqConsumer<CheckProductDto>>();

var app = builder.Build();

app.MapControllers();

app.UseSwagger();
app.UseSwaggerUI();


app.UseHttpsRedirection();

app.Run();
