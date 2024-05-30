using System.Text;
using System.Text.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using SendGrid;
using SendGrid.Helpers.Mail;

namespace TaskManagerNotification.WorkerService;

public class Worker(
    ILogger<Worker> logger,
    IConfiguration configuration)
    : BackgroundService
{

    public override async Task StartAsync(CancellationToken cancellationToken)
    {
        await SendEmail(cancellationToken);
        using var channel = GetChanell();

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

        await base.StartAsync(cancellationToken);
    }

    private async Task SendEmail(CancellationToken cancellationToken)
    {
        var client = new SendGridClient(configuration["SendGrid:ApiKey"]);
        var from = new EmailAddress(configuration["SendGrid:Sender"], "Example User");

        var tos = new List<EmailAddress>
        {
            new("email@hotmail.com", "Example User")
        };

        var subject = "Sending with SendGrid is Fun";
        var plainTextContent = "and easy to do anywhere, even with C#";
        var htmlContent = "<strong>and easy to do anywhere, even with C#</strong>";
        var msg = MailHelper.CreateSingleEmailToMultipleRecipients(from, tos, subject, plainTextContent, htmlContent);
        var response = await client.SendEmailAsync(msg, cancellationToken);

        logger.LogInformation("Sendgrig {@Status}", response.StatusCode);
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        using var channel = GetChanell();

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

    private static IModel GetChanell()
    {
        var factory = new ConnectionFactory { HostName = "localhost" };
        var connection = factory.CreateConnection();
        var channel = connection.CreateModel();
        return channel;
    }

    private void ConsumerRecived(object? sender, BasicDeliverEventArgs ea)
    {
        var body = ea.Body.ToArray();
        var message = Encoding.UTF8.GetString(body);
        logger.LogWarning("Received message: {@Message}", message);
    }

    private sealed record Test(int Id);
}