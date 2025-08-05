namespace OrderService.Domain.Entities;

public class IdempotencyKey
{
    public int Id { get; set; }
    public string Key { get; set; }
    
    private IdempotencyKey() { }
    
    public static IdempotencyKey Create(string key) => new() { Key = key };
}