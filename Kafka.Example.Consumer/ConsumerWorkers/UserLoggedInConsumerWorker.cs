using Confluent.Kafka;
using Kafka.Example.Consumer.Deserializer;
using Kafka.Example.Consumer.Protos;
using Kafka.Example.Consumer.Services;

namespace Kafka.Example.Consumer.ConsumerWorkers;

public class UserLoggedInConsumerWorker : BackgroundService
{
    readonly ILogger<UserLoggedInConsumerWorker> _logger;
    readonly IKafkaConsumerService<Null, PersonLoggedIn, KafkaProtoBufDeserializer<PersonLoggedIn>> _kafkaConsumerService;

    public UserLoggedInConsumerWorker(ILogger<UserLoggedInConsumerWorker> logger, IKafkaConsumerService<Null, PersonLoggedIn, KafkaProtoBufDeserializer<PersonLoggedIn>> kafkaConsumerService)
    {
        _logger = logger;
        _kafkaConsumerService = kafkaConsumerService;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        await Task.Yield();

        await _kafkaConsumerService.RegisterConsumer(stoppingToken, "logins-fake-topic", async personLoggedIn =>
        {
            _logger.LogInformation("Person With UserName {userName} and Id {Id} logged On {Date}", personLoggedIn.UserName, personLoggedIn.UserId, personLoggedIn.LoggedInDate.ToDateTime());

            await Task.CompletedTask;
        });
    }
}