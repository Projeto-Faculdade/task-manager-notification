namespace TaskManagerNotification.WorkerService.Emails.NewTask;

internal class NewTaskEnglisEmail : IEmail
{
    private string FirstName => Recipients[0].Name.Split(" ")[0];

    public List<Recipient> Recipients { get; set; } = []!;

    public string Key => "newtask-en";

    public string Subject => "New activity for you";

    public string PlainText => $"Hi {FirstName}! You have a new activity.";

    public string Content => $"Hi {FirstName}! You have a new activity.";
}