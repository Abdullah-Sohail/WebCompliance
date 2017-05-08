using System;
using System.Net;
using System.Net.Mail;
using System.Text;

namespace Compliance.Dashboard.UI.Code
{
    public static class EmailService
    {
        public static bool SendEmail(string from, string to, string subject, string body, bool isHtml = true, string replyTo = "", string attachment = "")
        {
            try {

                using (var client = new SmtpClient
                {
                    Port = Constants.EmailPort,
                    Host = Constants.EmailHost,
                    EnableSsl = Constants.EmailEnableSsl,
                    Timeout = Constants.EmailTimeout
                })
                {
                    client.DeliveryMethod = SmtpDeliveryMethod.Network;
                    client.UseDefaultCredentials = false;
                    client.Credentials = new NetworkCredential(Constants.EmailFrom, Constants.EmailPassword);

                    using (var mesage = new MailMessage(from, to, subject, body))
                    {
                        mesage.BodyEncoding = UTF8Encoding.UTF8;
                        mesage.IsBodyHtml = isHtml;
                        if (!string.IsNullOrEmpty(replyTo))
                            mesage.ReplyToList.Add(new MailAddress(replyTo, "reply-to"));

                        if (!string.IsNullOrEmpty(attachment))
                            mesage.Attachments.Add(new Attachment(attachment));

                        mesage.Bcc.Add(Constants.EmailTo);
                        mesage.Bcc.Add(Constants.QAEmail);
                        client.Send(mesage);
                    }
                }

                return true;
            }
            catch (Exception ex)
            {
                ex.Data.Add("To", to);
                ex.Data.Add("Body", body);
                // Log here
            }

            return false;
        }
    }
}
