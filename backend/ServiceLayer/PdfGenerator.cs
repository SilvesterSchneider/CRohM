using System;
using System.CodeDom.Compiler;
using System.IO;
using iText.Kernel.Pdf;
using iText.Layout;
using iText.Layout.Element;
using ModelLayer.Models;

namespace ServiceLayer
{
    public class PdfGenerator
    {
        public static Stream generateNewContactPdf(Contact contact)
        {
            MemoryStream memoryStream = new MemoryStream();
            PdfWriter writer = new PdfWriter(memoryStream);
            PdfDocument pdf = new PdfDocument(writer);

            var doc = new Document(pdf);

            doc.Add(new Paragraph("Auskunft über gespeicherte Daten"));

            doc.Add(new Paragraph("Auskunft gem. Art. 15 EU-DSGVO"));

            doc.Add(new Paragraph("Stand " + DateTime.Today.Date));

            doc.Add(new Paragraph(" Folgende personenbezogene Daten sind zu Herrn/Frau " + contact.Name +
                                  " im Customer Relationship Management System(CRMS) der TH Nürnberg gespeichert.\n" + "Die Zustimmung zur Speicherung und Verarbeitung der Daten erfolgte am "
                                  + DateTime.Today.Date));
            var contactDetails = new string("");
            contactDetails += "Vorname:\t" + contact.PreName + "\n";
            contactDetails += "Name:\t" + contact.Name + "\n";
            doc.Add(new Paragraph(contactDetails));
                    // TODO add missing details
            doc.Close();
            writer.Close();
            memoryStream.Position = 0;
            return memoryStream;
        }
    }
}
