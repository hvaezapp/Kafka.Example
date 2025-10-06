namespace Kafka.Example.Producer.Models;
public class OrderModel
{
    public Guid OrderId { get; set; }
    public string OrderName { get; set; } = null!;
}
