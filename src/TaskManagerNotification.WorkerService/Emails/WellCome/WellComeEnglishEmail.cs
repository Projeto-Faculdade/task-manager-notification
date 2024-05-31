
namespace TaskManagerNotification.WorkerService.Emails.WellCome;

internal class WellComeEnglishEmail : IEmail
{
    private string FirstName => Recipients[0].Name.Split(" ")[0];
    public string Key => "wellcome-en";

    public List<Recipient> Recipients { get; set; } = []!;

    public string Subject => "Well Come!";

    public string PlainText => $"Well Come {FirstName}! ";
    public string Content => File.ReadAllText($"{Path.Combine("Emails", "WellCome", nameof(WellComeEnglishEmail))}.html")
        .Replace("{@Name}", FirstName);

}