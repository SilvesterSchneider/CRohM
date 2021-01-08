using ModelLayer.Models.Base;
using System;
using System.Runtime.Serialization;

namespace ModelLayer.Models
{
    /// <summary>
    /// RAM: 100%
    /// </summary>
    /// <summary>
    /// Alle feldtypen, welche grundsätzlich verändert werden können.
    /// </summary>
    public enum DATA_TYPE
    {
        MAIL,
        PHONE,
        FAX,
        PHONE_EXTENDED,
        MAIL_EXTENDED,
        HISTORY_ELEMENT,
        NONE,
        PRENAME,
        NAME,
        CITY,
        COUNTRY,
        STREET,
        STREETNUMBER,
        ZIPCODE,
        DESCRIPTION,
        CONTACTS,
        DURATION,
        DATE,
        TIME,
        PARTICIPATED,
        TAG,
        INVITATION,
        CONTACT_PARTNER,
        GENDER,
        LOCATION
    }

    /// <summary>
    /// RAM: 100%
    /// </summary>
    /// <summary>
    /// Alle modelltypen für die es felder geben kann
    /// </summary>
    public enum MODEL_TYPE
    {
        [EnumMember(Value = "Kontakt")]
        CONTACT,

        [EnumMember(Value = "Organisation")]
        ORGANIZATION,

        [EnumMember(Value = "Veranstaltung")]
        EVENT
    }

    /// <summary>
    /// RAM: 100%
    /// </summary>
    /// <summary>
    /// die art der veränderung.
    /// </summary>
    public enum MODIFICATION
    {
        CREATED,
        MODIFIED,
        DELETED,
        ADDED
    }

    /// <summary>
    /// RAM: 100%
    /// </summary>
    public class ModificationEntry : BaseEntity
    {
        public DATA_TYPE DataType { get; set; }
        public MODEL_TYPE DataModelType { get; set; }
        public long DataModelId { get; set; }
        public MODIFICATION ModificationType { get; set; }

        /// <summary>
        /// der user der die veränderung herbeigeführt hat.
        /// </summary>
        public User User { get; set; }

        /// <summary>
        /// zeitpunkt der veränderung.
        /// </summary>
        public DateTime DateTime { get; set; }

        public string OldValue { get; set; } = "";
        public string ActualValue { get; set; } = "";

        /// <summary>
        /// wurde das item nachträglich durch einen datenschutzbeauftragten gelöscht?
        /// </summary>
        public void SetToDeletionState()
        {
            OldValue = GetDeletionInfo();
            ActualValue = GetDeletionInfo();
        }

        /// <summary>
        /// die id des konkreten contact possibility entry eintrags aus den erweiterten kontaktmöglichkeiten.
        /// </summary>
        public int ExtensionIndex { get; set; } = -1;

        /// <summary>
        /// der name des feldes für die anzeige im web.
        /// </summary>
        public string PropertyName
        {
            get
            {
                if (DataType == DATA_TYPE.CITY)
                {
                    return "Stadt";
                }
                else if (DataType == DATA_TYPE.COUNTRY)
                {
                    return "Land";
                }
                else if (DataType == DATA_TYPE.DESCRIPTION)
                {
                    return "Beschreibung";
                }
                else if (DataType == DATA_TYPE.CONTACTS)
                {
                    return "Teilnehmer";
                }
                else if (DataType == DATA_TYPE.FAX)
                {
                    return "Faxnr.";
                }
                else if (DataType == DATA_TYPE.HISTORY_ELEMENT)
                {
                    return "Historienanzahl";
                }
                else if (DataType == DATA_TYPE.MAIL || DataType == DATA_TYPE.MAIL_EXTENDED)
                {
                    return "Mailaddresse";
                }
                else if (DataType == DATA_TYPE.NAME)
                {
                    return "Name";
                }
                else if (DataType == DATA_TYPE.PHONE || DataType == DATA_TYPE.PHONE_EXTENDED)
                {
                    return "Telefonnr.";
                }
                else if (DataType == DATA_TYPE.PRENAME)
                {
                    return "Vorname";
                }
                else if (DataType == DATA_TYPE.STREET)
                {
                    return "Strasse";
                }
                else if (DataType == DATA_TYPE.STREETNUMBER)
                {
                    return "Hausnr.";
                }
                else if (DataType == DATA_TYPE.DURATION)
                {
                    return "Dauer";
                }
                else if (DataType == DATA_TYPE.DATE)
                {
                    return "Datum";
                }
                else if (DataType == DATA_TYPE.TIME)
                {
                    return "Uhrzeit";
                }
                else if (DataType == DATA_TYPE.PARTICIPATED)
                {
                    return "Teilnahmestatus";
                }
                else if (DataType == DATA_TYPE.TAG)
                {
                    return "Tags";
                }
                else if (DataType == DATA_TYPE.INVITATION)
                {
                    return "Einladung";
                }
                else if (DataType == DATA_TYPE.ZIPCODE)
                {
                    return "PLZ";
                }
                else if (DataType == DATA_TYPE.GENDER)
                {
                    return "Geschlecht";
                }
                else if (DataType == DATA_TYPE.CONTACT_PARTNER)
                {
                    return "Ansprechpartner";
                }
                else if (DataType == DATA_TYPE.LOCATION)
                {
                    return "Ort";
                }
                else
                {
                    return "Angelegt";
                }
            }
            set
            {

            }
        }

        /// <summary>
        /// wurde gelöscht info generieren in abhängigkeit des datentypes.
        /// </summary>
        /// <returns></returns>
        private string GetDeletionInfo()
        {
            if (DataType == DATA_TYPE.FAX)
            {
                return "Faxnr. wurde gelöscht!";
            }
            else if (DataType == DATA_TYPE.MAIL || DataType == DATA_TYPE.MAIL_EXTENDED)
            {
                return "Mail wurde gelöscht!";
            }
            else
            {
                return "Telefonnr. wurde gelöscht!";
            }
        }
    }
}
