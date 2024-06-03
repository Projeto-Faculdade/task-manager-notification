namespace TaskManagerNotification.WorkerService.Emails.NewTask;

internal class NewTaskPortugueseEmail : IEmail
{
    private string FirstName => Recipients[0].Name.Split(" ")[0];

    public List<Recipient> Recipients { get; set; } = []!;

    public string Key => "newtask-pt";

    public string Subject => "Nova atividade para você";

    public string PlainText => $"Oi {FirstName}! Você tem uma nova atividade.";

    public string Content => $"Oi {FirstName}! Você tem uma nova atividade.";
}
