using ModelLayer.Models.Base;
using System;

namespace ModelLayer.Models
{
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
        PARTICIPATED
    }

    public enum MODEL_TYPE
    {
        CONTACT,
        ORGANIZATION,
        EVENT
    }

    public enum MODIFICATION
    {
        CREATED,
        MODIFIED,
        DELETED,
        ADDED
    }

    public class ModificationEntry : BaseEntity
    {
        private string actualValue;

        public DATA_TYPE DataType { get; set; }
        public MODEL_TYPE DataModelType { get; set; }
        public long DataModelId { get; set; }
        public MODIFICATION ModificationType { get; set; }
        public string UserName { get; set; }
        public DateTime DateTime { get; set; }
        public string OldValue { get; set; }
        public string ActualValue {
            get
            {
                if (!IsDeleted)
                {
                    return actualValue;
                }
                else
                {
                    return GetDeletionInfo();
                }
            }
            set
            {
                actualValue = value;
            }
        }
        public bool IsDeleted { get; set; } = false;
        public int ExtensionIndex { get; set; } = -1;
        public string PropertyName {
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
                    return "Teilnehmeranzahl";
                }
                else if (DataType == DATA_TYPE.FAX)
                {
                    return "Faxnr.";
                }
                else if (DataType == DATA_TYPE.HISTORY_ELEMENT)
                {
                    return "Historieanzahl";
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
                else 
                {
                    return "PLZ";
                }
            }
            set
            {

            }
        }

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
