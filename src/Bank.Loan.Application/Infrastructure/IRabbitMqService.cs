namespace Bank.Loan.Application.Infrastructure;

public interface IRabbitMqService : IDisposable
{
    (T message, ulong DeliveryTag) FetchJsonMessage<T>();
    public void AcknowledgeMessage(ulong deliveryTag);
    void SendJsonMessage<T>(T message);
}