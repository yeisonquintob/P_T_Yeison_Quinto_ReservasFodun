using System.Net;
using System.Net.Mail;
using Microsoft.Extensions.Options;
using ReservasFodun.Application.Interfaces.Services;

namespace ReservasFodun.Infrastructure.Email;

public class EmailService : IEmailService
{
    private readonly SmtpSettings _smtpSettings;

    public EmailService(IOptions<SmtpSettings> smtpOptions)
    {
        _smtpSettings = smtpOptions.Value;
    }

    public async Task SendEmailAsync(
        string toEmail,
        string subject,
        string htmlMessage,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(toEmail))
            throw new ArgumentException("El correo destinatario es obligatorio.", nameof(toEmail));

        if (string.IsNullOrWhiteSpace(subject))
            throw new ArgumentException("El asunto del correo es obligatorio.", nameof(subject));

        if (string.IsNullOrWhiteSpace(htmlMessage))
            throw new ArgumentException("El mensaje del correo es obligatorio.", nameof(htmlMessage));

        if (string.IsNullOrWhiteSpace(_smtpSettings.Host))
            throw new InvalidOperationException("El servidor SMTP no está configurado.");

        if (_smtpSettings.Port <= 0)
            throw new InvalidOperationException("El puerto SMTP no está configurado correctamente.");

        if (string.IsNullOrWhiteSpace(_smtpSettings.FromEmail))
            throw new InvalidOperationException("El correo remitente SMTP no está configurado.");

        using var message = new MailMessage
        {
            From = new MailAddress(
                _smtpSettings.FromEmail,
                string.IsNullOrWhiteSpace(_smtpSettings.FromName)
                    ? "ReservasFodun"
                    : _smtpSettings.FromName),
            Subject = subject,
            Body = htmlMessage,
            IsBodyHtml = true
        };

        message.To.Add(toEmail);

        using var smtpClient = new SmtpClient(_smtpSettings.Host, _smtpSettings.Port)
        {
            DeliveryMethod = SmtpDeliveryMethod.Network,
            UseDefaultCredentials = false,
            EnableSsl = _smtpSettings.EnableSsl,
            Timeout = 30000
        };

        if (!string.IsNullOrWhiteSpace(_smtpSettings.UserName) &&
            !string.IsNullOrWhiteSpace(_smtpSettings.Password))
        {
            smtpClient.Credentials = new NetworkCredential(
                _smtpSettings.UserName,
                _smtpSettings.Password);
        }

        await smtpClient.SendMailAsync(message, cancellationToken);
    }
}
