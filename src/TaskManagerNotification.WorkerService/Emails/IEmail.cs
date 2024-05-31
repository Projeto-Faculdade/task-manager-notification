namespace TaskManagerNotification.WorkerService.Emails;

internal interface IEmail
{
    List<Recipient> Recipients { get; set; }

    string Key { get; }
    string Subject { get; }
    string PlainText { get; }
    string Content { get; }

    public static IEmail GetTemplate(string key) => typeof(IEmail).Assembly
      .GetTypes()
      .Where(typeof(IEmail).IsAssignableFrom)
      .Where(t => t is { IsClass: true, IsAbstract: false })
      .Select(t => (IEmail)Activator.CreateInstance(t)!)
      .ToDictionary(e => e.Key, e => e)[key];
}