using Confluent.Kafka;
using Microsoft.Extensions.Options;

namespace Kafka.Example.Consumer.Services;

public interface IKafkaConsumerService<TKey, TValue, TValueDeserializer> where TValueDeserializer : IDeserializer<TValue>, new()
{
    Task RegisterConsumer(CancellationToken cancellationToken, string topic, Func<TValue, Task> handler);
}

public class KafkaConsumerService<TKey, TValue, TValueDeserializer> : IKafkaConsumerService<TKey, TValue, TValueDeserializer> where TValueDeserializer : IDeserializer<TValue>, new()
{
    private readonly IConsumer<TKey, TValue> _consumer;
    public KafkaConsumerService(IOptions<ConsumerConfig> config)
    {
        _consumer = new ConsumerBuilder<TKey, TValue>(config.Value)
            .SetValueDeserializer(new TValueDeserializer())
            .Build();
    }

    public async Task RegisterConsumer(CancellationToken cancellationToken, string topic, Func<TValue, Task> handler)
    {
        _consumer.Subscribe(topic);


        while (!cancellationToken.IsCancellationRequested)
        {

            var consumeResult = _consumer.Consume(cancellationToken); //Blocking

            await handler(consumeResult.Message.Value);
        }
    }
}