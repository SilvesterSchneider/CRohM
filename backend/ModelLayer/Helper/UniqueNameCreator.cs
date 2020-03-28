using System;
using System.Collections.Generic;
using System.Text;

namespace ModelLayer.Helper
{
    public class UniqueNameCreator
    {
        /// <summary>
        /// Creates the unique username on base of pararmeters.
        /// </summary>
        /// <param name="firstName">the first name</param>
        /// <param name="lastName">the last name</param>
        /// <param name="numberOfEntryInTable">the actual amount of numbers in the table of users</param>
        /// <returns>the unique string containing the username</returns>
        public static string CreateUniqueName(string firstName, string lastName, int numberOfEntryInTable)
        {
            return lastName + firstName.Substring(0, 2) + numberOfEntryInTable;
        }
    }
}
