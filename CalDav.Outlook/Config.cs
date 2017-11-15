using log4net;
using System;
using System.ComponentModel;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Security.Cryptography;
using System.Threading.Tasks;

namespace CalDav.Outlook
{
    [Serializable]
    public class Config : INotifyPropertyChanged
    {
        [NonSerialized]
        private static readonly ILog log = LogManager.GetLogger(typeof(Config));
        [NonSerialized]
        private static Config instance;

        [NonSerialized]
        public static string Directory = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile) + "/AppData/Local/Microsoft/Outlook/";
           
        [NonSerialized]
        private static string path = Directory + "calendar_synchronizer.cfg";

        bool isSync = false;
        int syncTimeSeconds = 10;
        string url;
        string calendar;
        string username;
        string passw;

        #region Implementation of INotifyPropertyChanged

        [field: NonSerialized]
        public event PropertyChangedEventHandler PropertyChanged;

        public void InvokePropertyChanged(PropertyChangedEventArgs e)
        {
            PropertyChanged?.Invoke(this, e);
        }

        #endregion

        static public async Task Serialize()
        {
            MemoryStream ms = new MemoryStream();
            BinaryFormatter formatter = new BinaryFormatter();

            try {
                formatter.Serialize(ms, instance);

                byte[] encryptedData = Encrypt(ms.ToArray());

                File.WriteAllBytes(path, encryptedData);
            }
            catch (SerializationException e) {
                log.Error(e.Message);
                throw;
            }
            finally {
                ms.Close();
            }
        }

        static public void Deserealize()
        {
            if (File.Exists(path)) {
                MemoryStream ms = new MemoryStream();
                BinaryFormatter formatter = new BinaryFormatter();

                try {
                    byte[] data = Decrypt(File.ReadAllBytes(path));

                    ms.Write(data, 0, data.Length);
                    ms.Seek(0, SeekOrigin.Begin);

                    instance = (Config)formatter.Deserialize(ms);
                }
                catch (SerializationException e) {
                    log.Error(e.Message);
                    throw;
                }
                finally {
                    ms.Close();
                }
            }
        }

        static byte[] Encrypt(byte[] data)
        {
            byte[] encryptedData = null;

            try {
                using (RSACryptoServiceProvider rsa = new RSACryptoServiceProvider(3072)) {                    
                    rsa.FromXmlString(RSAKeyProvider.PrivateKey);
                    
                    encryptedData = rsa.Encrypt(data, false);
                }
            }
            catch (Exception ex) {
                log.Error(ex.Message);
            }

            return encryptedData;
        }
        static byte[] Decrypt(byte[] data)
        {
            byte[] decryptedData = null;

            try {
                using (RSACryptoServiceProvider rsa = new RSACryptoServiceProvider(3072)) {
                    rsa.FromXmlString(RSAKeyProvider.PrivateKey);
                    
                    decryptedData = rsa.Decrypt(data, false);
                }
            }
            catch (Exception ex) {
                log.Error(ex.Message);
            }

            return decryptedData;
        }

        public string Url {
            get {
                return url;
            }

            set {
                url = value;
                InvokePropertyChanged(new PropertyChangedEventArgs("Url"));
            }
        }

        public string Calendar {
            get {
                return calendar;
            }

            set {
                calendar = value;
            }
        }

        public bool IsSync {
            get {
                return isSync;
            }

            set {
                isSync = value;
            }
        }

        public string Username {
            get {
                return username;
            }

            set {
                username = value;
                InvokePropertyChanged(new PropertyChangedEventArgs("Username"));
            }
        }

        public string Passw {
            get {
                return passw;
            }

            set {
                passw = value;
                InvokePropertyChanged(new PropertyChangedEventArgs("Passw"));
            }
        }

        public static Config Instance {
            get {
                if (instance == null) {
                    instance = new Config();
                    Deserealize();
                }

                return instance;
            }

            private set {
                instance = value;
            }
        }

        public int SyncTimeSeconds {
            get {
                return syncTimeSeconds;
            }

            set {
                if (value < 10) {
                    throw new ArgumentOutOfRangeException("The synchronization interval cannot be less than 10 seconds");
                }

                syncTimeSeconds = value;

                InvokePropertyChanged(new PropertyChangedEventArgs("SyncTimeSeconds"));
            }
        }
    }
}
