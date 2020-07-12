using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using static System.String;

namespace ServiceLayer
{
    public interface IDataProtectionService
    {
        string GetContactChangesForEmail(JObject contact);
    }

    public class DataProtectionService : IDataProtectionService
    {
        public string GetContactChangesForEmail(JObject contact)
        {
            var changes = RecursiveChangeChecker(null, contact);
            return Join("", changes);
        }

        private List<string> RecursiveChangeChecker(string objectKey, JObject @object)
        {
            var list = new List<string>();

            foreach (var keyValuePair in @object)
            {
                string key = keyValuePair.Key;
                JToken value = keyValuePair.Value;

                JObject recursiveCheck = value as JObject;
                if (recursiveCheck != null)
                {
                    list.AddRange(RecursiveChangeChecker(key, recursiveCheck));
                }
                else
                {
                    string listItem = "<li>";
                    if (objectKey != null)
                    {
                        listItem += $"{GetGermanTranslation(objectKey)} ";
                    }
                    listItem += @$"({GetGermanTranslation(@object.GetValue("type").ToString())}: ""{GetGermanTranslation(@object.GetValue("data").ToString())}"")</li>";

                    list.Add(listItem);
                    break;
                }
            }

            return list;
        }

        private string GetGermanTranslation(string englishKey)
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