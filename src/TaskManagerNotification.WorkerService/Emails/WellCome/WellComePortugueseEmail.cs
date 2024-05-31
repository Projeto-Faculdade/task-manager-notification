namespace TaskManagerNotification.WorkerService.Emails.WellCome;

internal class WellComePortugueseEmail : IEmail
{
    private string FirstName => Recipients[0].Name.Split(" ")[0];

    public string Key => "wellcome-pt";

    public List<Recipient> Recipients { get; set; } = []!;

    public string Subject => "Seja bem vindo(a)!";

    public string PlainText => $"Seja bem vindo(a) {FirstName}! ";

    public string Content => File.ReadAllText($"{Path.Combine("Emails","WellCome", nameof(WellComePortugueseEmail))}.html")
        .Replace("{@Name}", FirstName);
}