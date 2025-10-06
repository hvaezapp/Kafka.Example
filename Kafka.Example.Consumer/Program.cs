using Confluent.Kafka;
using Kafka.Example.Consumer.ConsumerWorkers;
using Kafka.Example.Consumer.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddSingleton(typeof(IKafkaConsumerService<,,>), typeof(KafkaConsumerService<,,>));
builder.Services.Configure<ConsumerConfig>(config =>
{
    config.ClientId = "Kafka-test-consumer";
    config.BootstrapServers = "localhost:9092";
    config.GroupId = "Kafka-test-consumer-group";
});

builder.Services.AddHostedService<OrderModelConsumerWorker>();
builder.Services.AddHostedService<UserLoggedInConsumerWorker>();

var app = builder.Build();

app.Run();

