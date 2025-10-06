using Confluent.Kafka;
using Google.Protobuf;

namespace Kafka.Example.Consumer.Deserializer;

public class KafkaProtoBufDeserializer<TMessageModel> : IDeserializer<TMessageModel> where TMessageModel : class, IMessage<TMessageModel>, new()
{
    public TMessageModel Deserialize(ReadOnlySpan<byte> data, bool isNull, SerializationContext context)
    {
        if (isNull)
            return default!;

        var instance = new TMessageModel();

        instance.MergeFrom(data.ToArray());

        return instance;

    }
}