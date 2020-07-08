using System.Collections.Generic;
using ModelLayer.DataTransferObjects;
using Newtonsoft.Json.Linq;

namespace ServiceLayer
{
    public interface IDataProtectionService
    {
        List<string> MakeSomething(JObject carlist, ContactDto car);
    }

    public class DataProtectionService : IDataProtectionService
    {
        public List<string> MakeSomething(JObject obj, ContactDto car)
        {
            return loop(null, obj);
        }

        private List<string> loop(string pre, JObject obj)
        {
            var list = new List<string>();
            // TODO: hier weitermachen
            // TODO rename this zeug

            foreach (var x in obj)
            {
                string name = x.Key;
                JToken value = x.Value;

                JObject test = value as JObject;
                if (test != null)
                {
                    list.AddRange(loop(name, test));
                }
                else
                {
                    string tuple = "<li>";
                    if (pre != null)
                    {
                        tuple += $"{GetGermanTranslation(pre)} ";
                    }
                    tuple += @$"({GetGermanTranslation(obj.GetValue("type").ToString())}: ""{GetGermanTranslation(obj.GetValue("data").ToString())}"")</li>";

                    list.Add(tuple);
                    break;
                }
            }

            return list;
        }

        public string GetGermanTranslation(string englishKey)
        {
            switch (englishKey)
            {
                case "created": return "neu";
                case "updated": return "aktualisiert";
                case "deleted": return "gelöscht";
                case "name": return "Name";
                case "description": return "Beschreibung";
                case "preName": return "Vorname";
                case "city": return "Stadt";
                case "street": return "Straße";
                case "streetNumber": return "Hausnummer";
                case "zipcode": return "Postleitzahl";
                case "country": return "Land";
                case "phoneNumber": return "Telefonnummer";
                case "fax": return "Faxnummer";
                case "mail": return "E-Mail";
                case "date": return "Datum";
                case "time": return "Zeitpunkt";
                case "duration": return "dauer";
                case "type": return "Typ";
                case "comment": return "Kommentar";
                case "contactEntryName": return "Name der Kontaktmöglichkeit";
                case "contactEntryValue": return "Inhalt der Kontaktmöglichkeit";
                case "id": return "Id";
                default: return englishKey;
            }
        }
    }
}