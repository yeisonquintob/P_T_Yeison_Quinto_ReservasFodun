namespace ReservasFodun.Application.Interfaces.Services;

public interface IEmailService
{
    Task SendEmailAsync(
        string toEmail,
        string subject,
        string htmlMessage,
        CancellationToken cancellationToken = default);
}
