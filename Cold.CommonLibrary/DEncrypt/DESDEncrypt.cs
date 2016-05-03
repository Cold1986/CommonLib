using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace Cold.CommonLibrary
{
    /// <summary>
    /// 使用DES(数据加密算法)对数据进行加密/解密的类
    /// </summary>
    public class DESDEncrypt
    {
        /// <summary>
        /// 加密密钥
        /// </summary>
        public static readonly string SECRET = "L82V6ZVD6J";
        private byte[] sKey;
        private byte[] sIV;
        //public static readonly string SECRET = "0123456789";
        //默认密钥向量
        private static byte[] Keys = { 0x12, 0x34, 0x56, 0x78, 0x90, 0xAB, 0xCD, 0xEF };
        /// <summary>
        /// DES加密字符串
        /// </summary>
        /// <param name="encryptString">待加密的字符串</param>
        /// <param name="encryptKey">加密密钥,要求为8位</param>
        /// <returns>加密成功返回加密后的字符串,失败返回源串</returns>
        public static string Encode(string encryptString, string encryptKey)
        {
            byte[] bKey = Encoding.UTF8.GetBytes(encryptKey.Substring(0, 8));
            byte[] bIV = Keys;
            byte[] bStr = Encoding.UTF8.GetBytes(encryptString);
            try
            {
                DESCryptoServiceProvider desc = new DESCryptoServiceProvider();
                MemoryStream mStream = new MemoryStream();
                CryptoStream cStream = new CryptoStream(mStream, desc.CreateEncryptor(bKey, bIV), CryptoStreamMode.Write);
                cStream.Write(bStr, 0, bStr.Length);
                cStream.FlushFinalBlock();
                return HttpUtility.UrlEncode(Convert.ToBase64String(mStream.ToArray()));
            }
            catch
            {
                return string.Empty;
            }
        }

        /// <summary>
        /// DES解密字符串
        /// </summary>
        /// <param name="decryptString">待解密的字符串</param>
        /// <param name="decryptKey">解密密钥,要求为8位,和加密密钥相同</param>
        /// <returns>解密成功返回解密后的字符串,失败返源串</returns>
        public static string Decode(string decryptString, string decryptKey)
        {
            try
            {
                byte[] bKey = Encoding.UTF8.GetBytes(decryptKey.Substring(0, 8));
                byte[] bIV = Keys;
                byte[] bStr = Convert.FromBase64String(decryptString);
                DESCryptoServiceProvider desc = new DESCryptoServiceProvider();
                MemoryStream mStream = new MemoryStream();
                CryptoStream cStream = new CryptoStream(mStream, desc.CreateDecryptor(bKey, bIV), CryptoStreamMode.Write);
                cStream.Write(bStr, 0, bStr.Length);
                cStream.FlushFinalBlock();
                return Encoding.UTF8.GetString(mStream.ToArray());
            }
            catch
            {
                try
                {
                    decryptString = HttpUtility.UrlDecode(decryptString);
                    byte[] bKey = Encoding.UTF8.GetBytes(decryptKey.Substring(0, 8));
                    byte[] bIV = Keys;
                    byte[] bStr = Convert.FromBase64String(decryptString);
                    DESCryptoServiceProvider desc = new DESCryptoServiceProvider();
                    MemoryStream mStream = new MemoryStream();
                    CryptoStream cStream = new CryptoStream(mStream, desc.CreateDecryptor(bKey, bIV), CryptoStreamMode.Write);
                    cStream.Write(bStr, 0, bStr.Length);
                    cStream.FlushFinalBlock();
                    return Encoding.UTF8.GetString(mStream.ToArray());
                }
                catch
                {
                    return string.Empty;
                }
            }
        }

        /// <summary>
        /// 解密字符串
        /// </summary>
        /// <param name="inputStr">要解密的字符串</param>
        /// <param name="keyStr">密钥</param>
        /// <returns>解密后的结果</returns>
        public static string DecodeString(string inputStr, string keyStr = "")
        {
            DESDEncrypt ws = new DESDEncrypt();
            return ws.DecryptString(inputStr, keyStr);
        }

        /// <summary>
        /// 解密字符串
        /// </summary>
        /// <param name="inputStr">要解密的字符串</param>
        /// <param name="keyStr">密钥</param>
        /// <returns>解密后的结果</returns>
        public string DecryptString(string inputStr, string keyStr)
        {
            try
            {
                DESCryptoServiceProvider des = new DESCryptoServiceProvider();
                if (keyStr == "")
                    keyStr = SECRET;
                byte[] inputByteArray = new byte[inputStr.Length / 2];
                for (int x = 0; x < inputStr.Length / 2; x++)
                {
                    int i = (Convert.ToInt32(inputStr.Substring(x * 2, 2), 16));
                    inputByteArray[x] = (byte)i;
                }
                byte[] keyByteArray = Encoding.Default.GetBytes(keyStr);
                SHA1 ha = new SHA1Managed();
                byte[] hb = ha.ComputeHash(keyByteArray);
                sKey = new byte[8];
                sIV = new byte[8];
                for (int i = 0; i < 8; i++)
                    sKey[i] = hb[i];
                for (int i = 8; i < 16; i++)
                    sIV[i - 8] = hb[i];
                des.Key = sKey;
                des.IV = sIV;
                MemoryStream ms = new MemoryStream();
                CryptoStream cs = new CryptoStream(ms, des.CreateDecryptor(), CryptoStreamMode.Write);
                cs.Write(inputByteArray, 0, inputByteArray.Length);
                cs.FlushFinalBlock();
                StringBuilder ret = new StringBuilder();
                return System.Text.Encoding.Default.GetString(ms.ToArray());
            }
            catch
            {
                return "";
            }
        }

        /// <summary>
        /// 加密字符串
        /// </summary>
        /// <param name="inputStr">输入字符串</param>
        /// <param name="keyStr">密码，可以为“”</param>
        /// <returns>输出加密后字符串</returns>
        public static string EncodeString(string inputStr, string keyStr = "")
        {
            DESDEncrypt ws = new DESDEncrypt();
            return ws.EncryptString(inputStr, keyStr);
        }

        /// <summary>
        /// 加密字符串
        /// </summary>
        /// <param name="inputStr">输入字符串</param>
        /// <param name="keyStr">密码，可以为“”</param>
        /// <returns>输出加密后字符串</returns>
        public string EncryptString(string inputStr, string keyStr)
        {
            DESCryptoServiceProvider des = new DESCryptoServiceProvider();
            if (keyStr == "")
                keyStr = SECRET;
            byte[] inputByteArray = Encoding.Default.GetBytes(inputStr);
            byte[] keyByteArray = Encoding.Default.GetBytes(keyStr);
            SHA1 ha = new SHA1Managed();
            byte[] hb = ha.ComputeHash(keyByteArray);
            sKey = new byte[8];
            sIV = new byte[8];
            for (int i = 0; i < 8; i++)
                sKey[i] = hb[i];
            for (int i = 8; i < 16; i++)
                sIV[i - 8] = hb[i];
            des.Key = sKey;
            des.IV = sIV;
            MemoryStream ms = new MemoryStream();
            CryptoStream cs = new CryptoStream(ms, des.CreateEncryptor(), CryptoStreamMode.Write);
            cs.Write(inputByteArray, 0, inputByteArray.Length);
            cs.FlushFinalBlock();
            StringBuilder ret = new StringBuilder();
            foreach (byte b in ms.ToArray())
            {
                ret.AppendFormat("{0:X2}", b);
            }
            cs.Close();
            ms.Close();
            return ret.ToString();
        }
    }
}
