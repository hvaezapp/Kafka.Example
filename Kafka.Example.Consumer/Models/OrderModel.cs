namespace Kafka.Example.Consumer.Models;

public class OrderModel
{
    public Guid OrderId { get; set; }
    public string OrderName { get; set; } = null!;
}
