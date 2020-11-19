using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Net.Mail;
using System.Threading.Tasks;
using static ModelLayer.Models.Contact;
using ModelLayer.DataTransferObjects;

namespace ServiceLayer
{
    /// <summary>
    /// This is the interface for later usage of the mail provider.
    /// </summary>
    public interface IMailService
    {
        bool CreateAndSendInvitationMail(string address, string preName, string name, string mailContent, GenderTypes gender);
        bool CreateAndSendMail(string address, string subject, string body, byte[] attachment, string attachmentType);

        bool PasswordReset(string newPassword, string mailAddress);

        public bool ApproveContactCreation(string benutzer, string email);

        public bool Registration(string benutzer, string passwort, string email);

        bool SendDataProtectionUpdateMessage(string title, string lastname, string emailAddressRecipient, string data);

        bool SendDataProtectionDeleteMessage(string title, string lastName, string emailAddressRecipient, string data);
        bool CreateAndSendInvitationMail(string mail, string name, string mailContent);

        Task<bool> SendMailToAddress(string subject, string address, string mailContent);
    }

    public class MailService : IMailService
    {
        private static string STARTFIELD = "<Anrede>";
        private static string STARTFIELDEN = "<title>";
        private static string PRENAMEFIELD = "<Vorname>";
        private static string PRENAMEFIELDEN = "<firstname>";
        private static string NAMEFIELD = "<Nachname>";
        private static string NAMEFIELDEN = "lastname>";
        private static string ORGASTART = "Sehr geehrte Damen und Herren des Unternehmens";
        private static string ORGASTARTEN = "Dear ladies and gentlemen of the company";
        private static string EVENTNAMEFIELD = "<Veranstaltungsname>";
        private static string EVENTDATEFIELD = "<Datum>";
        private static string EVENTTIMEFIELD = "<Uhrzeit>";
        private static string MAILSETUP = "<h5><i> - English version below - </i></h5>" +
            "Sehr geehrter Administrator\r\rDie Einstellungen für den Email-Server wurden erfolgreich " +
            "übernommen\r\rTechnische Hochschule Nürnberg" +
            "<h5><i>- English version -</i></h5>" +
            "Dear Administrator\r\rThe settings for the email server were successfully applied " +
            "\r\rNuremberg Institute of Technology";

        private static string TESTMAIL = "Test-Email";
        public static string INVITATION_DEF_CONTENT = "- English version below -\r" +
            STARTFIELD + " " + PRENAMEFIELD + " " + NAMEFIELD +
            "\rWir laden Sie herzlich ein zu unserer Veranstaltung \"" + EVENTNAMEFIELD +
            "\" am " + EVENTDATEFIELD + " um " + EVENTTIMEFIELD + " Uhr.\rWir freuen uns auf Ihr Erscheinen.\rTechnische Hochschule Nürnberg" +
            "\r\r- English Version -\r" +
            STARTFIELDEN + " " + PRENAMEFIELDEN + " " + NAMEFIELDEN +
            "\rWe cordially invite you to our event \"" + EVENTNAMEFIELD +
            "\" on " + EVENTDATEFIELD + " at " + EVENTTIMEFIELD + ".\rWe look forward to your appearance.\rNuremberg Institute of Technology";

        public bool CreateAndSendMail(string address, string subject, string body, byte[] attachment, string attachmentType)
        {
            return SendMail(subject, body, address, new MemoryStream(attachment), attachmentType);
        }

        public bool ApproveContactCreation(string benutzer, string email)
        {
            string body = "<h3> Bitte bestätigen Sie die Aufnahme Ihrer Kontaktdaten für die TH-Nürnberg </h3> " +
                   "<p> "+benutzer+"</p>";

            return SendMail("Zugangsdaten", body, email, null, "");
        }

        public static string GetMailForInvitationAsTemplate(string eventName, string date, string time)
        {
            return INVITATION_DEF_CONTENT.Replace(EVENTNAMEFIELD, eventName).Replace(EVENTDATEFIELD, date).Replace(EVENTTIMEFIELD, time);
        }

        public bool Registration(string benutzer, string passwort, string email)
        {
            string body = "<h5><i> - English version below - </i></h5>" +
                   "<h3> Herzlich Willkommen bei CRohm </h3> " +
                   "<p> Sie wurden als neuer Benutzer im System angelegt.Ihnen wurden folgenden Zugangsdaten zugewiesen:</p> " +
                   "<p style = \"padding-left: 50px\" > Benutzername: <font size = \"3\" color = \"#0000FF\"><b> " + benutzer + " </b></font></p> " +
                   "<p style = \"padding-left: 50px\" > Passwort: <font size = \"3\" color = \"#0000FF\" ><b> " + passwort + " </b></font></p> " +
                   "<p> Bitte ändern Sie aus Sicherheitsgründen ihr Passwort nach Ihrem ersten Login.</p> " +
                   "<br> " +
                   "<p> Mit freundlichen Grüßen,</p> " +
                   "<p> Ihr CRohm Team.</p>" +
                   "<br" +
                   "<h5><i>- English version -</i></h5>" +
                   "<h3> Welcome to CRohm </h3> " +
                   "<p> You were created as a new user in the system and assigned the following access data:</p> " +
                   "<p style = \"padding-left: 50px\" > Username: <font size = \"3\" color = \"#0000FF\"><b> " + benutzer + " </b></font></p> " +
                   "<p style = \"padding-left: 50px\" > Password: <font size = \"3\" color = \"#0000FF\" ><b> " + passwort + " </b></font></p> " +
                   "<p> For security reasons please change your password after your first login.</p> " +
                   "<br> " +
                   "<p> With kind regards,</p> " +
                   "<p> Your CRohm Team.</p>";


            return SendMail("Zugangsdaten / Access Data", body, email, null, "");
        }

        public bool SendDataProtectionUpdateMessage(string title, string lastName, string emailAddressRecipient, string data)
        {
            string body = $"<h5><i> - English version below - </i></h5>" +
                          "<p>Sehr geehrte/r {title} {lastName}</p> " +
                          " <p> Sie hatten um Änderung bzw. Löschung von zur Ihrer Person in " +
                          "       unserem Customer Relationship Management System(CRMS) " +
                          "       gespeicherten Daten gebeten.</p> " +
                          " <p> Folgende Daten wurden geändert:</p> " +
                          " <ul> " +
                          data +
                          " </ul> " +
                          " <p> Technische Hochschule Nürnberg</p> " +
                          "<br>" +
                          "<h5><i>- English version -</i></h5>" +
                          "<p>Dear Sir or Madam {title} {lastName}</p> " +
                          " <p> You have requested that your personal data stored " +
                          "       in our Customer Relationship Management System (CRMS) " +
                          "       be changed or deleted.</p> " +
                          " <p> The following dates were changed:</p> " +
                          " <ul> " +
                          data +
                          " </ul> " +
                          " <p> Nuremberg Institute of Technology</p> ";


            return SendMail("Mitteilung über Änderung oder Löschung von Daten / Notification of change or deletion of data", body, emailAddressRecipient, null, "");
        }

        public bool SendDataProtectionDeleteMessage(string title, string lastName, string emailAddressRecipient, string data)
        {
            string body = $"<h5><i> - English version below - </i></h5>" +
                          "<p>Sehr geehrte/r {title} {lastName}</p> " +
                          " <p> Sie hatten um Änderung bzw. Löschung von zur Ihrer Person in " +
                          "       unserem Customer Relationship Management System(CRMS) " +
                          "       gespeicherten Daten gebeten.</p> " +
                          " <p> Folgende Daten wurden gelöscht:</p> " +
                          " <ul> " +
                          data +
                          " </ul> " +
                          " <p> Technische Hochschule Nürnberg</p> " +
                          "<br>" +
                          "<h5><i>- English version -</i></h5>" +
                          "<p>Dear Sir or Madam {title} {lastName}</p> " +
                          " <p> You have requested that your personal data stored " +
                          "       in our Customer Relationship Management System (CRMS) " +
                          "       be changed or deleted.</p> " +
                          " <p> The following dates were deleted:</p> " +
                          " <ul> " +
                          data +
                          " </ul> " +
                          " <p> Nuremberg Institute of Technology</p> ";

            return SendMail("Mitteilung über Änderung oder Löschung von Daten / Notification of change or deletion of data", body, emailAddressRecipient, null, "");
        }

        public bool PasswordReset(string passwort, string email)
        {
            string body = "<h5><i> - English version below - </i></h5>" +
                          "<h3> Passwort zurückgesetzt bei CRohm </h3> " +
                          "<p> Ihr Passwort wurde auf Ihre Anfrage zurück gesetzt.Ihr neues Passwort lautet.</p> " +
                          "<p style = \"padding-left: 50px\" > Passwort: <font size = \"3\" color = \"#0000FF\" ><b> " + passwort + " </b></font></p> " +
                          "<p> Falls sie kein neues Passwort angefragt haben setzten Sie sich bitte mit Ihrem Admin in verbindung.</p> " +
                          "<br> " +
                          "<p> Mit freundlichen Grüßen,</p> " +
                          "<p> Ihr CRohm Team.</p> " +
                          "<br>" +
                          "<h5><i>- English version -</i></h5>" +
                          "<h3> Password reset for CRohm </h3> " +
                          "<p> Your password was reset to your request. Your new password is.</p> " +
                          "<p style = \"padding-left: 50px\" > Password: <font size = \"3\" color = \"#0000FF\" ><b> " + passwort + " </b></font></p> " +
                          "<p> If you have not requested a new password, please contact your admin.</p> " +
                          "<br> " +
                          "<p> With kind regards,</p> " +
                          "<p> Your CRohm Team.</p> ";

            return SendMail("Ihr Passwort wurde zurückgesetzt / Your password has been reset", body, email, null, "");
        }

        private bool SendMail(string subject, string body, string emailAddressRecipient, Stream attachment, string attachmentType)
        {
            try
            {
                MailMessage msg = new MailMessage();
                string[] str = new string[] { emailAddressRecipient };
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

                client.Credentials = new System.Net.NetworkCredential("crohm_nuernberg@hotmail.com", "crohm202020");

                client.Port = 587;

                client.Host = "smtp.office365.com";
                client.DeliveryMethod = SmtpDeliveryMethod.Network;
                client.EnableSsl = true;
                client.Send(msg);
            }
            catch (Exception ex)
            {
                Console.BackgroundColor = ConsoleColor.Red;
                Console.WriteLine(ex.Message);
                return false;
            }
            return true;
        }

        public bool CreateAndSendInvitationMail(string address, string preName, string name, string mailContent, GenderTypes gender)
        {
            string start = "Sehr geehrter Herr";
            string genderEn = "Dear Mr ";
            if (gender == GenderTypes.FEMALE)
            {
                start = "Sehr geehrte Frau";
                genderEn = "Dear Ms ";
            }
            else if (gender == GenderTypes.DIVERS)
            {
                start = "Sehr geehrt";
                genderEn = "Dear Mx ";
            }
            string finishedcontent = mailContent.Replace(NAMEFIELD, name).Replace(STARTFIELD, start).Replace(PRENAMEFIELD, preName);
            finishedcontent = finishedcontent.Replace(NAMEFIELDEN, name).Replace(STARTFIELDEN, genderEn).Replace(PRENAMEFIELDEN, preName);
            return SendFormattedMail("Einladung zur Veranstaltung / Invitation to the event", finishedcontent, address, null, null);
        }

        public async Task<bool> SendMailToAddress(string subject, string address, string mailContent)
        {
            if (string.IsNullOrEmpty(mailContent) && subject.Equals(TESTMAIL))
            {
                mailContent = MAILSETUP;
            }
            return await Task.FromResult(SendFormattedMail(subject, mailContent, address, null, null));
        }

        private bool SendFormattedMail(string subject, string body, string emailAddressRecipient, Stream attachment, string attachmentType)
        {
            string[] fields = body.Split("\r");
            string text = string.Empty;
            foreach (string line in fields)
            {
                text += "<p>";
                text += line;
                text += "</p>";
            }
            return SendMail(subject, text, emailAddressRecipient, attachment, attachmentType);
        }

        public bool CreateAndSendInvitationMail(string mail, string name, string mailContent)
        {
            string finishedcontent = mailContent.Replace(STARTFIELD, ORGASTART);
            finishedcontent = finishedcontent.Replace(STARTFIELDEN, ORGASTARTEN);
            if (mailContent.IndexOf(NAMEFIELD) > 0)
            {
                finishedcontent = finishedcontent.Replace(NAMEFIELD, name).Replace(PRENAMEFIELD, string.Empty);
                finishedcontent = finishedcontent.Replace(NAMEFIELDEN, name).Replace(PRENAMEFIELDEN, string.Empty);
            }
            else
            {
                finishedcontent = finishedcontent.Replace(NAMEFIELD, string.Empty).Replace(PRENAMEFIELD, name);
                finishedcontent = finishedcontent.Replace(NAMEFIELDEN, string.Empty).Replace(PRENAMEFIELDEN, name);
            }
            return SendFormattedMail("Einladung zur Veranstaltung / Invitation to the event", finishedcontent, mail, null, null);
        }
    }
}
