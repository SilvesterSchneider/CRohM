using System;
using System.IO;
using iText.Kernel.Pdf;
using iText.Layout;
using iText.Layout.Element;
using ModelLayer.Models;

namespace ServiceLayer
{
    public class PdfGenerator
    {
        public static byte[] GenerateNewContactPdf(Contact contact, String contactDetails)
        {
            var stream = new MemoryStream();
            var writer = new PdfWriter(stream);
            writer.SetCloseStream(false); // to prevent exception
            var pdf = new PdfDocument(writer);
            var doc = new Document(pdf);
            doc.SetMargins(25,25,25,25);
            doc.Add(new Paragraph("Auskunft über gespeicherte Daten").SetFontSize(14).SetBold());

            doc.Add(new Paragraph("Auskunft gem. Art. 15 EU-DSGVO"));

            doc.Add(new Paragraph("Stand " + DateTime.Today.Date.Day + "." + DateTime.Today.Date.Month + "." + DateTime.Today.Date.Year));
            string gender = "Herrn ";
            if (contact.Gender == Contact.GenderTypes.FEMALE)
            {
                gender = "Frau ";
            }
            else if (contact.Gender == Contact.GenderTypes.DIVERS)
            {
                gender = "Divers ";
            }
            doc.Add(new Paragraph(" Folgende personenbezogene Daten sind zu " + gender + contact.Name +
                                  " im Customer Relationship Management System(CRMS) der TH Nürnberg gespeichert. Die Zustimmung zur Speicherung und Verarbeitung der Daten erfolgte am "
                                  + DateTime.Today.Date.Day + "." + DateTime.Today.Date.Month + "." + DateTime.Today.Date.Year));
            doc.Add(new Paragraph(contactDetails));
            doc.Close();
            return stream.ToArray();
        }
    }
}
