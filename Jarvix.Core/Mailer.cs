using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Mail;
using System.Net;
using System.Configuration;

namespace Ultrapulito.Jarvix.Core {

    public class Mailer {

        public static void SendMail(List<string> addresses, string subject, string body) {
            SmtpClient smtpServer = new SmtpClient(ConfigurationManager.AppSettings["SMTP_HOST"]);
            smtpServer.Port = Convert.ToInt32(ConfigurationManager.AppSettings["SMTP_PORT"]);
            smtpServer.Credentials = new NetworkCredential(ConfigurationManager.AppSettings["SMTP_USERNAME"], ConfigurationManager.AppSettings["SMTP_PASSWORD"]);
            smtpServer.EnableSsl = true;
            MailMessage mail = new MailMessage();
            mail.From = new MailAddress(ConfigurationManager.AppSettings["SMTP_MAIL_FROM"]);
            for (int i = 0; i < addresses.Count; i++) {
                mail.To.Add(addresses[i]);
            }
            mail.Subject = subject;
            mail.Body = body;
            mail.IsBodyHtml = true;
            smtpServer.Send(mail);
        }

    }
}
