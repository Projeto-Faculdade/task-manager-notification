using System.Text;
using System.Text.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace TaskManagerNotification.WorkerService
{
    public class Worker(ILogger<Worker> logger) : BackgroundService
    {
        private sealed record Test(int Id);

        public override Task StartAsync(CancellationToken cancellationToken)
        {
            var factory = new ConnectionFactory { HostName = "localhost" };
            using var connection = factory.CreateConnection();
            using var channel = connection.CreateModel();

            channel.QueueDeclare(queue: "test",
                                 durable: true,
                                 exclusive: false,
                                 autoDelete: false,
                                 arguments: null);

            for (int i = 0; i < 10; i++)
            {
                var body = Encoding.UTF8.GetBytes(JsonSerializer.Serialize(new Test(i)));
                channel.BasicPublish(exchange: string.Empty,
                     routingKey: "test",
                     body: body);

                logger.LogInformation("Publlish {@Id}", i);
            }

            return base.StartAsync(cancellationToken);
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            var factory = new ConnectionFactory { HostName = "localhost" };
            using var connection = factory.CreateConnection();
            using var channel = connection.CreateModel();

            var consumer = new EventingBasicConsumer(channel);

            consumer.Received += ConsumerRecived;
            channel.BasicConsume(queue: "test",
                                 autoAck: true,
                                 consumer: consumer);

            while (!stoppingToken.IsCancellationRequested)
            {
                logger.LogInformation("Worker ativo");
                await Task.Delay(TimeSpan.FromMinutes(1), stoppingToken);
            }
        }

        private void ConsumerRecived(object? sender, BasicDeliverEventArgs ea)
        {
            var body = ea.Body.ToArray();
            var message = Encoding.UTF8.GetString(body);
            logger.LogWarning("Received message: {@Message}", message);
        }
    }
}