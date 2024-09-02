using System.Text;
using System.Text.Json;
using Bank.Loan.Application.Infrastructure;
using Microsoft.Extensions.Configuration;
using RabbitMQ.Client;

namespace Bank.Loan.Infrastructure.Messaging;

public class RabbitMqService : IRabbitMqService //TODO I need separate consumera nd producer rmq services
{
    private readonly string _hostname;
    private readonly string _queueName;
    private readonly IConnection _connection;
    private readonly IModel _channel; //TODO think about rabbit settings

    public RabbitMqService(IConfiguration configuration)
    {
        _hostname = configuration["RabbitMq:HostName"]; //TODO appsettings
        _queueName = "loans";

        var factory = new ConnectionFactory()
        {
            HostName = _hostname,
            UserName = "user",
            Password = "password" //TODO appsecretize it
        };
        _connection = factory.CreateConnection();
        _channel = _connection.CreateModel();
        _channel.QueueDeclare(queue: _queueName, durable: false, exclusive: false, autoDelete: false, arguments: null);
    }
    
    public (T message, ulong DeliveryTag) FetchJsonMessage<T>()
    {
        var result = _channel.BasicGet(_queueName, autoAck: false); //TODO queue name should not be same for all rabbit service consumers
        if (result == null)
        {
            throw new ApplicationException(); //TODO change excep
        }

        var message = Encoding.UTF8.GetString(result.Body.ToArray());
        var deserializedMessage = JsonSerializer.Deserialize<T>(message);
        return (deserializedMessage, result.DeliveryTag);
    }
    
    public void AcknowledgeMessage(ulong deliveryTag)
    {
        _channel.BasicAck(deliveryTag, multiple: false);
    }
    
    public void SendJsonMessage<T>(T message) //TODO make it async, DispatchConsumersAsync true
    {
        var json = JsonSerializer.Serialize(message);
        var body = Encoding.UTF8.GetBytes(json);

        _channel.BasicPublish(exchange: "", routingKey: _queueName, basicProperties: null, body: body);
    }

    public void Dispose()
    {
        _channel.Close();
        _connection.Close();
    }
}