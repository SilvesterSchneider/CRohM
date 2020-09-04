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

        public static readonly string[] ALL_CLAIMS =
        {
            "Werte lesen",
            "Werte schreiben",
            "Werte löschen"
        };

        private static List<Claim> claims = GetAllPredefinedClaims();

        public static List<Claim> GetAllClaims()
        {
            return new List<Claim>(claims);
        }

        private static List<Claim> GetAllPredefinedClaims()
        {
            List<Claim> list = new List<Claim>();
            foreach (string claim in ALL_CLAIMS)
            {
                list.Add(new Claim(claim, claim));
            }
            return list;
        }
    }
}
