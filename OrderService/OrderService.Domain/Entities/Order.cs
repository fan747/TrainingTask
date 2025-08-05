using OrderService.Domain.Exceptions;

namespace OrderService.Domain.Entities;

public class Order : IEquatable<Order>
{
    public int Id { get; private set; }
    public int ProductId { get; private set; }
    public int Quantity { get; private set; }

    private Order()
    {
        
    }

    public static Order Create(int productId, int quantity)
    {
        if (productId <= 0)
            throw new IncorrectDataException("Product id cannot be negative");
        if (quantity <= 0)
            throw new IncorrectDataException("Quantity cannot be negative");

        return new Order()
        {
            ProductId = productId,
            Quantity = quantity
        };
    }


    public bool Equals(Order? other)
    {
        if (other is null) return false;
        if (ReferenceEquals(this, other)) return true;
        return Id == other.Id && ProductId == other.ProductId && Quantity == other.Quantity;
    }

    public override bool Equals(object? obj)
    {
        if (obj is null) return false;
        if (ReferenceEquals(this, obj)) return true;
        if (obj.GetType() != GetType()) return false;
        return Equals((Order)obj);
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(Id, ProductId, Quantity);
    }
}