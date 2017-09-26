using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace Common
{
    public class EncryptHelper
    {
        private const string EncryptKey = "!@-897Wh";

        /// <summary>
        /// 描述：des加密
        /// 创建标识:cpf
        /// 创建时间：2017-9-20 11:12:31
        /// </summary>
        /// <param name="pToEncrypt">需要加密的字符串</param>
        /// <returns></returns>
        public static string DesEncrypt(string pToEncrypt)
        {
            using (DESCryptoServiceProvider des = new DESCryptoServiceProvider())
            {
                byte[] inputByteArray = Encoding.UTF8.GetBytes(pToEncrypt);
                des.Key = ASCIIEncoding.ASCII.GetBytes(EncryptKey);
                des.IV = ASCIIEncoding.ASCII.GetBytes(EncryptKey);
                System.IO.MemoryStream ms = new System.IO.MemoryStream();
                using (CryptoStream cs = new CryptoStream(ms, des.CreateEncryptor(), CryptoStreamMode.Write))
                {
                    cs.Write(inputByteArray, 0, inputByteArray.Length);
                    cs.FlushFinalBlock();
                    cs.Close();
                }
                string str = Convert.ToBase64String(ms.ToArray());
                ms.Close();
                return str;
            }
        }

        /// <summary>
        /// 描述：des解密
        /// 创建标识：cpf
        /// 创建时间：2017-9-20 11:14:33
        /// </summary>
        /// <param name="pToDecrypt"></param>
        /// <returns></returns>
        public static string DesDecrypt(string pToDecrypt)
        {
            byte[] inputByteArray = Convert.FromBase64String(pToDecrypt);
            using (DESCryptoServiceProvider des = new DESCryptoServiceProvider())
            {
                des.Key = ASCIIEncoding.ASCII.GetBytes(EncryptKey);
                des.IV = ASCIIEncoding.ASCII.GetBytes(EncryptKey);
                System.IO.MemoryStream ms = new System.IO.MemoryStream();
                using (CryptoStream cs = new CryptoStream(ms, des.CreateDecryptor(), CryptoStreamMode.Write))
                {
                    cs.Write(inputByteArray, 0, inputByteArray.Length);
                    cs.FlushFinalBlock();
                    cs.Close();
                }
                string str = Encoding.UTF8.GetString(ms.ToArray());
                ms.Close();
                return str;
            }
        }
    }
}
