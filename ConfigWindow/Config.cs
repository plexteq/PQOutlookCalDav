using System;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Threading.Tasks;

namespace CalCli.UI
{
    [Serializable]
    public class Config
    {
        [NonSerialized]
        static Config instance;

        [NonSerialized]
        static string path = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile) + "/AppData/Local/Microsoft/Outlook/calendar_synchronizer.cfg";

        bool isSync = false;
        string url;
        string calendar;
        string username;
        string passw;

        int syncTimeMinutes;
        
        public Config()
        {
            
        }

        static public async Task Serialize()
        {
            MemoryStream ms = new MemoryStream();
            BinaryFormatter formatter = new BinaryFormatter();

            try
            {
                formatter.Serialize(ms, instance);

                byte[] encryptedData = Encrypt(ms.ToArray());

                File.WriteAllBytes(path, encryptedData);
            }
            catch (SerializationException e)
            {
                Console.WriteLine("Failed to serialize. Reason: " + e.Message);
                throw;
            }
            finally
            {
                ms.Close();
            }
        }

        static public void Deserealize()
        {
            if (File.Exists(path))
            {
                MemoryStream ms = new MemoryStream();
                BinaryFormatter formatter = new BinaryFormatter();

                try
                {
                    byte[] data = Decrypt(File.ReadAllBytes(path));

                    ms.Write(data, 0, data.Length);
                    ms.Seek(0, SeekOrigin.Begin);

                    instance = (Config)formatter.Deserialize(ms);
                }
                catch (SerializationException e)
                {
                    Console.WriteLine("Failed to serialize. Reason: " + e.Message);
                    throw;
                }
                finally
                {
                    ms.Close();
                }
            }
        }

        static byte[] Encrypt(byte[] data)
        {
            //TODO: implement encryption
            return data;
        }
        static byte[] Decrypt(byte[] data)
        {
            //TODO: implement decryption
            return data;
        }

        public string Url
        {
            get
            {
                return url;
            }

            set
            {
                url = value;
            }
        }

        public string Calendar
        {
            get
            {
                return calendar;
            }

            set
            {
                calendar = value;
            }
        }

        public bool IsSync
        {
            get
            {
                return isSync;
            }

            set
            {
                isSync = value;
            }
        }

        public string Username
        {
            get
            {
                return username;
            }

            set
            {
                username = value;
            }
        }

        public string Passw
        {
            get
            {
                return passw;
            }

            set
            {
                passw = value;
            }
        }

        public static Config Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new Config();
                    Deserealize();
                }

                return instance;
            }

            private set
            {
                instance = value;
            }
        }

        public int SyncTimeMinutes
        {
            get
            {
                return syncTimeMinutes;
            }

            set
            {
                if(value < 0)
                {
                    throw new ArgumentOutOfRangeException("The synchronization interval cannot be less than 0");
                }

                syncTimeMinutes = value;
            }
        }
    }
}
