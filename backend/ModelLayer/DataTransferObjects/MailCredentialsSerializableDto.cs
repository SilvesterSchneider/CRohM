using System;
using System.Collections.Generic;
using System.Text;

namespace ModelLayer.DataTransferObjects
{
    /// <summary>
    /// RAM: 100%
    /// </summary>
    public class MailCredentialsSerializableDto
    {
        public string MailAddress { get; set; }
        public string DisplayName { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        public int Port { get; set; }
        public string Host { get; set; }
    }
}
