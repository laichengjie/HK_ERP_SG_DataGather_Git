using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Security.Cryptography;
using System.IO;
using System.Text.RegularExpressions;

namespace Gat.Utils
{
    public class SecurityHelper
    {
        private const string defaultKey = "Yuanyuan";
        private static byte[] keys = Encoding.Default.GetBytes("Yuanyuan"); //Keys长度必须为８位

        /// <summary>
        /// DES解密
        /// </summary>
        /// <param name="decryptString">要解密的字符串</param>
        /// <returns></returns>
        public static string DecryptDes(string decryptString)
        {
            return DecryptDes(decryptString, defaultKey);
        }

        /// <summary>
        /// DES解密
        /// </summary>
        /// <param name="decryptString">要解密的字符串</param>
        ///  <param name="decryptKey">密钥，必须为８位?</param>
        /// <returns></returns>
        public static string DecryptDes(string decryptString, string decryptKey)
        {
            //规定密钥只能是数字与字母的组合，并且长度不得小于８位
            Regex regex = new Regex(@"^[0-9a-zA-Z]{8,}");
            if (!regex.IsMatch(decryptKey)) throw new ArgumentOutOfRangeException("decryptKey", "密钥必须由字母或者数字组成，并且长度不得小于８位！");

            try
            {
                using (DESCryptoServiceProvider provider = new DESCryptoServiceProvider())
                {
                    byte[] inputByte = Convert.FromBase64String(decryptString);
                    byte[] inputKey = Encoding.Default.GetBytes(decryptKey.Substring(0, 8));
                    byte[] inputIV = Encoding.Default.GetBytes(decryptKey);

                    using (MemoryStream ms = new MemoryStream())
                    {
                        using (CryptoStream cs = new CryptoStream(ms, provider.CreateDecryptor(keys, inputIV), CryptoStreamMode.Write))
                        {
                            cs.Write(inputByte, 0, inputByte.Length);
                            cs.FlushFinalBlock();
                            return Encoding.Default.GetString(ms.ToArray());
                        }
                    }
                }
            }
            catch (Exception)
            {
                return string.Empty;
            }

        }

        /// <summary>
        /// DES加密字符串
        /// </summary>
        /// <param name="encryptString">要加密的字符串</param>
        /// <returns></returns>
        public static string EncryptDes(string encryptString)
        {
            return EncryptDes(encryptString, defaultKey);
        }

        /// <summary>
        /// DES加密字符串
        /// </summary>
        /// <param name="encryptString">加密的字符串</param>
        /// <returns></returns>
        public static string EncryptDes(string encryptString, string encryptKey)
        {
            //规定密钥只能是数字与字母的组合，并且长度不得小于８位
            Regex regex = new Regex(@"^[0-9a-zA-Z]{8,}");
            if (!regex.IsMatch(encryptKey)) throw new ArgumentOutOfRangeException("decryptKey", "密钥必须由字母或者数字组成，并且长度不得小于８位！");

            try
            {
                using (DESCryptoServiceProvider provider = new DESCryptoServiceProvider())
                {
                    byte[] inputByte = Encoding.Default.GetBytes(encryptString);
                    byte[] inputKey = Encoding.Default.GetBytes(encryptKey.Substring(0, 8));
                    byte[] inputIV = Encoding.Default.GetBytes(encryptKey);

                    using (MemoryStream ms = new MemoryStream())
                    {
                        using (CryptoStream cs = new CryptoStream(ms, provider.CreateEncryptor(keys, inputIV), CryptoStreamMode.Write))
                        {
                            cs.Write(inputByte, 0, inputByte.Length);
                            cs.FlushFinalBlock();
                            return Convert.ToBase64String(ms.ToArray());
                        }
                    }
                }
            }
            catch (Exception)
            {

                return string.Empty;
            }
        }
    }
}
