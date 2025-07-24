http://localhost:5267/swagger/index.html - Order Service

http://localhost:5262/swagger/index.html - Product Service

Для запуска надо сбилдить compose.yaml

REST API + RabbittMQ ( использовал  еще MediatR и AutoMapper )

[Order publisher](OrderService/OrderService.Infrastructure/Persistence/RabbitMQ/RabbitMqPublisher.cs)
[Product consumer](ProductService/ProductService.Infrastructure/Persistence/RabbitMQ/RabbitMqConsumer.cs)

1. Идет сообщение из Order Service к Product Service c CheckProductDto
2. Product Service отправляет Query в медиатр и [CheckAndReserveCommandHandler](ProductService/ProductService.Application/Handlers/CheckAndReserveCommandHandler.cs) проверяет есть ли товар в наличии и если есть удаляет и возвращает Result<CheckProductDto>.Succes(CheckProductDto
если нету или есть ошибка то Result<CheckProductDto>.Failure(error.message)
3. Result<CheckProductDto> возвращается в [Handler](https://github.com/fan747/TrainingTask/blob/main/OrderService/OrderService.Application/Handlers/CreateOrderCommandHandler.cs) и уходит пользователю ( без CheckProductDto )
