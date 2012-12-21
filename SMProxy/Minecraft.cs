using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Net;
using System.Diagnostics;
using System.Xml.Linq;
using System.Runtime.InteropServices;

namespace SMProxy
{
    public static class Minecraft
    {
        private const string LoginUrl = "https://login.minecraft.net?user={0}&password={1}&version=13";
        private const string ResourceUrl = "http://s3.amazonaws.com/MinecraftResources/";
        private const string DownloadUrl = "http://s3.amazonaws.com/MinecraftDownload/";

        public static Session DoLogin(string Username, string Password)
        {
            HttpWebRequest request = (HttpWebRequest) WebRequest.Create(string.Format(LoginUrl,
                Uri.EscapeUriString(Username),
                Uri.EscapeUriString(Password)));
            var response = request.GetResponse();
            StreamReader responseStream = new StreamReader(response.GetResponseStream());
            string login = responseStream.ReadToEnd();
            responseStream.Close();
            if (login.Count(c => c == ':') != 4)
                return new Session(login.Trim());
            string[] parts = login.Split(':');
            return new Session(parts[2], parts[3], parts[0]);
        }

        public static string LastLoginFile
        {
            get
            {
                return Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), ".minecraft/lastlogin");
            }
        }

        public static string DotMinecraft
        {
            get
            {
                return Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), ".minecraft");
            }
        }

        private static readonly byte[] LastLoginSalt = new byte[] { 0x0c, 0x9d, 0x4a, 0xe4, 0x1e, 0x83, 0x15, 0xfc };
        private const string LastLoginPassword = "passwordfile";
        public static LastLogin GetLastLogin()
        {
            try
            {
                byte[] encryptedLogin = File.ReadAllBytes(LastLoginFile);
                PKCSKeyGenerator crypto = new PKCSKeyGenerator(LastLoginPassword, LastLoginSalt, 5, 1);
                ICryptoTransform cryptoTransform = crypto.Decryptor;
                byte[] decrypted = cryptoTransform.TransformFinalBlock(encryptedLogin, 0, encryptedLogin.Length);
                short userLength = IPAddress.HostToNetworkOrder(BitConverter.ToInt16(decrypted, 0));
                byte[] user = decrypted.Skip(2).Take(userLength).ToArray();
                short passLength = IPAddress.HostToNetworkOrder(BitConverter.ToInt16(decrypted, userLength + 2));
                byte[] password = decrypted.Skip(4 + userLength).ToArray();
                LastLogin result = new LastLogin();
                result.Username = System.Text.Encoding.UTF8.GetString(user);
                result.Password = System.Text.Encoding.UTF8.GetString(password);
                return result;
            }
            catch
            {
                return null;
            }
        }
    }

    public class LastLogin
    {
        public string Username { get; set; }
        public string Password { get; set; }
    }

    public class Session
    {
        public Session(string Error)
        {
            this.Error = Error;
        }

        public Session(string Username, string SessionID)
        {
            this.Username = Username;
            this.SessionID = SessionID;
        }

        public Session(string Username, string SessionID, string Version)
            : this(Username, SessionID)
        {
            this.Version = Version;
        }

        public string Username { get; set; }
        public string SessionID { get; set; }
        public string Version { get; set; }
        public string Error { get; set; }
    }
}
