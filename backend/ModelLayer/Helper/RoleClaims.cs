using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Text;

namespace ModelLayer.Helper
{
    public class RoleClaims
    {
        public static readonly string[] DEFAULT_GROUPS =
        {
            "Admin",
            "Datenschutzbeauftragter"
        };

        public static readonly string[] CONTACT_CLAIMS =
        {
            "Anlegen eines Kontakts",
            "Einsehen und Bearbeiten aller Kontakte",
            "Löschen eines Kontakts"
        };

        public static readonly string[] ORGANISATION_CLAIMS =
        {
            "Anlegen einer Organisation",
            "Einsehen und Bearbeiten aller Organisationen",
            "Zuordnen eines Kontakts zu einer Organisation",
            "Löschen einer Organisation"
        };

        public static readonly string[] CALENDAR_CLAIMS =
        {
            "Hinzufügen eines Historieneintrags bei Kontakt oder Organisation",
            "Anlegen einer Veranstaltung",
            "Einsehen und Bearbeiten einer Veranstaltung",
            "Löschen einer Veranstaltung"
        };

        public static readonly string[] DSGVO_CLAIMS =
        {
            "Auskunft gegenüber eines Kontakts zu dessen Daten",
            "Mitteilung an einen Kontakt nach Löschung oder Änderung"
        };

        public static readonly string[] ADMIN_CLAIMS =
        {
            "Anlegen eines Benutzers",
            "Zuweisung einer neuen Rolle zu einem Benutzer",
            "Rücksetzen eines Passworts eines Benutzers",
            "Löschen / Deaktivieren eines Benutzers",
            "Einsehen und Überarbeiten des Rollenkonzepts"
        };

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

        private static List<Claim> dsgvoClaims = GetAllDsgvoClaimsInternally();
        private static List<Claim> allClaims = GetAllPredefinedClaims();

        private static List<Claim> GetAllDsgvoClaimsInternally()
        {
            List<Claim> list = new List<Claim>();
            list.Add(new Claim(CONTACT_CLAIMS[1], CONTACT_CLAIMS[1]));
            list.Add(new Claim(CONTACT_CLAIMS[2], CONTACT_CLAIMS[2]));
            list.AddRange(GetClaimListForStringArray(DSGVO_CLAIMS));
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

        public static List<Claim> GetAllClaims()
        {
            return new List<Claim>(allClaims);
        }

        public static List<Claim> GetAllDsgvoClaims()
        {
            return new List<Claim>(dsgvoClaims);
        }

        private static List<Claim> GetAllPredefinedClaims()
        {
            List<Claim> list = new List<Claim>();
            list.Add(new Claim(CONTACT_CLAIMS[0], CONTACT_CLAIMS[0]));
            list.AddRange(dsgvoClaims);
            list.AddRange(GetClaimListForStringArray(ORGANISATION_CLAIMS));
            list.AddRange(GetClaimListForStringArray(CALENDAR_CLAIMS));
            list.AddRange(GetClaimListForStringArray(ADMIN_CLAIMS));
            return list;
        }
    }
}
