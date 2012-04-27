using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Xml.Serialization;

namespace tournament
{
    public static class authData
    {
        private static readonly List<authInfo> _validLogins = new List<authInfo>(10);
        private static readonly XmlSerializer _ser = new XmlSerializer(typeof(List<authInfo>));
        private static readonly System.Text.ASCIIEncoding _ascii = new System.Text.ASCIIEncoding();

        static authData()
        {
            try
            {
                using (Stream fileStream = File.OpenRead("logons.xml"))
                {
                    _validLogins = (List<authInfo>) _ser.Deserialize(fileStream);
                }

            }
            catch (Exception)
            {
                // Use the defaults if no file can be loaded.
                _validLogins = new List<authInfo>();
                _validLogins.Add(new authInfo() { username = "a@b.com", password = hash("pass") });

                saveData();
            }
            _validLogins.Add(new authInfo() { username = "c@d.com", password = hash("pass") });

        }

        private static void saveData()
        {
            lock(_validLogins)
            {
                using (Stream fileStream = File.Create("logons.xml"))
                {
                    _ser.Serialize(fileStream, _validLogins);
                }
            }
        }

        public static bool authUser(string usename, string pass)
        {
            lock (_validLogins)
            {
                return _validLogins.Count(x => x.username == usename && x.password == hash(pass)) > 0;
            }
        }

        public static void setPass(string username, string newPass)
        {
            lock (_validLogins)
            {
                _validLogins.Single(x => x.username == username).password = hash(newPass);

                saveData();
            }
        }

        private static string hash(string toHash)
        {
            SHA1 hash = SHA1.Create();

            hash.ComputeHash(_ascii.GetBytes(toHash));

            return Convert.ToBase64String(hash.Hash);
        }
    }
}