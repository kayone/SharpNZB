using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Text;
using NLog;
using SharpNzb.Core.Model.Nzb;

namespace SharpNzb.Core.Providers
{
    public class EmailProvider : IEmailProvider
    {
        private IConfigProvider _config;
        private IDiskProvider _disk;

        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        public EmailProvider(IConfigProvider config, IDiskProvider disk)
        {
            _config = config;
            _disk = disk;
        }

        #region IEmailProvider Members

        public bool Send(NzbModel nzb)
        {
            //Get the Template
            //Get SMTP Server Information, To/From Address, etc

            var toAddress = _config.GetValue("EmailToAddress", String.Empty, false);
            var fromAddress = _config.GetValue("EmailFromAddress", String.Empty, false);
            var host = _config.GetValue("SmtpServerHost", String.Empty, false);
            int port = Convert.ToInt32(_config.GetValue("SmtpServerPort", "25", true));
            bool ssl = Convert.ToBoolean(_config.GetValue("SmtpServerSsl", "0", true));
            bool authentication = Convert.ToBoolean(_config.GetValue("SmtpServerAthentication", "0", true));
            var username = _config.GetValue("SmtpServerUsername", String.Empty, false);
            var password = _config.GetValue("SmtpServerPassword", String.Empty, false);
            
            MailMessage email = new MailMessage(); //Create a new MailMessage named email

            email.To.Add(toAddress);
            email.From = new MailAddress(fromAddress); //Create a new MailAddress for the senders address;

            //Todo: Need to Build these from a template
            email.Subject = String.Empty; //Set the subject of the email
            email.Body = String.Empty; //set the body of the email

            SmtpClient client = new SmtpClient(); //Create a new SMTP client
            client.Host = host; //Set the host for the client
            client.Port = port; //Set the port for the client

            try
            {
                client.Send(email); //Try to send the message
            }

                        catch (Exception ex)
            {
                Logger.DebugException(ex.Message, ex);
                return false;
            }

            //return true;
            throw new NotImplementedException("Email Provider - Send");
        }

        #endregion

        private string BuildSubject()
        {

            throw new NotImplementedException("Email Provider - BuildSubject");
        }

        private string BuildBody()
        {

            throw new NotImplementedException("Email Provider - BuildBody");
        }
    }
}