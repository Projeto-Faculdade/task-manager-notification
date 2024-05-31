using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace TaskManagerNotification.WorkerService.BrokerMesage;

internal class RabbitMqBrokerService(IConfiguration configuration)
{
    private IModel GetChanell()
    {
        var factory = new ConnectionFactory
        {
            HostName = configuration["RabbitMq:HostName"],
            DispatchConsumersAsync = true
        };
        var connection = factory.CreateConnection();
        var channel = connection.CreateModel();
        return channel;
    }

    public IModel CreateQueueIfNotExists()
    {
        var channel = GetChanell();

        channel.QueueDeclare(queue: configuration["RabbitMq:Queue"],
                             durable: true,
                             exclusive: false,
                             autoDelete: false,
                             arguments: null);
        return channel;
    }

    public AsyncEventingBasicConsumer Consummer()
    {
        var channel = GetChanell();

        var consumer = new AsyncEventingBasicConsumer(channel);
        channel.BasicConsume(queue: configuration["RabbitMq:Queue"],
                             autoAck: true,
                             consumer: consumer);
        return consumer;
    }
}