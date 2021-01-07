using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Text;

namespace ModelLayer.Helper
{
    public static class RoleClaims
    {
        /// <summary>
        /// The admin role
        /// </summary>
        public static readonly string ADMIN_GROUP = "Admin";

        /// <summary>
        /// datenschutzbeauftragter rolle
        /// </summary>
        public static readonly string DATA_SECURITY_ENGINEER_GROUP = "Datenschutzbeauftragter";

        /// <summary>
        /// The default role groups to be added on system startup
        /// </summary>
        public static readonly string[] DEFAULT_GROUPS =
        {
            ADMIN_GROUP,
            DATA_SECURITY_ENGINEER_GROUP
        };

        /// <summary>
        /// all contact related claims
        /// </summary>
        public static readonly string[] CONTACT_CLAIMS =
        {
            "Anlegen eines Kontakts",
            "Einsehen und Bearbeiten aller Kontakte",
            "Löschen eines Kontakts"
        };

        /// <summary>
        /// all organisations related claims
        /// </summary>
        public static readonly string[] ORGANISATION_CLAIMS =
        {
            "Anlegen einer Organisation",
            "Einsehen und Bearbeiten aller Organisationen",
            "Zuordnen eines Kontakts zu einer Organisation",
            "Löschen einer Organisation"
        };

        /// <summary>
        /// all calendar related claims
        /// </summary>
        public static readonly string[] CALENDAR_CLAIMS =
        {
            "Hinzufügen eines Historieneintrags bei Kontakt oder Organisation",
            "Anlegen einer Veranstaltung",
            "Einsehen und Bearbeiten einer Veranstaltung",
            "Löschen einer Veranstaltung"
        };

        /// <summary>
        /// all dsgvo related claims
        /// </summary>
        public static readonly string[] DSGVO_CLAIMS =
        {
            "Auskunft gegenüber eines Kontakts zu dessen Daten",
            "Mitteilung an einen Kontakt nach Löschung oder Änderung"
        };

        /// <summary>
        /// all admin related claims
        /// </summary>
        public static readonly string[] ADMIN_CLAIMS =
        {
            "Anlegen eines Benutzers",
            "Zuweisung einer neuen Rolle zu einem Benutzer",
            "Rücksetzen eines Passworts eines Benutzers",
            "Löschen / Deaktivieren eines Benutzers",
            "Einsehen und Überarbeiten des Rollenkonzepts"
        };

        /// <summary>
        /// all claims of the system
        /// </summary>
        public static readonly string[] ALL_CLAIMS = GetAllClaimsAsArray();

        private static string[] GetAllClaimsAsArray()
        {
            List<string> list = new List<string>();
            list.AddRange(CONTACT_CLAIMS);
            list.AddRange(ORGANISATION_CLAIMS);
            list.AddRange(CALENDAR_CLAIMS);
            list.AddRange(DSGVO_CLAIMS);
            list.AddRange(ADMIN_CLAIMS);
            return list.ToArray();
        }

        private static List<Claim> contactClaimsInternally = GetClaimListForStringArray(CONTACT_CLAIMS);
        private static List<Claim> dsgvoClaimsInternally = GetClaimListForStringArray(DSGVO_CLAIMS);
        private static List<Claim> adminClaimsInternally = GetClaimListForStringArray(ADMIN_CLAIMS);
        private static List<Claim> calendarClaimsInternally = GetClaimListForStringArray(CALENDAR_CLAIMS);
        private static List<Claim> organizationClaimsInternally = GetClaimListForStringArray(ORGANISATION_CLAIMS);
        private static List<Claim> dsgvoClaims = GetAllDsgvoClaimsInternally();
        private static List<Claim> allClaims = GetAllPredefinedClaims();
        private static List<Claim> adminClaims = GetAllAdminClaimsInternally();

        private static List<Claim> GetAllAdminClaimsInternally()
        {
            return allClaims;
        }

        private static List<Claim> GetAllDsgvoClaimsInternally()
        {
            List<Claim> list = new List<Claim>();
            list.Add(contactClaimsInternally[1]);
            list.Add(contactClaimsInternally[2]);
            list.AddRange(dsgvoClaimsInternally);
            return list;
        }

        private static List<Claim> GetClaimListForStringArray(string[] array)
        {
            List<Claim> list = new List<Claim>();
            foreach (string claim in array)
            {
                list.Add(new Claim(claim, claim));
            }
            return list;
        }

        /// <summary>
        /// Get all claims of this system
        /// </summary>
        /// <returns>list with claims</returns>
        public static List<Claim> GetAllClaims()
        {
            return new List<Claim>(allClaims);
        }

        /// <summary>
        /// Get all dsgvo related claims
        /// </summary>
        /// <returns>list with claims</returns>
        public static List<Claim> GetAllDsgvoClaims()
        {
            return new List<Claim>(dsgvoClaims);
        }

        /// <summary>
        /// Get all admin related claims
        /// </summary>
        /// <returns>list with claims</returns>
        public static List<Claim> GetAllAdminClaims()
        {
            return new List<Claim>(adminClaims);
        }

        private static List<Claim> GetAllPredefinedClaims()
        {
            List<Claim> list = new List<Claim>();
            list.AddRange(contactClaimsInternally);
            list.AddRange(organizationClaimsInternally);
            list.AddRange(calendarClaimsInternally);
            list.AddRange(dsgvoClaimsInternally);
            list.AddRange(adminClaimsInternally);
            return list;
        }
    }
}
