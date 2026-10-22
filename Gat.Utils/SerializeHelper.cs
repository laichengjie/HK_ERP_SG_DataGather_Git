using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using System.Security.Cryptography;

namespace Gat.Utils
{
    public class SerializeHelper
    {
        /// <summary>
        /// 采用二进制序列化指定对象
        /// </summary>
        /// <param name="theObject">指定对象</param>
        /// <returns></returns>
        public static string BinarySerialize(object theObject)
        {
            string strObject = string.Empty;
            BinaryFormatter ser = new BinaryFormatter();
            using (MemoryStream ms = new MemoryStream())
            {
                ser.Serialize(ms, theObject);
                strObject = Convert.ToBase64String(ms.ToArray());
                ms.Close();
                ser = null;
            }
            return strObject;
        }


        /// <summary>
        /// 采用二进制反序列化指定对象
        /// </summary>
        /// <param name="theObject">指定对象</param>
        /// <returns></returns>
        public static object DeserializeFromBinary(byte[] buffer)
        {
            object objData = null;
            using (MemoryStream ms = new MemoryStream(buffer))
            {
                BinaryFormatter ser = new BinaryFormatter();
                objData = ser.Deserialize(new MemoryStream(buffer));
                ms.Close();
                ser = null;
            }

            return objData;
        }

        /// <summary>
        /// 采用二进制反序列化指定对象
        /// </summary>
        /// <param name="theObject">指定对象</param>
        /// <returns></returns>
        public static object DeserializeFromBinary(string s)
        {
            byte[] buffer = Convert.FromBase64String(s);
            object objData = null;
            using (MemoryStream ms = new MemoryStream(buffer))
            {
                BinaryFormatter ser = new BinaryFormatter();
                objData = ser.Deserialize(new MemoryStream(buffer));
                ms.Close();
                ser = null;
            }

            return objData;
        }
    }
}
