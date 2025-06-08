using Microsoft.AspNetCore.Identity.UI.Services;
using System.Threading.Tasks;

public class EmailSender : IEmailSender
{
    public Task SendEmailAsync(string email, string subject, string htmlMessage)
    {
        // Just log or skip for dev purposes
        Console.WriteLine($"Sending email to {email} with subject: {subject}");
        return Task.CompletedTask;
    }
}
