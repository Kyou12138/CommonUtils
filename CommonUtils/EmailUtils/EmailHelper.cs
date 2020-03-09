using CommonUtils.ConfigUtils;
using System;
using System.IO;
using System.Net;
using System.Net.Mail;
using System.Text.RegularExpressions;

namespace CommonUtils.EmailUtils
{
    public class EmailHelper : IDisposable
    {
        private static readonly string EmailConfigFilePath = @"EmailUtils\email.config";
        public EmailConfig config;
        private SmtpClient smtp;

        public EmailHelper()
        {
            config = ConfigHelper.GetConfig<EmailConfig>(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, EmailConfigFilePath));
            smtp = new SmtpClient();
            smtp.Host = config.Host;
            smtp.EnableSsl = config.EnableSsl;
            smtp.Credentials = new NetworkCredential(config.User, config.Password);
            smtp.DeliveryMethod = SmtpDeliveryMethod.Network;
        }

        public EmailHelper(EmailConfig config)
        {
            this.config = config;
            smtp = new SmtpClient();
            smtp.Host = config.Host;
            smtp.EnableSsl = config.EnableSsl;
            smtp.Credentials = new NetworkCredential(config.User, config.Password);
            smtp.DeliveryMethod = SmtpDeliveryMethod.Network;
        }

        public bool SendEmail(string subject, string content, string toAddress = "", string attachmentPath = "")
        {
            try
            {
                if (string.IsNullOrEmpty(toAddress)) toAddress = config.ReceiveEmailAddress;
                MailMessage mail = new MailMessage(config.UserEmailAddress, toAddress, subject, content);
                if (!string.IsNullOrEmpty(attachmentPath))
                {
                    mail.Attachments.Add(new Attachment(attachmentPath));
                }
                smtp.Send(mail);
                return true;
            }
            catch (Exception e)
            {
                LoggerHelper.Error("send email error", e);
            }
            return false;
        }

        public static bool IsEmail(string emailAdd)
        {
            return Regex.IsMatch(emailAdd, @"^([\w-\.]+)@((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.)|(([\w-]+\.)+))([a-zA-Z]{2,4}|[0-9]{1,3})(\]?)$");
        }

        public void Dispose()
        {
            ((IDisposable)smtp).Dispose();
        }
    }
}