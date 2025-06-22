using MailKit.Net.Smtp;
using MimeKit;
using System.Threading.Tasks;

public class EmailSender
{
    public async Task SendEmailAsync(string toEmail, string subject, string htmlMessage)
    {
        var email = new MimeMessage();
        email.From.Add(MailboxAddress.Parse("vduyit@gmail.com")); // Email gửi
        email.To.Add(MailboxAddress.Parse(toEmail));
        email.Subject = subject;

        var builder = new BodyBuilder { HtmlBody = htmlMessage };
        email.Body = builder.ToMessageBody();

        using var smtp = new SmtpClient();
        await smtp.ConnectAsync("smtp.gmail.com", 587, MailKit.Security.SecureSocketOptions.StartTls);
        await smtp.AuthenticateAsync("vduyit@gmail.com", "ygbw urvt vkch rksm"); // Dùng app password nếu Gmail
        await smtp.SendAsync(email);
        await smtp.DisconnectAsync(true);
    }
}
