using ModelLayer.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace ModelLayer.Helper
{
    public class ComparerForModificationEntryCreation
    {
        public static void CompareEvents(Event oldEvent, Event newEvent, string userOfModification,
            out List<ModificationEntry> listWithCreation, out List<ModificationEntry> listWithDeletion)
        {
            listWithDeletion = new List<ModificationEntry>();
            List<ModificationEntry> listEntries = new List<ModificationEntry>();
            if (!oldEvent.Name.Equals(newEvent.Name))
            {
                listEntries.Add(GetNewModificationEntry(newEvent.Name, oldEvent.Name, newEvent.Id, MODEL_TYPE.EVENT, DATA_TYPE.NAME, userOfModification, MODIFICATION.MODIFIED));
            }
            if (!oldEvent.Date.Equals(newEvent.Date))
            {
                listEntries.Add(GetNewModificationEntry(newEvent.Date.ToString(), oldEvent.Date.ToString(), newEvent.Id, MODEL_TYPE.EVENT, DATA_TYPE.DATE, userOfModification, MODIFICATION.MODIFIED));
            }
            if (!oldEvent.Time.Equals(newEvent.Time))
            {
                listEntries.Add(GetNewModificationEntry(newEvent.Time.ToString(), oldEvent.Time.ToString(), newEvent.Id, MODEL_TYPE.EVENT, DATA_TYPE.TIME, userOfModification, MODIFICATION.MODIFIED));
            }
            if (!oldEvent.Duration.Equals(newEvent.Duration))
            {
                listEntries.Add(GetNewModificationEntry(newEvent.Duration.ToString(), oldEvent.Duration.ToString(), newEvent.Id, MODEL_TYPE.EVENT, DATA_TYPE.DURATION, userOfModification, MODIFICATION.MODIFIED));
            }
            GetParticipatedChangesOfEvent(oldEvent.Participated, newEvent.Participated, listEntries, userOfModification, newEvent.Id);
            listWithCreation = listEntries;
        }

        private static void GetParticipatedChangesOfEvent(List<Participated> oldOnes, List<Participated> newOnes, List<ModificationEntry> listEntries, string userOfModification, long id)
        {
            foreach (Participated partOld in oldOnes)
            {
                Participated newPart = newOnes.Find(a => a.Id == partOld.Id);
                if (newPart == null)
                {
                    listEntries.Add(GetNewModificationEntry("", partOld.HasParticipated.ToString(), id, MODEL_TYPE.EVENT, DATA_TYPE.PARTICIPATED, userOfModification, MODIFICATION.MODIFIED));
                }
                else if (newPart.HasParticipated != partOld.HasParticipated)
                {
                    listEntries.Add(GetNewModificationEntry(newPart.HasParticipated.ToString(), partOld.HasParticipated.ToString(), id, MODEL_TYPE.EVENT, DATA_TYPE.PARTICIPATED, userOfModification, MODIFICATION.MODIFIED));
                }
            }
            foreach (Participated partNew in newOnes)
            {
                if (oldOnes.Find(a => a.Id == partNew.Id) == null)
                {
                    listEntries.Add(GetNewModificationEntry(partNew.HasParticipated.ToString(), "", id, MODEL_TYPE.EVENT, DATA_TYPE.PARTICIPATED, userOfModification, MODIFICATION.ADDED));
                }
            }
        }

        public static void CompareContacts(Contact oldContact, Contact newContact, string userOfModification, bool deleteEntries,
            out List<ModificationEntry> listWithCreation, out List<ModificationEntry> listWithDeletion)
        {
            List<ModificationEntry> listCreation = new List<ModificationEntry>();
            List<ModificationEntry> listDeletion = new List<ModificationEntry>();
            if (!oldContact.Name.Equals(newContact.Name))
            {
                listCreation.Add(GetNewModificationEntry(newContact.Name, oldContact.Name, newContact.Id, MODEL_TYPE.CONTACT, DATA_TYPE.NAME, userOfModification, MODIFICATION.MODIFIED));
            }
            if (!oldContact.PreName.Equals(newContact.PreName))
            {
                listCreation.Add(GetNewModificationEntry(newContact.PreName, oldContact.PreName, newContact.Id, MODEL_TYPE.CONTACT, DATA_TYPE.PRENAME, userOfModification, MODIFICATION.MODIFIED));
            }
            if (oldContact.History.Count != newContact.History.Count)
            {
                listCreation.Add(GetNewModificationEntry(newContact.History.Count.ToString(), oldContact.History.Count.ToString(), newContact.Id, MODEL_TYPE.CONTACT, DATA_TYPE.HISTORY_ELEMENT, userOfModification, MODIFICATION.ADDED));
            }
            listCreation.AddRange(GetModificationsForAddressObject(oldContact.Address, newContact.Address, newContact.Id, userOfModification));
            GetModificationsForContactPossibilitiesObject(oldContact.ContactPossibilities, newContact.ContactPossibilities, newContact.Id, userOfModification, deleteEntries, listCreation, listDeletion, MODEL_TYPE.CONTACT);
            listWithCreation = listCreation;
            listWithDeletion = listDeletion;
        }

        public static void CompareOrganizations(Organization oldOrga, Organization newOrga, string userOfModification, bool deleteEntries,
            out List<ModificationEntry> listWithCreation, out List<ModificationEntry> listWithDeletion)
        {
            List<ModificationEntry> listCreation = new List<ModificationEntry>();
            List<ModificationEntry> listDeletion = new List<ModificationEntry>();
            if (!oldOrga.Name.Equals(newOrga.Name))
            {
                listCreation.Add(GetNewModificationEntry(newOrga.Name, oldOrga.Name, newOrga.Id, MODEL_TYPE.ORGANIZATION, DATA_TYPE.NAME, userOfModification, MODIFICATION.MODIFIED));
            }
            if (!oldOrga.Description.Equals(newOrga.Description))
            {
                listCreation.Add(GetNewModificationEntry(newOrga.Description, oldOrga.Description, newOrga.Id, MODEL_TYPE.ORGANIZATION, DATA_TYPE.DESCRIPTION, userOfModification, MODIFICATION.MODIFIED));
            }
            if (oldOrga.OrganizationContacts.Count != newOrga.OrganizationContacts.Count)
            {
                listCreation.Add(GetNewModificationEntry(newOrga.OrganizationContacts.Count.ToString(), oldOrga.OrganizationContacts.Count.ToString(), newOrga.Id, MODEL_TYPE.ORGANIZATION, DATA_TYPE.CONTACTS, userOfModification, MODIFICATION.ADDED));
            }
            //    if (oldOrga.History.Count != newOrga.History.Count)
            //   {
            //       listCreation.Add(GetNewModificationEntry(newOrga.History.Count.ToString(), oldOrga.History.Count.ToString(), newOrga.Id, MODEL_TYPE.ORGANIZATION, DATA_TYPE.HISTORY_ELEMENT, userOfModification, MODIFICATION.ADDED));
            //   }
            listCreation.AddRange(GetModificationsForAddressObject(oldOrga.Address, newOrga.Address, newOrga.Id, userOfModification));
            GetModificationsForContactPossibilitiesObject(oldOrga.Contact, newOrga.Contact, newOrga.Id, userOfModification, deleteEntries, listCreation, listDeletion, MODEL_TYPE.ORGANIZATION);
            listWithCreation = listCreation;
            listWithDeletion = listDeletion;
        }

        private static void GetModificationsForContactPossibilitiesObject(ContactPossibilities oldOne, ContactPossibilities newOne, long idOfModel, string userOfModification, bool deleteEntries,
            List<ModificationEntry> listWithCreation, List<ModificationEntry> listWithDeletion, MODEL_TYPE modelType)
        {
            if (!oldOne.Fax.Equals(newOne.Fax))
            {
                if (string.IsNullOrEmpty(newOne.Fax) && deleteEntries)
                {
                    listWithDeletion.Add(GetNewModificationEntry(newOne.Fax, oldOne.Fax, idOfModel, modelType, DATA_TYPE.FAX, userOfModification, MODIFICATION.MODIFIED, -1, true));
                } else
                {
                    listWithCreation.Add(GetNewModificationEntry(newOne.Fax, oldOne.Fax, idOfModel, modelType, DATA_TYPE.FAX, userOfModification, MODIFICATION.MODIFIED));
                }                
            }
            if (!oldOne.Mail.Equals(newOne.Mail))
            {
                if (string.IsNullOrEmpty(newOne.Mail) && deleteEntries)
                {
                    listWithDeletion.Add(GetNewModificationEntry(newOne.Mail, oldOne.Mail, idOfModel, modelType, DATA_TYPE.MAIL, userOfModification, MODIFICATION.MODIFIED, -1, true));
                }
                else
                {
                    listWithCreation.Add(GetNewModificationEntry(newOne.Mail, oldOne.Mail, idOfModel, modelType, DATA_TYPE.MAIL, userOfModification, MODIFICATION.MODIFIED));
                }
            }
            if (!oldOne.PhoneNumber.Equals(newOne.PhoneNumber))
            {
                if (string.IsNullOrEmpty(newOne.PhoneNumber) && deleteEntries)
                {
                    listWithDeletion.Add(GetNewModificationEntry(newOne.PhoneNumber, oldOne.PhoneNumber, idOfModel, modelType, DATA_TYPE.PHONE, userOfModification, MODIFICATION.MODIFIED, -1, true));
                }
                else
                {
                    listWithCreation.Add(GetNewModificationEntry(newOne.PhoneNumber, oldOne.PhoneNumber, idOfModel, modelType, DATA_TYPE.PHONE, userOfModification, MODIFICATION.MODIFIED));
                }
            }
            GetModificationsForContactEntriesObject(oldOne.ContactEntries, newOne.ContactEntries, idOfModel, userOfModification, deleteEntries, listWithCreation, listWithDeletion, modelType);
        }

        private static void GetModificationsForContactEntriesObject(List<ContactPossibilitiesEntry> oldOne, List<ContactPossibilitiesEntry> newOne, long idOfModel, string userOfModification, bool deleteEntries,
            List<ModificationEntry> listWithCreation, List<ModificationEntry> listWithDeletion, MODEL_TYPE modelType)
        {
            int idx = 0;
            foreach (ContactPossibilitiesEntry entryOld in oldOne)
            {
                DATA_TYPE dataType = DATA_TYPE.PHONE_EXTENDED;
                if (new MailValidator().IsValid(entryOld))
                {
                    dataType = DATA_TYPE.MAIL_EXTENDED;
                }
                ContactPossibilitiesEntry newEntryToFind = newOne.Find(a => a.Id == entryOld.Id);
                if (newEntryToFind == null && deleteEntries)
                {                    
                    listWithDeletion.Add(GetNewModificationEntry("" , entryOld.ContactEntryValue, idOfModel, modelType, dataType, userOfModification, MODIFICATION.MODIFIED, idx, true));
                }
                else 
                {
                    if (newEntryToFind == null || !newEntryToFind.ContactEntryName.Equals(entryOld.ContactEntryName) || !newEntryToFind.ContactEntryValue.Equals(entryOld.ContactEntryValue))
                    {
                        string newValue = string.Empty;
                        string newName = string.Empty;
                        if (newEntryToFind != null)
                        {
                            newName = newEntryToFind.ContactEntryName;
                            newValue = newEntryToFind.ContactEntryValue;
                        }
                        if (!newName.Equals(entryOld.ContactEntryName))
                        {
                            listWithCreation.Add(GetNewModificationEntry(newName, entryOld.ContactEntryName, idOfModel, modelType, dataType, userOfModification, MODIFICATION.MODIFIED, idx));
                        }
                        if (!newValue.Equals(entryOld.ContactEntryValue))
                        {
                            listWithCreation.Add(GetNewModificationEntry(newValue, entryOld.ContactEntryValue, idOfModel, modelType, dataType, userOfModification, MODIFICATION.MODIFIED, idx));
                        }
                    }                    
                }
                idx++;
            }
            idx = oldOne.Count;
            foreach (ContactPossibilitiesEntry entryNew in newOne)
            { 
                if (oldOne.Find(a => a.Id == entryNew.Id) == null)
                {
                    DATA_TYPE dataType = DATA_TYPE.PHONE_EXTENDED;
                    if (new MailValidator().IsValid(entryNew))
                    {
                        dataType = DATA_TYPE.MAIL_EXTENDED;
                    }
                    listWithCreation.Add(GetNewModificationEntry(entryNew.ContactEntryName, "", idOfModel, modelType, dataType, userOfModification, MODIFICATION.ADDED, idx));
                    listWithCreation.Add(GetNewModificationEntry(entryNew.ContactEntryValue, "", idOfModel, modelType, dataType, userOfModification, MODIFICATION.ADDED, idx));
                    idx++;
                }                
            }
        }

        private static List<ModificationEntry> GetModificationsForAddressObject(Address oldOne, Address newOne, long idOfModel, string userOfModification)
        {
            List<ModificationEntry> list = new List<ModificationEntry>();
            if (!oldOne.City.Equals(newOne.City))
            {
                list.Add(GetNewModificationEntry(newOne.City, oldOne.City, idOfModel, MODEL_TYPE.CONTACT, DATA_TYPE.CITY, userOfModification, MODIFICATION.MODIFIED));
            }
            if (!oldOne.Country.Equals(newOne.Country))
            {
                list.Add(GetNewModificationEntry(newOne.Country, oldOne.Country, idOfModel, MODEL_TYPE.CONTACT, DATA_TYPE.COUNTRY, userOfModification, MODIFICATION.MODIFIED));
            }
            if (!oldOne.Street.Equals(newOne.Street))
            {
                list.Add(GetNewModificationEntry(newOne.Street, oldOne.Street, idOfModel, MODEL_TYPE.CONTACT, DATA_TYPE.STREET, userOfModification, MODIFICATION.MODIFIED));
            }
            if (!oldOne.StreetNumber.Equals(newOne.StreetNumber))
            {
                list.Add(GetNewModificationEntry(newOne.StreetNumber, oldOne.StreetNumber, idOfModel, MODEL_TYPE.CONTACT, DATA_TYPE.STREETNUMBER, userOfModification, MODIFICATION.MODIFIED));
            }
            if (!oldOne.Zipcode.Equals(newOne.Zipcode))
            {
                list.Add(GetNewModificationEntry(newOne.Zipcode, oldOne.Zipcode, idOfModel, MODEL_TYPE.CONTACT, DATA_TYPE.ZIPCODE, userOfModification, MODIFICATION.MODIFIED));
            }
            return list;
        }

        private static ModificationEntry GetNewModificationEntry(string actualValue, string oldValue, long idOfModel, MODEL_TYPE modelType, DATA_TYPE dataType,
            string userOfModification, MODIFICATION modification, int index = -1, bool shouldBeDeleted = false)
        {
            return new ModificationEntry() { ActualValue = actualValue, DataModelId = idOfModel, DataModelType = modelType, DataType = dataType,
                DateTime = DateTime.Now, ModificationType = modification, OldValue = oldValue, UserName = userOfModification, ExtensionIndex = index,
            IsDeleted = shouldBeDeleted };
        }
    }
}
