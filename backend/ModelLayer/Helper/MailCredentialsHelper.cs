using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Mail;
using System.Text;

namespace ModelLayer.Helper
{
    public class MailCredentialsHelper
    {
        private const string FILENAME = "\\mailCredentials.info";
        private static MailCredentials mailCredentials;

        /// <summary>
        /// Save the mail credentials into file
        /// </summary>
        /// <param name="mailCredentialsIntern">the mail credentials</param>
        public static void SaveMailCredentials(MailCredentials mailCredentialsIntern)
        {
            mailCredentials = mailCredentialsIntern;
            File.Delete(Directory.GetCurrentDirectory() + FILENAME);
            WriteToBinaryFile(Directory.GetCurrentDirectory() + FILENAME, new MailCredentialsSerializable(mailCredentialsIntern, true));
        }

        /// <summary>
        /// Check if initially the file with data exists.
        /// </summary>
        /// <param name="mailCredentialsIntern">the credentials to save if file does not exist</param>
        public static void CheckIfCredentialsExist(MailCredentials mailCredentialsIntern)
        {
            if (!File.Exists(Directory.GetCurrentDirectory() + FILENAME))
            {
                SaveMailCredentials(mailCredentialsIntern);
            }
        }

        /// <summary>
        /// Getter for the mail credentials
        /// </summary>
        /// <returns></returns>
        public static MailCredentials GetMailCredentials()
        {
            if (mailCredentials == null)
            {
                ReadMailCredentials();
            }

            return mailCredentials;
        }

        private static void ReadMailCredentials()
        {
            MailCredentialsSerializable mailCredentialsIntern = ReadFromBinaryFile<MailCredentialsSerializable>(Directory.GetCurrentDirectory() + "\\mailCredentials.info");
            mailCredentials = new MailCredentials(mailCredentialsIntern, true);
        }

        /// <summary>
        /// Writes the given object instance to a binary file.
        /// <para>Object type (and all child types) must be decorated with the [Serializable] attribute.</para>
        /// <para>To prevent a variable from being serialized, decorate it with the [NonSerialized] attribute; cannot be applied to properties.</para>
        /// </summary>
        /// <typeparam name="T">The type of object being written to the binary file.</typeparam>
        /// <param name="filePath">The file path to write the object instance to.</param>
        /// <param name="objectToWrite">The object instance to write to the binary file.</param>
        /// <param name="append">If false the file will be overwritten if it already exists. If true the contents will be appended to the file.</param>
        private static void WriteToBinaryFile<T>(string filePath, T objectToWrite, bool append = false)
        {
            using (Stream stream = File.Open(filePath, append ? FileMode.Append : FileMode.Create))
            {
                var binaryFormatter = new System.Runtime.Serialization.Formatters.Binary.BinaryFormatter();
                try
                {
                    binaryFormatter.Serialize(stream, objectToWrite);
                }
                catch
                {
                }
            }
        }

        /// <summary>
        /// Reads an object instance from a binary file.
        /// </summary>
        /// <typeparam name="T">The type of object to read from the binary file.</typeparam>
        /// <param name="filePath">The file path to read the object instance from.</param>
        /// <returns>Returns a new instance of the object read from the binary file.</returns>
        private static T ReadFromBinaryFile<T>(string filePath)
        {
            using (Stream stream = File.Open(filePath, FileMode.Open))
            {
                var binaryFormatter = new System.Runtime.Serialization.Formatters.Binary.BinaryFormatter();
                try
                {
                    return (T)binaryFormatter.Deserialize(stream);
                }
                catch
                {
                    return default(T);
                }
            }
        }
    }

    /// <summary>
    /// The serializable variant of the class containing all necessary information about the mail settings
    /// </summary>
    [Serializable]
    public class MailCredentialsSerializable
    {
        public MailCredentialsSerializable() { }

        public MailCredentialsSerializable(MailCredentials mailCredentials, bool useBase64Encoding = false)
        {
            if (useBase64Encoding)
            {
                MailAddress = Convert.ToBase64String(Encoding.ASCII.GetBytes(mailCredentials.MailAddress.Address));
                DisplayName = Convert.ToBase64String(Encoding.ASCII.GetBytes(mailCredentials.MailAddress.DisplayName));
                UserName = Convert.ToBase64String(Encoding.ASCII.GetBytes(mailCredentials.NetworkCredential.UserName));
                Password = Convert.ToBase64String(Encoding.ASCII.GetBytes(mailCredentials.NetworkCredential.Password));
                Port = mailCredentials.Port;
                Host = Convert.ToBase64String(Encoding.ASCII.GetBytes(mailCredentials.Host));
            }
            else
            {
                MailAddress = mailCredentials.MailAddress.Address;
                DisplayName = mailCredentials.MailAddress.DisplayName;
                UserName = mailCredentials.NetworkCredential.UserName;
                Password = mailCredentials.NetworkCredential.Password;
                Port = mailCredentials.Port;
                Host = mailCredentials.Host;
            }
        }

        public string MailAddress { get; private set; }
        public string DisplayName { get; private set; }
        public string UserName { get; private set; }
        public string Password { get; private set; }
        public int Port { get; private set; }
        public string Host { get; private set; }
    }

    /// <summary>
    /// The class containing all necessary information about the mail settings.
    /// </summary>
    public class MailCredentials
    {
        public MailCredentials(MailCredentialsSerializable mailCredentialsSerializable, bool useBase64Decoding = false)
        {
            if (useBase64Decoding)
            {
                MailAddress = new MailAddress(Encoding.ASCII.GetString(Convert.FromBase64String(mailCredentialsSerializable.MailAddress)),
                Encoding.ASCII.GetString(Convert.FromBase64String(mailCredentialsSerializable.DisplayName)));
                NetworkCredential = new NetworkCredential(Encoding.ASCII.GetString(Convert.FromBase64String(mailCredentialsSerializable.UserName)),
                    Encoding.ASCII.GetString(Convert.FromBase64String(mailCredentialsSerializable.Password)));
                Port = mailCredentialsSerializable.Port;
                Host = Encoding.ASCII.GetString(Convert.FromBase64String(mailCredentialsSerializable.Host));
            }
            else
            {
                MailAddress = new MailAddress(mailCredentialsSerializable.MailAddress,
                    mailCredentialsSerializable.DisplayName);
                NetworkCredential = new NetworkCredential(mailCredentialsSerializable.UserName,
                   mailCredentialsSerializable.Password);
                Port = mailCredentialsSerializable.Port;
                Host = mailCredentialsSerializable.Host;
            }
        }

        public MailCredentials(MailAddress mailAddress, NetworkCredential networkCredential, int port, string host)
        {
            MailAddress = mailAddress;
            NetworkCredential = networkCredential;
            Port = port;
            Host = host;
        }

        public MailAddress MailAddress { get; private set; }
        public NetworkCredential NetworkCredential { get; private set; }
        public int Port { get; private set; }
        public string Host { get; private set; }
    }
}
