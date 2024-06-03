using System.Text;
using System.Text.Json;
using RabbitMQ.Client.Events;
using TaskManagerNotification.WorkerService.BrokerMesage;
using TaskManagerNotification.WorkerService.Emails;

namespace TaskManagerNotification.WorkerService;

internal class Worker(
    ILogger<Worker> logger,
    RabbitMqBrokerService mqBrokerService,
    EmailService emailService)
    : BackgroundService
{
    public override async Task StartAsync(CancellationToken cancellationToken)
    {
        using var channel = mqBrokerService.CreateQueueIfNotExists();

        await base.StartAsync(cancellationToken);
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var consumer = mqBrokerService.Consummer();

        consumer.Received += ConsumerRecivedAsync;

        while (!stoppingToken.IsCancellationRequested)
        {
            logger.LogInformation("Worker ativo {@Runing}", consumer.IsRunning);
            await Task.Delay(TimeSpan.FromMinutes(1), stoppingToken);
        }
    }

    private async Task ConsumerRecivedAsync(object? sender, BasicDeliverEventArgs ea)
    {
        var body = ea.Body.ToArray();
        var message = Encoding.UTF8.GetString(body);
        logger.LogInformation("Received message: {@Message}", message);
        var emailConfiguration = JsonSerializer.Deserialize<Message>(message)!;

        var email = IEmail.GetTemplate(emailConfiguration.EmailType);

        email.Recipients = emailConfiguration.Recipients;
#if SENDEMAIL
        await emailService.SendEmail(email);
#endif
        logger.LogInformation("Send email to: {@Message}", email.Key);

        await Task.Yield();
    }
}