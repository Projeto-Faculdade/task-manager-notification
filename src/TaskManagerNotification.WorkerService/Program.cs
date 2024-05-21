using TaskManagerNotification.WorkerService;

var builder = Host.CreateApplicationBuilder(args);
var services = builder.Services;
services.AddHostedService<Worker>();

var host = builder.Build();
host.Run();
