using Bogus;
using Timestamp = Google.Protobuf.WellKnownTypes.Timestamp;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOpenApi();

builder.Services.Configure<ProducerConfig>(config =>
{
    config.ClientId = "Kafka-test-producer";
    config.BootstrapServers = "localhost:9092";
});

builder.Services.AddSingleton(typeof(IKafkaProducerService<,,>), typeof(KafkaProducerService<,,>));

builder.Services.AddMassTransit(configurator =>
{
    configurator.UsingInMemory();
    configurator.AddRider(rider =>
    {
        rider.AddProducer<Null, OrderModel>("order-fake-topic", (x, s) =>
        {
            s.SetValueSerializer(new KafkaJsonSerializer<OrderModel>());
        });

        rider.UsingKafka((context, kafkaConfig) =>
        {
            kafkaConfig.Host("localhost:9092");
        });
    });
});


var app = builder.Build();


if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference();
}


app.MapPost("/OrderSubmitter", async (IKafkaProducerService<Null
    , OrderModel
    , KafkaJsonSerializer<OrderModel>> service) =>
{
    var fakeOrders = new Faker<OrderModel>()
        .RuleFor(c => c.OrderId, Guid.NewGuid)
        .RuleFor(c => c.OrderName, f => f.Commerce.Product())
        .Generate(10000);

    foreach (var orderModel in fakeOrders)
    {
        await service.ProduceAsync("order-fake-topic", new Message<Null, OrderModel>() { Value = orderModel });
    }
});


app.MapPost("/OrderSubmitterMassTransit", async (ITopicProducerProvider producer) =>
{
    var fakeOrders = new Faker<OrderModel>()
        .RuleFor(c => c.OrderId, Guid.NewGuid)
        .RuleFor(c => c.OrderName, f => f.Commerce.Product())
        .Generate(10);

    var messageProducer = producer.GetProducer<OrderModel>(new Uri("topic:order-fake-topic"));

    foreach (var orderModel in fakeOrders)
    {
        await messageProducer.Produce(orderModel);
    }
});

app.MapPost("/UserLoggedIn", async (IKafkaProducerService<Null
    , PersonLoggedIn
    , KafkaProtoBufSerializer<PersonLoggedIn>> service) =>
{

    var fakeLogins = new Faker<PersonLoggedIn>()
        .RuleFor(c => c.UserName, f => f.Person.UserName)
        .RuleFor(c => c.LoggedInDate, f => Timestamp.FromDateTime(f.Date.Future(1, DateTime.UtcNow)))
        .RuleFor(c => c.UserId, f => f.IndexFaker)
        .Generate(1000);

    foreach (var fakeLogin in fakeLogins)
    {
        await service.ProduceAsync("logins-fake-topic", new Message<Null, PersonLoggedIn>() { Value = fakeLogin });
    }
});


app.Run();

