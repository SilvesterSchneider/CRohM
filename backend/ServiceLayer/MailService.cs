using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Net.Mail;
using System.Threading.Tasks;
using static ModelLayer.Models.Contact;
using ModelLayer.DataTransferObjects;
using ModelLayer.Models;
using System.Runtime.CompilerServices;
using ModelLayer.Helper;

namespace ServiceLayer
{
    /// <summary>
    /// This is the interface for later usage of the mail provider.
    /// </summary>
    /// /// <summary>
    /// RAM: 25%
    /// </summary>
    public interface IMailService
    {
        Task<bool> CreateAndSendInvitationMail(string address, string preName, string name, string mailContent, GenderTypes gender, long eventid, long contactid, long organisationid, string uri);
        Task<bool> CreateAndSendMail(string address, string subject, string body, byte[] attachment, string attachmentType);

        Task<bool> PasswordReset(string newPassword, string mailAddress);

        Task<bool> ApproveContactCreation(string benutzer, string email);

        Task<bool> Registration(string benutzer, string passwort, string email);

        Task<bool> SendDataProtectionUpdateMessage(string title, string lastname, string emailAddressRecipient, string data);

        Task<bool> SendDataProtectionDeleteMessage(string title, string lastName, string emailAddressRecipient, string data);
        Task<bool> CreateAndSendInvitationMail(string mail, string name, string mailContent, long eventid, long contactid, long organisationid, string uri);

        Task<bool> SendMailToAddress(string subject, string address, string mailContent, string preName, string name);

        Task<bool> SendEventDeletedMessage(List<EventContact> contactMails, List<EventOrganization> orgas);
    }

    /// <summary>
    /// RAM: 30%
    /// </summary>
    public class MailService : IMailService
    {
        private const bool USE_TESTMODE = false;
        private const string BASE_PATH_FOR_TESTMAIL_CREATION = "C:\\Mails";
        private static string STARTFIELD = "<Anrede>";
        private static string STARTFIELDEN = "<title>";
        private static string PRENAMEFIELD = "<Vorname>";
        private static string PRENAMEFIELDEN = "<firstname>";
        private static string NAMEFIELD = "<Nachname>";
        private static string NAMEFIELDEN = "<lastname>";
        private static string ORGASTART = "Sehr geehrte Damen und Herren des Unternehmens";
        private static string ORGASTARTEN = "Dear ladies and gentlemen of the company";
        private static string EVENTNAMEFIELD = "<Veranstaltungsname>";
        private static string EVENTDATEFIELD = "<Datum>";
        private static string EVENTTIMEFIELD = "<Uhrzeit>";
        private static string EVENTBUTTONFIELD_DE = "<BUTTON_DE>";
        private static string EVENTBUTTONFIELD_EN = "<BUTTON_EN>";
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
            "\" am " + EVENTDATEFIELD + " um " + EVENTTIMEFIELD + " Uhr.\r" +
            EVENTBUTTONFIELD_DE + "\r" +
            "Wir freuen uns auf Ihr Erscheinen.\rTechnische Hochschule Nürnberg" +
            "\r\r- English Version -\r" +
            STARTFIELDEN + " " + PRENAMEFIELDEN + " " + NAMEFIELDEN +
            "\rWe cordially invite you to our event \"" + EVENTNAMEFIELD +
            "\" on " + EVENTDATEFIELD + " at " + EVENTTIMEFIELD + ".\r" +
            EVENTBUTTONFIELD_EN + " \r" +
            "We look forward to your appearance.\rNuremberg Institute of Technology";
        private static string EVENT_CANCELATION_CONTENT = "<p>" + STARTFIELD + NAMEFIELD + ", </p>" +
            "<p> leider musste die Veranstaltung '" + EVENTNAMEFIELD + "', </p>" +
            "<p> welche am " + EVENTDATEFIELD + " um " + EVENTTIMEFIELD + " Uhr stattfinden sollte, </p>" +
            "<p> abgesagt werden.</p>";
        private static string EVENT_CANCELATION_SUBJECT = "Absage einer Veranstaltung der technischen Hochschule";

        public async Task<bool> CreateAndSendMail(string address, string subject, string body, byte[] attachment, string attachmentType)
        {
            return await SendMailAsync(subject, body, address, new MemoryStream(attachment), attachmentType);
        }

        public async Task<bool> ApproveContactCreation(string benutzer, string email)
        {
            string body = "<h3> Bitte bestätigen Sie die Aufnahme Ihrer Kontaktdaten für die TH-Nürnberg </h3> " +
                   "<p> "+benutzer+"</p>";

            return await SendMailAsync("Aufnahmebestätigung ins CRMS System der TH Nürnberg / Please confirm the inclusion of your contact details for TH Nuremberg", body, email, null, "");
        }

        public static string GetMailForInvitationAsTemplate(string eventName, string date, string time)
        {
            return INVITATION_DEF_CONTENT.Replace(EVENTNAMEFIELD, eventName).Replace(EVENTDATEFIELD, DateTime.Parse(date).ToString("dd.MM.yyyy")).Replace(EVENTTIMEFIELD, time);
        }

        public async Task<bool> Registration(string benutzer, string passwort, string email)
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


            return await SendMailAsync("Zugangsdaten / Access Data", body, email, null, "");
        }

        public async Task<bool> SendDataProtectionUpdateMessage(string title, string lastName, string emailAddressRecipient, string data)
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


            return await SendMailAsync("Mitteilung über Änderung oder Löschung von Daten / Notification of change or deletion of data", body, emailAddressRecipient, null, "");
        }

        public async Task<bool> SendDataProtectionDeleteMessage(string title, string lastName, string emailAddressRecipient, string data)
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

            return await SendMailAsync("Mitteilung über Änderung oder Löschung von Daten / Notification of change or deletion of data", body, emailAddressRecipient, null, "");
        }

        public async Task<bool> PasswordReset(string passwort, string email)
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

            return await SendMailAsync("Ihr Passwort wurde zurückgesetzt / Your password has been reset", body, email, null, "");
        }

        private bool SendMail(string subject, string body, string emailAddressRecipient, Stream attachment, string attachmentType)
        {
            if (!USE_TESTMODE)
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
                    return true;
                }
                catch (Exception ex)
                {
                    Console.BackgroundColor = ConsoleColor.Red;
                    Console.WriteLine(ex.Message);
                    return false;
                }
            }
            else
            {
                return CreateMailFiles(body, emailAddressRecipient, attachment, attachmentType);
            }
        }

        private async Task<bool> SendMailAsync(string subject, string body, string emailAddressRecipient, Stream attachment, string attachmentType)
        {
            if (!USE_TESTMODE)
            {
                var mailCredentials = MailCredentialsHelper.GetMailCredentials();
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
                    msg.From = mailCredentials.MailAddress;
                    msg.Subject = subject;
                    msg.Body = body;

                    if (attachment != null)
                        msg.Attachments.Add(new Attachment(attachment, attachmentType));

                    SmtpClient client = new SmtpClient();
                    client.UseDefaultCredentials = false;

                    client.Credentials = mailCredentials.NetworkCredential;

                    client.Port = mailCredentials.Port;

                    client.Host = mailCredentials.Host;
                    client.DeliveryMethod = SmtpDeliveryMethod.Network;
                    client.EnableSsl = true;
                    await client.SendMailAsync(msg);
                    return true;
                }
                catch (Exception ex)
                {
                    Console.BackgroundColor = ConsoleColor.Red;
                    Console.WriteLine(ex.Message);
                    return false;
                }
            }
            else
            {
                return CreateMailFiles(body, emailAddressRecipient, attachment, attachmentType);
            }
        }

        private bool CreateMailFiles(string body, string emailAddressRecipient, Stream attachment, string attachmentType)
        {
            if (string.IsNullOrEmpty(emailAddressRecipient) || string.IsNullOrEmpty(body))
            {
                return false;
            }

            try
            {
                string mailPath = BASE_PATH_FOR_TESTMAIL_CREATION + "\\" + emailAddressRecipient + ".txt";
                if (!Directory.Exists(BASE_PATH_FOR_TESTMAIL_CREATION))
                {
                    Directory.CreateDirectory(BASE_PATH_FOR_TESTMAIL_CREATION);
                }

                if (!File.Exists(mailPath))
                {
                    File.Create(mailPath).Close();
                }

                if (attachment != null && attachmentType != null)
                {
                    body += "\r\n\r\n With Attachments!";
                }

                File.WriteAllText(mailPath, body);
                return true;
            }
            catch (Exception e)
            {
                return false;
            }
        }

        public async Task<bool> CreateAndSendInvitationMail(string address, string preName, string name, string mailContent, GenderTypes gender, long eventid, long contactid, long organisationid, string uri)
        {
            string start = GetGenderTitle(gender);
            string genderEn = GetGenderTitleEnglish(gender);
            string finishedcontent = mailContent.Replace(NAMEFIELD, name).Replace(NAMEFIELDEN, name).Replace(STARTFIELD, start).Replace(PRENAMEFIELD, preName).Replace(PRENAMEFIELDEN, preName).Replace(STARTFIELDEN, genderEn);

            //Send Mail to Approve
#if DEBUG
            string buttonDe = "<p><a href = \"https://localhost:4200/EventAnswer/" + eventid + "/" + contactid + "/" + organisationid + "/2\"><button> Zusagen </button></a></p>" +
             "<p><a href = \"https://localhost:4200/EventAnswer/" + eventid + "/" + contactid + "/" + organisationid + "/3\"><button> Absagen </button></a></p>";
            string buttonEn = "<p><a href = \"https://localhost:4200/EventAnswer/" + eventid + "/" + contactid + "/" + organisationid + "/2\"><button> Agree </button></a></p>" +
             "<p><a href = \"https://localhost:4200/EventAnswer/" + eventid + "/" + contactid + "/" + organisationid + "/3\"><button> Cancel </button></a></p>";
#else
            string buttonDe = "<p><a href = \"https://ops085010.cs.ohmhs.de/EventAnswer/"+ eventid+"/"+contactid+"/"+organisationid+ "/2\"><button> Zusagen </button></a></p>" +
                "<p><a href = \"https://ops085010.cs.ohmhs.de/EventAnswer/"+eventid+"/" + contactid+"/"+organisationid+"/3\"><button> Absagen </button></a></p>";
            string buttonEn = "<p><a href = \"https://ops085010.cs.ohmhs.de/EventAnswer/"+ eventid+"/"+contactid+"/"+organisationid+ "/2\"><button> Agree </button></a></p>" +
                "<p><a href = \"https://ops085010.cs.ohmhs.de/EventAnswer/"+eventid+"/" + contactid+"/"+organisationid+"/3\"><button> Cancel </button></a></p>";
#endif

            finishedcontent = finishedcontent.Replace(EVENTBUTTONFIELD_DE, buttonDe).Replace(EVENTBUTTONFIELD_EN, buttonEn);

            return await SendFormattedMail("Einladung zur Veranstaltung / Invitation to the event", finishedcontent, address, null, null);
        }

        public async Task<bool> SendMailToAddress(string subject, string address, string mailContent, string preName, string name)
        {
            if (string.IsNullOrEmpty(mailContent) && subject.Equals(TESTMAIL))
            {
                mailContent = MAILSETUP;
            }
            else
            {
                mailContent = mailContent.Replace(PRENAMEFIELD, preName).Replace(PRENAMEFIELDEN, preName).Replace(NAMEFIELD, name).Replace(NAMEFIELDEN, name);
            }
            return await SendFormattedMail(subject, mailContent, address, null, null);
        }

        private async Task<bool> SendFormattedMail(string subject, string body, string emailAddressRecipient, Stream attachment, string attachmentType)
        {
            string[] fields = body.Split("\r");
            string text = string.Empty;
            foreach (string line in fields)
            {
                text += "<p>";
                text += line;
                text += "</p>";
            }
            return await SendMailAsync(subject, text, emailAddressRecipient, attachment, attachmentType);
        }

        public async Task<bool> CreateAndSendInvitationMail(string mail, string name, string mailContent, long eventid, long contactid, long organisationid, string uri)
        {
            string finishedcontent = mailContent.Replace(STARTFIELD, ORGASTART);
            finishedcontent = finishedcontent.Replace(STARTFIELDEN, ORGASTARTEN);


#if DEBUG
            string buttonDe = "<p><a href = \"https://localhost:4200/EventAnswer/" + eventid + "/" + contactid + "/" + organisationid + "/2\"><button> Zusagen </button></a></p>" +
              "<p><a href = \"https://localhost:4200/EventAnswer/" + eventid + "/" + contactid + "/" + organisationid + "/3\"><button> Absagen </button></a></p>";
            string buttonEn = "<p><a href = \"https://localhost:4200/EventAnswer/" + eventid + "/" + contactid + "/" + organisationid + "/2\"><button> Agree </button></a></p>" +
              "<p><a href = \"https://localhost:4200/EventAnswer/" + eventid + "/" + contactid + "/" + organisationid + "/3\"><button> Cancel </button></a></p>";
#else
            string buttonDe = "<p><a href = \"https://ops085010.cs.ohmhs.de/EventAnswer/" + eventid + "/" + contactid + "/" + organisationid + "/2\"><button> Zusagen </button></a></p>" +
              "<p><a href = \"https://ops085010.cs.ohmhs.de/EventAnswer/" + eventid + "/" + contactid + "/" + organisationid + "/3\"><button> Absagen </button></a></p>";
            string buttonEn = "<p><a href = \"https://ops085010.cs.ohmhs.de/EventAnswer/" + eventid + "/" + contactid + "/" + organisationid + "/2\"><button> Agree </button></a></p>" +
              "<p><a href = \"https://ops085010.cs.ohmhs.de/EventAnswer/" + eventid + "/" + contactid + "/" + organisationid + "/3\"><button> Cancel </button></a></p>";
#endif



            finishedcontent = finishedcontent.Replace(EVENTBUTTONFIELD_DE, buttonDe).Replace(EVENTBUTTONFIELD_EN, buttonEn);

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
            return await SendFormattedMail("Einladung zur Veranstaltung / Invitation to the event", finishedcontent, mail, null, null);
        }

        public async Task<bool> SendEventDeletedMessage(List<EventContact> contactMails, List<EventOrganization> orgas)
        {
            bool ok = true;
            foreach (EventContact contact in contactMails)
            {
                if (!await SendEventCancelInfo(contact.Contact.ContactPossibilities.Mail, GetGenderTitle(contact.Contact.Gender), contact.Contact.Name, contact.Event.Name, contact.Event.Date, contact.Event.Starttime)) {
                    ok = false;
                }
            }

            foreach (EventOrganization orga in orgas)
            {
                if (!await SendEventCancelInfo(orga.Organization.Contact.Mail, GetGenderTitle(GenderTypes.MALE), orga.Organization.Name, orga.Event.Name, orga.Event.Date, orga.Event.Starttime))
                {
                    ok = false;
                }
            }

            return ok;
        }

        private string GetGenderTitle(GenderTypes gender)
        {
            string start = "Sehr geehrter Herr ";
            if (gender == GenderTypes.FEMALE)
            {
                start = "Sehr geehrte Frau ";
            }
            else if (gender == GenderTypes.DIVERS)
            {
                start = "Sehr geehrt ";
            }
            return start;
        }

        private string GetGenderTitleEnglish(GenderTypes gender)
        {
            string start = "Dear mister ";
            if (gender == GenderTypes.FEMALE)
            {
                start = "Dear misses ";
            }
            else if (gender == GenderTypes.DIVERS)
            {
                start = "Dear ";
            }
            return start;
        }

        private async Task<bool> SendEventCancelInfo(string mail, string beginning, string name, string eventName, DateTime date, DateTime time)
        {
            string content = EVENT_CANCELATION_CONTENT
                .Replace(STARTFIELD, beginning)
                .Replace(NAMEFIELD, name)
                .Replace(EVENTNAMEFIELD, eventName)
                .Replace(EVENTDATEFIELD, date.ToString("dd.MM.yyyy"))
                .Replace(EVENTTIMEFIELD, time.ToString("hh:mm"));
            return await SendMailAsync(EVENT_CANCELATION_SUBJECT, content, mail, null, null);
        }
    }
}
