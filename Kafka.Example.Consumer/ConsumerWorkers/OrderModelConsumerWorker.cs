using Confluent.Kafka;
using Kafka.Example.Consumer.Deserializer;
using Kafka.Example.Consumer.Models;
using Kafka.Example.Consumer.Services;

namespace Kafka.Example.Consumer.ConsumerWorkers;

public class OrderModelConsumerWorker(
        IKafkaConsumerService<Null, OrderModel, KafkaJsonDeserializer<OrderModel>> kafkaConsumer,
        ILogger<OrderModelConsumerWorker> logger)
    : BackgroundService
{
    private readonly IKafkaConsumerService<Null, OrderModel, KafkaJsonDeserializer<OrderModel>> _kafkaConsumer = kafkaConsumer;
    readonly ILogger<OrderModelConsumerWorker> _logger = logger;


    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        await Task.Yield();

        await kafkaConsumer.RegisterConsumer(stoppingToken, "order-fake-topic", async model =>
        {
            _logger.LogWarning("Order With ID {Id} and name {name} received", model.OrderId, model.OrderName);
            await Task.CompletedTask;
        });
    }
}