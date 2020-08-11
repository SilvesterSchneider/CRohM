using System;
using System.IO;
using iText.IO.Source;
using iText.Kernel.Geom;
using iText.Kernel.Pdf;
using iText.Layout;
using iText.Layout.Element;
using java.io;
using ModelLayer.Models;

namespace ServiceLayer
{
    public class PdfGenerator
    {
        public static Stream GenerateNewContactPdf(Contact contact)
        {
            var stream = new MemoryStream();
            var writer = new PdfWriter(stream);
            writer.SetCloseStream(false); // to prevent exception
            var pdf = new PdfDocument(writer);
            var doc = new Document(pdf);
            // doc.SetMargins(25,25,25,25);
            doc.Add(new Paragraph("Auskunft über gespeicherte Daten"));

            doc.Add(new Paragraph("Auskunft gem. Art. 15 EU-DSGVO"));

            doc.Add(new Paragraph("Stand " + DateTime.Today.Date));

            doc.Add(new Paragraph(" Folgende personenbezogene Daten sind zu Herrn/Frau " + contact.Name +
                                  " im Customer Relationship Management System(CRMS) der TH Nürnberg gespeichert.\n" + "Die Zustimmung zur Speicherung und Verarbeitung der Daten erfolgte am "
                                  + DateTime.Today.Date));

            var contactDetails = new string("");
            contactDetails += "Vorname:\t" + contact.PreName + "\n";
            contactDetails += "Name:\t" + contact.Name + "\n";

            contactDetails += "Mail:\t" + contact.ContactPossibilities.Mail + "\n";
            contactDetails += "Fax:\t" + contact.ContactPossibilities.Fax + "\n";
            contactDetails += "PhoneNumber:\t" + contact.ContactPossibilities.PhoneNumber + "\n";
            contactDetails += "Description:\t" + contact.ContactPossibilities.Description + "\n";

            // TODO add missing details

            doc.Add(new Paragraph(contactDetails));
            doc.Close();
            return stream;
        }
    }
}
