using System;
using System.Collections.Generic;
using System.Text;

namespace ModelLayer.Helper
{
    /// <summary>
    /// This is the interface for later usage of the mail provider.
    /// </summary>
    public interface IMailProvider
    {
        /// <summary>
        /// Send an email containing the new password for the user.
        /// </summary>
        /// <param name="newPassword">the new password</param>
        /// <param name="mailAddress">the mail address to be sended to</param>
        public void SendMailContainingNewPasswort(string newPassword, string mailAddress);
    }
}
