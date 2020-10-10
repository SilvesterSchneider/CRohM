using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Mail;
using System.Text;

namespace ModelLayer.Helper
{
    public class MailCredentialsHelper { 

        private static MailCredentials mailCredentials;

        public static void SaveMailCredentials(MailCredentials mailCredentialsIntern)
        {
            mailCredentials = mailCredentialsIntern;
            File.Delete(Directory.GetCurrentDirectory() + "\\mailCredentials.info");
            WriteToBinaryFile(Directory.GetCurrentDirectory() + "\\mailCredentials.info", new MailCredentialsSerializable(mailCredentialsIntern));
        }

        public static void CheckIfCredentialsExist(MailCredentials mailCredentialsIntern)
        {
            if (!File.Exists(Directory.GetCurrentDirectory() + "\\mailCredentials.info"))
            {
                SaveMailCredentials(mailCredentialsIntern);
            }
        }

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
            mailCredentials = new MailCredentials(new MailAddress(mailCredentialsIntern.MailAddress, mailCredentialsIntern.DisplayName),
                new System.Net.NetworkCredential(mailCredentialsIntern.UserName, mailCredentialsIntern.Password), mailCredentialsIntern.Port, mailCredentialsIntern.Host);
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
                catch (Exception e)
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
                catch (Exception e)
                {
                    return default(T);
                }
            }
        }
    }

    [Serializable]
    public class MailCredentialsSerializable
    {
        public MailCredentialsSerializable() { }

        public MailCredentialsSerializable(MailCredentials mailCredentials)
        {
            MailAddress = mailCredentials.MailAddress.Address;
            DisplayName = mailCredentials.MailAddress.DisplayName;
            UserName = mailCredentials.NetworkCredential.UserName;
            Password = mailCredentials.NetworkCredential.Password;
            Port = mailCredentials.Port;
            Host = mailCredentials.Host;
        }

        public string MailAddress { get; private set; }
        public string DisplayName { get; private set; }
        public string UserName { get; private set; }
        public string Password { get; private set; }
        public int Port { get; private set; }
        public string Host { get; private set; }
    }

    public class MailCredentials
    {
        public MailCredentials(MailCredentialsSerializable mailCredentialsSerializable)
        {
            MailAddress = new MailAddress(mailCredentialsSerializable.MailAddress, mailCredentialsSerializable.DisplayName);
            NetworkCredential = new NetworkCredential(mailCredentialsSerializable.UserName, mailCredentialsSerializable.Password);
            Port = mailCredentialsSerializable.Port;
            Host = mailCredentialsSerializable.Host;
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
