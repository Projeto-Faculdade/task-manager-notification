using System.Text.Json.Serialization;

namespace TaskManagerNotification.WorkerService;

internal record Recipient
{
    [JsonPropertyName("name")]
    public string Name { get; set; } = string.Empty!;

    [JsonPropertyName("email")]
    public string Email { get; set; } = string.Empty!;
}

internal record Message
{
    [JsonPropertyName("emailType")]
    public string EmailType { get; set; } = string.Empty!;
    [JsonPropertyName("recipients")]
    public List<Recipient> Recipients { get; set; } = []!;
}