using TaskManagerNotification.WorkerService;
using TaskManagerNotification.WorkerService.BrokerMesage;
using TaskManagerNotification.WorkerService.Emails;

var builder = Host.CreateApplicationBuilder(args);
var services = builder.Services;

services.AddSingleton<EmailService>();
services.AddSingleton<RabbitMqBrokerService>();
services.AddHostedService<Worker>();

var host = builder.Build();
host.Run();