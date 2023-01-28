using System.Net;
using System.Net.Mail;

namespace Api.Blog.Services;

public class EmailService
{
    public bool Send(string toName,
        string toEmail,
        string subject,
        string body,
        string fromName = "Equipe balta.io",
        string fromEmail = "email@balta.io")
    {
        SmtpClient smtpClient = CreateAndConfigurationEmail();
        MailMessage mail = CreateMailMessage(toName, toEmail, subject, body, fromName, fromEmail);

        try
        {
            smtpClient.Send(mail);
            return true;
        }
        catch (Exception ex)
        {
            return false;
        }
    }

    private static SmtpClient CreateAndConfigurationEmail()
    {
        var smtpClient = new SmtpClient(Configuration.Smtp.Host, Configuration.Smtp.Port);
        smtpClient.Credentials = new NetworkCredential(Configuration.Smtp.UserName, Configuration.Smtp.Password);
        smtpClient.DeliveryMethod = SmtpDeliveryMethod.Network;
        smtpClient.EnableSsl = true;
        return smtpClient;
    }

    private MailMessage CreateMailMessage(string toName, string toEmail, string subject, string body, string fromName, string fromEmail)
    {
        var mail = new MailMessage();
        mail.From = new MailAddress(fromEmail, fromName);
        mail.To.Add(new MailAddress(toEmail, toName));
        mail.Subject = subject;
        mail.Body = body;
        mail.IsBodyHtml = true;
        return mail;
    }

   
}
