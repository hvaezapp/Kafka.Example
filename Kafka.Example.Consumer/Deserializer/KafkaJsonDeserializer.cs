using System.Text.Json;
using Confluent.Kafka;

namespace Kafka.Example.Consumer.Deserializer;

public class KafkaJsonDeserializer<TMessageModel>:IDeserializer<TMessageModel> where TMessageModel : class,new()
{
    public TMessageModel Deserialize(ReadOnlySpan<byte> data, bool isNull, SerializationContext context)
    {
        return JsonSerializer.Deserialize<TMessageModel>(data,
            new JsonSerializerOptions() { PropertyNameCaseInsensitive = false })!;
    }
}