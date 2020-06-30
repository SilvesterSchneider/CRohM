using System;
using System.Collections.Generic;
using System.Text;

namespace ModelLayer.Helper
{
    public class PasswordGuidelines
    {
        public const bool RequireDigit = true;
        public const bool RequireLowercase = false;
        public const bool RequireNonAlphanumeric = true;
        public const bool RequireUppercase = false;
        public const int RequiredMinLength = 5;
        public const int RequiredUniqueChars = 0;
        public const int MaxLength = 64;

        public static int GetMaximumLength()
        {
            return RequiredMinLength * 2;
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

        public static bool IsPasswordWithinRestrictions(string password)
        {
            if (password.Length < RequiredMinLength)
            {
                return false;
            }
            int requiredAmountOfSpecialChar = GetAmountOfSpecialChars();
            if (requiredAmountOfSpecialChar > 0)
            {
                if (GetAmountOfSpecialChars(password) < requiredAmountOfSpecialChar)
                {
                    return false;
                }
            }
            int requiredAmountOfDigits = GetAmountOfNumerics();
            if (requiredAmountOfDigits > 0)
            {
                if (GetAmountOfNumerics(password) < requiredAmountOfDigits)
                {
                    return false;
                }
            }
            int requiredUpperLetters = GetAmountOfUpperLetters();
            if (requiredUpperLetters > 0)
            {
                if (GetAmountOfUpperLetters(password) < requiredUpperLetters)
                {
                    return false;
                }
            }
            int requiredLowerLetters = GetAmountOfLowerLetters();
            if (requiredLowerLetters > 0)
            {
                if (GetAmountOfLowerLetters(password) < requiredLowerLetters)
                {
                    return false;
                }
            }
            return true;
        }

        private static int GetAmountOfLowerLetters(string password)
        {
            int amount = 0;
            foreach (char ch in password)
            {
                if (Char.IsLower(ch))
                {
                    amount++;
                }
            }
            return amount;
        }

        private static int GetAmountOfUpperLetters(string password)
        {
            int amount = 0;
            foreach (char ch in password)
            {
                if (Char.IsUpper(ch))
                {
                    amount++;
                }
            }
            return amount;
        }

        private static int GetAmountOfNumerics(string password)
        {
            int amount = 0;
            foreach (char ch in password)
            {
                if (Char.IsDigit(ch))
                {
                    amount++;
                }
            }
            return amount;
        }

        private static int GetAmountOfSpecialChars(string password)
        {
            int amount = 0;
            foreach (char ch in password)
            {
                if (!Char.IsLetterOrDigit(ch))
                {
                    amount++;
                }
            }
            return amount;
        }
    }
}
