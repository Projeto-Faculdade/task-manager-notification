using SendGrid;
using SendGrid.Helpers.Mail;

namespace TaskManagerNotification.WorkerService.Emails;

internal class EmailService(ILogger<EmailService> logger, IConfiguration configuration)
{
    public async Task SendEmail(IEmail email)
    {
        var client = new SendGridClient(configuration["SendGrid:ApiKey"]);
        var from = new EmailAddress(configuration["SendGrid:Sender"], "Task Manager Notification");

        var tos = email.Recipients
                  .Select(r => new EmailAddress(r.Email, r.Name))
                  .ToList();

        var msg = MailHelper.CreateSingleEmailToMultipleRecipients(from, tos, email.Subject, email.PlainText, email.Content);
        var response = await client.SendEmailAsync(msg);

        logger.LogInformation("Sendgrig {@Status}", response.IsSuccessStatusCode);
    }
}