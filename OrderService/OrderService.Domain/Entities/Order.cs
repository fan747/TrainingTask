namespace OrderService.Application.Handlers;

public class Order
{
    public int Id { get; private set; }
    public int ProductId { get; private set; }
    public int Quantity { get; private set; }

    private Order()
    {
        
    }

    public static Order Create(int productId, int quantity)
    {
        if (productId < 0)
            throw new ArgumentException("Product id cannot be negative", nameof(productId));
        if (quantity < 0)
            throw new ArgumentException("Quantity cannot be negative", nameof(quantity));

        return new Order()
        {
            ProductId = productId,
            Quantity = quantity
        };
    }
    
    
}