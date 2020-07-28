using System;
using System.IO;
using System.Net.Mail;

namespace ServiceLayer
{
    /// <summary>
    /// This is the interface for later usage of the mail provider.
    /// </summary>
    public interface IMailProvider
    {
        public bool CreateAndSendMail(string address, string subject, string body, Stream attachment,
            string attachmentType);
        public bool PasswordReset(string newPassword, string mailAddress);
        public bool Registration(string user, string password, string email);
    }

    public class MailService : IMailProvider
    {
        public bool CreateAndSendMail(string address, string subject, string body, Stream attachment, string attachmentType)
        {
            return SendMail(subject, body, address, attachment, attachmentType);
        }

        // TODO refactor extract subject and body
        public bool Registration(string user, string password, string email)
        {
            string body = "<h3> Herzlich Willkommen bei CRohm </h3> " +
                   "<p> Sie wurden als neuer Benutzer im System angelegt.Ihnen wurden folgenden Zugangsdaten zugewiesen:</p> " +
                   "<p style = \"padding-left: 50px\" > Benutzername: <font size = \"3\" color = \"#0000FF\"><b> " + user + " </b></font></p> " +
                   "<p style = \"padding-left: 50px\" > Passwort: <font size = \"3\" color = \"#0000FF\" ><b> " + password + " </b></font></p> " +
                   "<p> Bitte ändern Sie aus Sicherheitsgründen ihr Passwort nach Ihrem ersten Login.</p> " +
                   "<br> " +
                   "<p> Mit freundlichen Grüßen,</p> " +
                   "<p> Ihr CRohm Team.</p>";

            return SendMail("Zugangsdaten", body, email,null, "");
        }

        public bool PasswordReset(string password, string email)
        {
            string body = "<h3> Passwort zurückgesetzt bei CRohm </h3> " +
                          "<p> Ihr Passwort wurde auf Ihre Anfrage zurück gesetzt.Ihr neues Passwort lautet.</p> " +
                          "<p style = \"padding-left: 50px\" > Passwort: <font size = \"3\" color = \"#0000FF\" ><b> " + password + " </b></font></p> " +
                          "<p> Falls sie kein neues Passwort angefragt haben setzten Sie sich bitte mit Ihrem Admin in verbindung.</p> " +
                          "<br> " +
                          "<p> Mit freundlichen Grüßen,</p> " +
                          "<p> Ihr CRohm Team.</p> ";

            return SendMail("Ihr Passwort wurde zurückgesetzt.", body, email, null, "");

        }

        private bool SendMail(string subject, string body, string mail, Stream attachment, string attachmentType)
        {
            try
            {
                MailMessage msg = new MailMessage();
                string[] str = new string[] { mail };
                msg.IsBodyHtml = true;
                //Empfänger hinzufügen
                foreach (string empf in str)
                {
                    msg.To.Add(new MailAddress(empf));
                }
                msg.From = new MailAddress("crohm_nuernberg@hotmail.com", "CRMS-Team");
                msg.Subject = subject;
                msg.Body = body;

                if (attachment != null)
                    msg.Attachments.Add(new Attachment(attachment, attachmentType));

                SmtpClient client = new SmtpClient();
                client.UseDefaultCredentials = false;

                client.Credentials = new System.Net.NetworkCredential("crohm_nuernberg@hotmail.com", "crohm2020");

                client.Port = 587;

                client.Host = "smtp.office365.com";
                client.DeliveryMethod = SmtpDeliveryMethod.Network;
                client.EnableSsl = true;
                client.Send(msg);
            }
            catch (Exception)
            {
                return false;
            }
            return true;
        }

        /*
         *Falls wir einmal eine Signatur einbauen möchten... wird in der Regel iwo als .htm abgelgt
         *Ich ändere in der Firma oft meine Signatur z.B. werbung für Messenauftritte, besondere aktionen etc.
        private string ReadSignature()
        {
            string signature = string.Empty;

            if (File.Exists("htmlDatei"))
            {
                FileInfo fiSignature = new FileInfo("htmlDatei");
                StreamReader sr = new StreamReader(fiSignature.FullName, Encoding.Default);
                signature = sr.ReadToEnd();

                if (!string.IsNullOrEmpty(signature))
                {
                    string fileName = fiSignature.Name.Replace(fiSignature.Extension, string.Empty);
                }
            }
            return signature;
        }
        */
    }
}
