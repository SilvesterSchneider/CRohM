using ModelLayer.DataTransferObjects;
using ModelLayer.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace ModelLayer.Helper
{
    public class ComparerForModificationEntryCreation
    {

        /// <summary>
        /// compare two events together
        /// </summary>
        /// <param name="oldEvent">the old event</param>
        /// <param name="newEvent">the new event</param>
        /// <param name="userOfModification">who made some changes?</param>
        /// <param name="listWithCreation">the list with all modification entries to be created afterwards</param>
        /// <param name="listWithDeletion">the list with all modification entries to be set up to deletion state = true</param>
        public static void CompareEvents(Event oldEvent, EventDto newEvent, string userOfModification,
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
            GetContactsChangeOfEvents(oldEvent.Contacts, newEvent.Contacts, newEvent.Id, userOfModification, listEntries);
            GetParticipatedChangesOfEvent(oldEvent.Participated, newEvent.Participated, listEntries, userOfModification, newEvent.Id);
            listWithCreation = listEntries;
        }

        /// <summary>
        /// check whether the contacts of the list of a event have changed
        /// </summary>
        /// <param name="oldContacts">the old contacts of list</param>
        /// <param name="newContacts">the new contacts of list</param>
        /// <param name="id">the model id</param>
        /// <param name="userOfModification">who made this change?</param>
        /// <param name="listEntries">the list with all entries to create</param>
        private static void GetContactsChangeOfEvents(List<EventContact> oldContacts, List<ContactDto> newContacts, long id, string userOfModification, List<ModificationEntry> listEntries)
        {
            foreach (EventContact connection in oldContacts)
            {
                if (newContacts.Find(a => a.Id == connection.ContactId) == null)
                {
                    listEntries.Add(GetNewModificationEntry("", connection.Contact.PreName + " " + connection.Contact.Name, id, MODEL_TYPE.EVENT, DATA_TYPE.CONTACTS, userOfModification, MODIFICATION.DELETED));
                }
            }
            foreach (ContactDto contact in newContacts)
            {
                if (oldContacts.Find(a => a.Contact.Id == contact.Id) == null)
                {
                    listEntries.Add(GetNewModificationEntry(contact.PreName + " " + contact.Name, "", id, MODEL_TYPE.EVENT, DATA_TYPE.CONTACTS, userOfModification, MODIFICATION.ADDED));
                }
            }
        }

        /// <summary>
        /// check if the participated state have changed of an event
        /// </summary>
        /// <param name="oldOnes">the old participated list</param>
        /// <param name="newOnes">the new participated list</param>
        /// <param name="listEntries">the list with all entries to create</param>
        /// <param name="userOfModification">who made this change?</param>
        /// <param name="id">the model id</param>
        private static void GetParticipatedChangesOfEvent(List<Participated> oldOnes, List<ParticipatedDto> newOnes, List<ModificationEntry> listEntries, string userOfModification, long id)
        {
            foreach (Participated partOld in oldOnes)
            {
                ParticipatedDto newPart = newOnes.Find(a => a.Id == partOld.Id);
                if (newPart != null && newPart.HasParticipated != partOld.HasParticipated)
                {
                    listEntries.Add(GetNewModificationEntry(newPart.HasParticipated.ToString(), partOld.HasParticipated.ToString(), id, MODEL_TYPE.EVENT, DATA_TYPE.PARTICIPATED, userOfModification, MODIFICATION.MODIFIED));
                }
            }
        }

        /// <summary>
        /// compare two contacts together.
        /// </summary>
        /// <param name="oldContact">the old contact</param>
        /// <param name="newContact">the new contact</param>
        /// <param name="userOfModification">who made a change?</param>
        /// <param name="deleteEntries">if true, then the deleted property will be unvisible for all older ones as well, if false the older values stay</param>
        /// <param name="listWithCreation">the list with entries which should be created afterwards</param>
        /// <param name="listWithDeletion">the list with modification entries which should be deleted afterwards</param>
        /// <param name="nextNewId">the next new index of modification entry of entities to consider</param>
        public static void CompareContacts(Contact oldContact, Contact newContact, string userOfModification, bool deleteEntries,
            out List<ModificationEntry> listWithCreation, out List<ModificationEntry> listWithDeletion, int nextNewId)
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
                listCreation.Add(GetNewModificationEntry(newContact.History[newContact.History.Count - 1].Description + ":" + newContact.History[newContact.History.Count - 1].Comment, "", newContact.Id, MODEL_TYPE.CONTACT, DATA_TYPE.HISTORY_ELEMENT, userOfModification, MODIFICATION.ADDED));
            }
            listCreation.AddRange(GetModificationsForAddressObject(oldContact.Address, newContact.Address, newContact.Id, userOfModification, MODEL_TYPE.CONTACT));
            GetModificationsForContactPossibilitiesObject(oldContact.ContactPossibilities, newContact.ContactPossibilities, newContact.Id, userOfModification, deleteEntries, listCreation, listDeletion, MODEL_TYPE.CONTACT, nextNewId);
            listWithCreation = listCreation;
            listWithDeletion = listDeletion;
        }

        /// <summary>
        /// compare organizations together.
        /// </summary>
        /// <param name="oldOrga">the old one</param>
        /// <param name="newOrga">the new one</param>
        /// <param name="userOfModification">who made this change?</param>
        /// <param name="deleteEntries">if true, then the deleted property will be unvisible for all older ones as well, if false the older values stay</param>
        /// <param name="listWithCreation">the list with entries which should be created afterwards</param>
        /// <param name="listWithDeletion">the list with modification entries which should be deleted afterwards</param>
        /// <param name="nextNewId">the next new index of modification entry of entities to consider</param>
        public static void CompareOrganizations(Organization oldOrga, Organization newOrga, string userOfModification, bool deleteEntries,
            out List<ModificationEntry> listWithCreation, out List<ModificationEntry> listWithDeletion, int nextNewId)
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
            //    if (oldOrga.History.Count != newOrga.History.Count)
            //   {
            //       listCreation.Add(GetNewModificationEntry(newOrga.History.Count.ToString(), oldOrga.History.Count.ToString(), newOrga.Id, MODEL_TYPE.ORGANIZATION, DATA_TYPE.HISTORY_ELEMENT, userOfModification, MODIFICATION.ADDED));
            //   }
            listCreation.AddRange(GetModificationsForAddressObject(oldOrga.Address, newOrga.Address, newOrga.Id, userOfModification, MODEL_TYPE.ORGANIZATION));
            GetModificationsForContactPossibilitiesObject(oldOrga.Contact, newOrga.Contact, newOrga.Id, userOfModification, deleteEntries, listCreation, listDeletion, MODEL_TYPE.ORGANIZATION, nextNewId);
            listWithCreation = listCreation;
            listWithDeletion = listDeletion;
        }

        /// <summary>
        /// check contact possibilities object for differences.
        /// </summary>
        /// <param name="oldOne">the old one</param>
        /// <param name="newOne">the new one</param>
        /// <param name="idOfModel">the id of model</param>
        /// <param name="userOfModification">who made a change?</param>
        /// <param name="deleteEntries">if true, then the deleted property will be unvisible for all older ones as well, if false the older values stay</param>
        /// <param name="listWithCreation">the list with entries which should be created afterwards</param>
        /// <param name="listWithDeletion">the list with modification entries which should be deleted afterwards</param>
        /// <param name="nextNewId">the next new index of modification entry of entities to consider</param>
        /// <param name="modelType">for which model type</param>
        private static void GetModificationsForContactPossibilitiesObject(ContactPossibilities oldOne, ContactPossibilities newOne, long idOfModel, string userOfModification, bool deleteEntries,
            List<ModificationEntry> listWithCreation, List<ModificationEntry> listWithDeletion, MODEL_TYPE modelType, int nextNewId)
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
            GetModificationsForContactEntriesObject(oldOne.ContactEntries, newOne.ContactEntries, idOfModel, userOfModification, deleteEntries, listWithCreation, listWithDeletion, modelType, nextNewId);
        }

        /// <summary>
        /// check the contact possibilities entries for changes.
        /// </summary>
        /// <param name="oldOne">the old ones</param>
        /// <param name="newOne">the new ones</param>
        /// <param name="idOfModel">the id of model</param>
        /// <param name="userOfModification">who made the changes?</param>
        /// <param name="deleteEntries">if true, then the deleted property will be unvisible for all older ones as well, if false the older values stay</param>
        /// <param name="listWithCreation">the list with entries which should be created afterwards</param>
        /// <param name="listWithDeletion">the list with modification entries which should be deleted afterwards</param>
        /// <param name="nextNewId">the next new index of modification entry of entities to consider</param>
        /// <param name="modelType">for which model type</param>
        private static void GetModificationsForContactEntriesObject(List<ContactPossibilitiesEntry> oldOne, List<ContactPossibilitiesEntry> newOne, long idOfModel, string userOfModification, bool deleteEntries,
            List<ModificationEntry> listWithCreation, List<ModificationEntry> listWithDeletion, MODEL_TYPE modelType, int nextNewId)
        {
            foreach (ContactPossibilitiesEntry entryOld in oldOne)
            {
                DATA_TYPE dataType = DATA_TYPE.PHONE_EXTENDED;
                if (new MailValidator().IsValid(entryOld.ContactEntryValue))
                {
                    dataType = DATA_TYPE.MAIL_EXTENDED;
                }
                ContactPossibilitiesEntry newEntryToFind = newOne.Find(a => a.Id == entryOld.Id);
                if (newEntryToFind == null && deleteEntries)
                {                    
                    listWithDeletion.Add(GetNewModificationEntry("" , entryOld.ContactEntryValue, idOfModel, modelType, dataType, userOfModification, MODIFICATION.DELETED, (int) entryOld.Id, true));
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
                        listWithCreation.Add(GetNewModificationEntry(newName + ":" + newValue, entryOld.ContactEntryName + ":" + entryOld.ContactEntryValue, idOfModel, modelType, dataType, userOfModification, MODIFICATION.MODIFIED, (int)entryOld.Id));
                    }                    
                }
            }
            foreach (ContactPossibilitiesEntry entryNew in newOne)
            { 
                if (oldOne.Find(a => a.Id == entryNew.Id) == null)
                {
                    DATA_TYPE dataType = DATA_TYPE.PHONE_EXTENDED;
                    if (new MailValidator().IsValid(entryNew.ContactEntryValue))
                    {
                        dataType = DATA_TYPE.MAIL_EXTENDED;
                    }
                    listWithCreation.Add(GetNewModificationEntry(entryNew.ContactEntryName + ":" + entryNew.ContactEntryValue, "", idOfModel, modelType, dataType, userOfModification, MODIFICATION.ADDED, nextNewId++));
                }                
            }
        }

        /// <summary>
        /// compare two addresses objects together.
        /// </summary>
        /// <param name="oldOne">the old one</param>
        /// <param name="newOne">the new one</param>
        /// <param name="idOfModel">the id of model</param>
        /// <param name="userOfModification">who made this change?</param>
        /// <param name="modelType">the model type</param>
        /// <returns></returns>
        private static List<ModificationEntry> GetModificationsForAddressObject(Address oldOne, Address newOne, long idOfModel, string userOfModification, MODEL_TYPE modelType)
        {
            List<ModificationEntry> list = new List<ModificationEntry>();
            if (!oldOne.City.Equals(newOne.City))
            {
                list.Add(GetNewModificationEntry(newOne.City, oldOne.City, idOfModel, modelType, DATA_TYPE.CITY, userOfModification, MODIFICATION.MODIFIED));
            }
            if (!oldOne.Country.Equals(newOne.Country))
            {
                list.Add(GetNewModificationEntry(newOne.Country, oldOne.Country, idOfModel, modelType, DATA_TYPE.COUNTRY, userOfModification, MODIFICATION.MODIFIED));
            }
            if (!oldOne.Street.Equals(newOne.Street))
            {
                list.Add(GetNewModificationEntry(newOne.Street, oldOne.Street, idOfModel, modelType, DATA_TYPE.STREET, userOfModification, MODIFICATION.MODIFIED));
            }
            if (!oldOne.StreetNumber.Equals(newOne.StreetNumber))
            {
                list.Add(GetNewModificationEntry(newOne.StreetNumber, oldOne.StreetNumber, idOfModel, modelType, DATA_TYPE.STREETNUMBER, userOfModification, MODIFICATION.MODIFIED));
            }
            if (!oldOne.Zipcode.Equals(newOne.Zipcode))
            {
                list.Add(GetNewModificationEntry(newOne.Zipcode, oldOne.Zipcode, idOfModel, modelType, DATA_TYPE.ZIPCODE, userOfModification, MODIFICATION.MODIFIED));
            }
            return list;
        }

        /// <summary>
        /// create a new modification entry
        /// </summary>
        /// <param name="actualValue">the actual, new value</param>
        /// <param name="oldValue">the old value</param>
        /// <param name="idOfModel">the id of model</param>
        /// <param name="modelType">the model type</param>
        /// <param name="dataType">the data type</param>
        /// <param name="userOfModification">who made the change?</param>
        /// <param name="modification">the modification type</param>
        /// <param name="index">the id of contact possibilities entry</param>
        /// <param name="shouldBeDeleted">if true, then this object should not make visible its values</param>
        /// <returns></returns>
        private static ModificationEntry GetNewModificationEntry(string actualValue, string oldValue, long idOfModel, MODEL_TYPE modelType, DATA_TYPE dataType,
            string userOfModification, MODIFICATION modification, int index = -1, bool shouldBeDeleted = false)
        {
            return new ModificationEntry() { ActualValue = actualValue, DataModelId = idOfModel, DataModelType = modelType, DataType = dataType,
                DateTime = DateTime.Now, ModificationType = modification, OldValue = oldValue, UserName = userOfModification, ExtensionIndex = index,
            IsDeleted = shouldBeDeleted };
        }
    }
}
