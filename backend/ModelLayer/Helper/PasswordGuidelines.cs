using System;
using System.Collections.Generic;
using System.Text;

namespace ModelLayer.Helper
{
    public class PasswordGuidelines
    {
        public const bool RequireDigit = false;
        public const bool RequireLowercase = false;
        public const bool RequireNonAlphanumeric = false;
        public const bool RequireUppercase = false;
        public const int RequiredLength = 5;
        public const int RequiredUniqueChars = 0;

        public static int GetMaximumLength()
        {
            return RequiredLength * 2;
        }

        public static int GetAmountOfSpecialChars()
        {
            return RequireNonAlphanumeric ? 1 : 0;
        }

        public static int GetAmountOfUpperLetters()
        {
            return RequireUppercase ? 1 : 0;
        }

        public static int GetAmountOfLowerLetters()
        {
            return RequireLowercase ? 1 : 0;
        }

        public static int GetAmountOfNumerics()
        {
            return RequireDigit ? 1 : 0;
        }
    }
}
