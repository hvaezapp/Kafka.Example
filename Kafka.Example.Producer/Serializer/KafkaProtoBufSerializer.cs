namespace Kafka.Example.Producer.Serializer;

public class KafkaProtoBufSerializer<TMessageModel>:ISerializer<TMessageModel> where TMessageModel : IMessage<TMessageModel>, new()
{
    public byte[] Serialize(TMessageModel data, SerializationContext context)
    {
        return data.ToByteArray();
    }
}