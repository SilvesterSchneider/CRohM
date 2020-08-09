using System;
using System.Collections.Generic;
using System.Reflection;
using ModelLayer.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using static System.String;

namespace ServiceLayer
{
    public interface IDataProtectionService
    {
        /// <summary>
        /// Method to call from controller to get changes for email
        /// </summary>
        /// <param name="contact">contact as JObject</param>
        public string GetContactChangesForEmail(JObject contact);

        /// <summary>
        /// Returns all keys of contact object for email
        /// </summary>
        public string GetContactDeletesForEmail();
    }

    /// <summary>
    /// Contains methods for dsgvo
    /// </summary>
    public class DataProtectionService : IDataProtectionService
    {
        public string GetContactChangesForEmail(JObject contact)
        {
            var changes = RecursiveChangeChecker(null, contact);
            return Join("", changes);
        }

        public string GetContactDeletesForEmail()
        {
            var list = new List<string>();
            PropertyInfo[] properties = typeof(Contact).GetProperties();
            foreach (var property in properties)
            {
                if (property.Name.ToLower() == "id")
                    continue;

                list.Add(@$"<li>{GetGermanTranslation(property.Name)}</li>");
            }

            return Join("", list);
        }

        /// <summary>
        /// checks recursive the JObject if anything has changed or is tagged as changed
        /// </summary>
        /// <param name="objectKey">key of object</param>
        /// <param name="object">value of object</param>
        private List<string> RecursiveChangeChecker(string objectKey, JObject @object)
        {
            var list = new List<string>();

            foreach (var keyValuePair in @object)
            {
                string key;

                //checks if it is array with keys from 0 to ...
                var successfullyParsed = int.TryParse(keyValuePair.Key, out _);
                if (!successfullyParsed)
                {
                    key = keyValuePair.Key;
                }
                else
                {
                    key = objectKey;
                }

                JToken value = keyValuePair.Value;

                //if data value is json object go recursive
                JObject recursiveCheckObj = value as JObject;
                JArray recursiveCheckArr = value as JArray;
                if (recursiveCheckObj != null)
                {
                    list.AddRange(RecursiveChangeChecker(key, recursiveCheckObj));
                }
                else if (recursiveCheckArr != null)
                {
                    foreach (var VARIABLE in recursiveCheckArr)
                    {
                        list.AddRange(RecursiveChangeChecker(key, VARIABLE as JObject));
                    }
                }
                else
                {
                    //create list item in html for email
                    string listItem = "<li>";
                    if (objectKey == "id")
                        continue;
                    if (value.Type == JTokenType.Null)
                        continue;
                    if (@object.GetValue("data").Type == JTokenType.Null)
                        continue;
                    if (objectKey != null)
                    {
                        listItem += $"{GetGermanTranslation(objectKey)} ";
                    }

                    //check if object is contactPossibilities

                    if (@object.GetValue("data") is JObject temp)
                    {
                        string tempstring = "";
                        foreach (var keyValuePair2 in temp)
                        {
                            //contactEntries are special object hierarchy
                            if (keyValuePair2.Value is JArray cps)
                            {
                                foreach (var cp in cps)
                                {
                                    if (cp is JObject tempCp)
                                        foreach (var keyValueCp in tempCp)
                                        {
                                            if (keyValueCp.Key == "id")
                                                continue;
                                            if (keyValueCp.Value.Type == JTokenType.Null)
                                                continue;

                                            if (!IsNullOrEmpty(tempstring))
                                                tempstring += " , ";

                                            tempstring +=
                                                $"{GetGermanTranslation(keyValueCp.Key)} : {keyValueCp.Value}";
                                        }
                                }
                                continue;
                            }

                            if (keyValuePair2.Key == "id")
                                continue;
                            if (keyValuePair2.Value.Type == JTokenType.Null)
                                continue;

                            if (!IsNullOrEmpty(tempstring))
                                tempstring += " , ";

                            tempstring += $"{GetGermanTranslation(keyValuePair2.Key)} : {keyValuePair2.Value}";
                        }

                        listItem += @$"({GetGermanTranslation(@object.GetValue("type").ToString())}: ""{tempstring}"")</li>";
                    }
                    else
                    {
                        listItem += @$"({GetGermanTranslation(@object.GetValue("type").ToString())}: ""{GetGermanTranslation(@object.GetValue("data").ToString())}"")</li>";
                    }

                    list.Add(listItem);
                    break;
                }
            }

            return list;
        }

        /// <summary>
        /// Dictionary with translations needed
        /// </summary>
        /// <param name="englishKey">search phrase</param>
        /// <returns></returns>
        private string GetGermanTranslation(string englishKey)
        {
            var searchphrase = englishKey.ToLower();
            switch (searchphrase)
            {
                case "created": return "neu";
                case "updated": return "aktualisiert";
                case "deleted": return "gelöscht";
                case "name": return "Name";
                case "description": return "Beschreibung";
                case "prename": return "Vorname";
                case "city": return "Stadt";
                case "street": return "Straße";
                case "streetnumber": return "Hausnummer";
                case "zipcode": return "Postleitzahl";
                case "country": return "Land";
                case "phonenumber": return "Telefonnummer";
                case "fax": return "Faxnummer";
                case "mail": return "E-Mail";
                case "date": return "Datum";
                case "time": return "Zeitpunkt";
                case "duration": return "dauer";
                case "type": return "Typ";
                case "comment": return "Kommentar";
                case "contactentryname": return "Name der Kontaktmöglichkeit";
                case "contactentryvalue": return "Inhalt der Kontaktmöglichkeit";
                case "id": return "Id";
                case "address": return "Adresse";
                case "contactpossibilities": return "Kontaktmöglichkeiten";
                case "events": return "Veranstaltungen";
                case "organizations": return "Organisationen";
                case "history": return "Historie";
                case "contactentries": return "Kontakteinträge";
                case "organizationcontacts": return "Organisation in Verbindung zu Kontakt";

                default: return englishKey;
            }
        }
    }
}
