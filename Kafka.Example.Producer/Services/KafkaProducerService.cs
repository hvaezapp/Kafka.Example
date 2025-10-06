namespace Kafka.Example.Producer.Services;

public interface IKafkaProducerService<TKey, TValue, TSerializer> where TSerializer : ISerializer<TValue>, new()
{
    Task ProduceAsync(string topic, Message<TKey, TValue> message);
}

public class KafkaProducerService<TKey, TValue, TSerializer>(IOptions<ProducerConfig> config) : IDisposable, IKafkaProducerService<TKey, TValue, TSerializer>
where TSerializer : ISerializer<TValue>, new()
{

    private readonly IProducer<TKey, TValue> _producer = new ProducerBuilder<TKey, TValue>(config.Value).SetValueSerializer(new TSerializer()).Build();

    public async Task ProduceAsync(string topic, Message<TKey, TValue> message)
    {
        var deliveryResult = await _producer.ProduceAsync(topic, message);
        Console.WriteLine($"Message delivered to {deliveryResult.TopicPartitionOffset}");
    }

    public void Dispose()
    {
        _producer.Dispose();
        GC.SuppressFinalize(this);
    }
}