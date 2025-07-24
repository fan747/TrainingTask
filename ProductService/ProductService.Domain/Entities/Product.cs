namespace ProductService.Domain.Entities;

public class Product
{
    public int Id { get; private set; }
    public string Name { get; private set; }
    public int Quantity { get; private set; }
    public decimal Price { get; private set; }
    
    private Product()
    {
    }

    public static Product Create(string name, int quantity, decimal price)
    {
        Validate(name, quantity, price);

        return new Product()
        {
            Name = name,
            Quantity = quantity,
            Price = price
        };
    }

    public void Update(string name, int quantity, decimal price)
    {
        Validate(name, quantity, price);
        
        Name = name;
        Quantity = quantity;
        Price = price;
    }

    private static void Validate(string name, int quantity, decimal price)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Name cannot be null or whitespace", nameof(name));
        if (quantity < 0)
            throw new ArgumentException("Quantity cannot be negative", nameof(quantity));
        if (price <= 0) 
            throw new ArgumentException("Price must be positive", nameof(price));
    }
}