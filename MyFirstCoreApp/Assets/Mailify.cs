using System;
using System.Net;
using System.Net.Mail;
using System.Collections.Generic;
using System.Linq;
using MailMessage = System.Net.Mail.MailMessage;

namespace MyFirstCoreApp
{
    class Mailify
    {
        public static string sendFrom;
        public static string accountUser;
        public static string accountPassword;
        public static string relayServer;
        public static int serverPort;
        public static int serverTimeout;
        public static Boolean sendEmailAsync;
        public Mailify(string relay, string user, string password, string from, int port = 25, int timeout = 30000, Boolean sendAsync = true)
        {
            sendFrom = from;
            relayServer = relay;
            accountUser = user;
            accountPassword = password;
            serverPort = port;
            serverTimeout = timeout;
            sendEmailAsync = sendAsync;
        }
        public string send(string sendTo, string subject, string body)
        {
            List<string> recipients = sendTo.Split(',').ToList();
            /* INIT */
            SmtpClient SMTPClient = new SmtpClient();
            MailMessage Message = new MailMessage();
            MailAddress sendFromAddress = new MailAddress(sendFrom);

            /* Setup Message */
            Message.From = sendFromAddress;
            Message.Subject = subject;
            Message.IsBodyHtml = true;
            Message.Body = body;

            /* Add recipients (comma separated) */
            foreach (string rcp in recipients)
            {
                Message.To.Add(rcp);
            }


            /* Setup Client */
            SMTPClient.Host = relayServer;
            SMTPClient.Port = serverPort;
            SMTPClient.DeliveryMethod = SmtpDeliveryMethod.Network;
            SMTPClient.UseDefaultCredentials = true;
            SMTPClient.Credentials = new NetworkCredential(accountUser,accountPassword);
            SMTPClient.Timeout = serverTimeout;


            /* Send */
            try
            {
                if (!sendEmailAsync)
                {
                    SMTPClient.Send(Message);
                    return "";
                }
                else
                {
                    SMTPClient.SendCompleted += (s, e) => {
                        SMTPClient.Dispose();
                        SMTPClient.Dispose();
                    };
                    SMTPClient.SendAsync(Message, null);
                    return "";
                }
                
            }
            catch (Exception err)
            {
                return err.Message.ToString();
            }

        }
    }
}
