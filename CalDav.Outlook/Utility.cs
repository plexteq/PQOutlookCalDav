using log4net;
using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace CalDav.Outlook
{
    public class Utility
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(Utility));

        public static byte[] Serialize(object obj)
        {
            MemoryStream ms = new MemoryStream();

            try {
                BinaryFormatter formatter = new BinaryFormatter();
                formatter.Serialize(ms, obj);

                return ms.ToArray();
            }
            catch (Exception ex) {
                log.Error(ex.Message);
            }
            finally {
                ms.Close();
            }

            return null;
        }
        public static object Deserialize(byte[] obj)
        {
            MemoryStream ms = new MemoryStream();

            try {
                BinaryFormatter formatter = new BinaryFormatter();

                ms.Write(obj, 0, obj.Length);
                ms.Seek(0, SeekOrigin.Begin);

                return formatter.Deserialize(ms);
            }
            catch (Exception ex) {
                log.Error(ex.Message);
            }
            finally {
                ms.Close();
            }

            return null;
        }

    }
}
