using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;


namespace Common
{
    public class CommonHelper
    {
        public static string WebCatalog()
        {
            string path = HttpRuntime.AppDomainAppVirtualPath;
            if (path.Length > 1)
            {
                return path;
            }
            else
            {
                return ServerInfo.AppPath; 
            }

        }

        /// <summary>
        /// 获取本机IP
        /// </summary>
        /// <returns></returns>
        public static string GetLocalIPAddress()
        {
            ///获取本地的IP地址
            string AddressIP = string.Empty;
            var addressArray = Dns.GetHostEntry(Dns.GetHostName()).AddressList;
            if (addressArray == null || addressArray.Count() == 0)
            {
                return string.Empty;
            }
            var address = addressArray.FirstOrDefault(a => a.AddressFamily.ToString() == "InterNetwork");
            if (address == null)
            {
                return string.Empty;
            }
            return address.ToString();
        }

        /// <summary>
        ///     MD5函数 转化成大写
        /// </summary>
        /// <param name="str">原始字符串</param>
        /// <param name="toLower">是否转换小写</param>
        /// <returns>MD5结果</returns>
        public static string Md5(string str, bool toLower = false)
        {
            byte[] b = Encoding.Default.GetBytes(str);
            b = new MD5CryptoServiceProvider().ComputeHash(b);
            string ret = "";
            for (int i = 0; i < b.Length; i++)
                ret += b[i].ToString("x").PadLeft(2, '0');
            if (toLower)
            {
                return ret;
            }
            return ret.ToUpper();
        }

        /// <summary>
        /// MD5函数 默认不转化大小写
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static string Md5Default(string str)
        {
            return Md5(str, true);
        }

        /// <summary>
        /// MD5加密
        /// </summary>
        /// <param name="input">待计算MD5哈希值的输入字符串</param>
        /// <param name="charset">输入字符串的字符集</param>
        /// <returns>输入字符串的MD5哈希值</returns>
        public static string GetMd5(string input)
        {
            MD5 md5 = new MD5CryptoServiceProvider();
            byte[] data = md5.ComputeHash(Encoding.GetEncoding("utf-8").GetBytes(input));
            StringBuilder builder = new StringBuilder(32);
            for (int i = 0; i < data.Length; i++)
            {
                builder.Append(data[i].ToString("x2"));
            }
            return builder.ToString();
        }

        /// <summary> 
        /// 加密数据 
        /// </summary> 
        /// <param name="Text"></param> 
        /// <param name="sKey"></param> 
        /// <returns></returns> 
        public static string DesEncrypt(string Text, string sKey)
        {
            DESCryptoServiceProvider des = new DESCryptoServiceProvider();
            byte[] inputByteArray;
            inputByteArray = Encoding.Default.GetBytes(Text);
            des.Key = ASCIIEncoding.ASCII.GetBytes(sKey);
            des.IV = ASCIIEncoding.ASCII.GetBytes(sKey);
            System.IO.MemoryStream ms = new System.IO.MemoryStream();
            des.Mode = CipherMode.CBC;

            CryptoStream cs = new CryptoStream(ms, des.CreateEncryptor(), CryptoStreamMode.Write);
            cs.Write(inputByteArray, 0, inputByteArray.Length);
            cs.FlushFinalBlock();
            StringBuilder ret = new StringBuilder();
            foreach (byte b in ms.ToArray())
            {
                ret.AppendFormat("{0:x2}", b);
            }
            return ret.ToString();
        }

        /// <summary> 
        /// 解密数据 
        /// </summary> 
        /// <param name="Text"></param> 
        /// <param name="sKey"></param> 
        /// <returns></returns> 
        public static string DesDecrypt(string Text, string sKey)
        {
            DESCryptoServiceProvider des = new DESCryptoServiceProvider();
            int len;
            len = Text.Length / 2;
            byte[] inputByteArray = new byte[len];
            int x, i;
            for (x = 0; x < len; x++)
            {
                i = Convert.ToInt32(Text.Substring(x * 2, 2), 16);
                inputByteArray[x] = (byte)i;
            }
            des.Key = ASCIIEncoding.ASCII.GetBytes(sKey);
            des.IV = ASCIIEncoding.ASCII.GetBytes(sKey);
            System.IO.MemoryStream ms = new System.IO.MemoryStream();
            CryptoStream cs = new CryptoStream(ms, des.CreateDecryptor(), CryptoStreamMode.Write);
            cs.Write(inputByteArray, 0, inputByteArray.Length);
            cs.FlushFinalBlock();
            return Encoding.Default.GetString(ms.ToArray());
        }

        #region 可逆加密
        /// <summary>
        /// 加密——秘钥必须8位
        /// </summary>
        /// <param name="data"></param>
        /// <param name="KEYStr"></param>
        /// <param name="IVStr"></param>
        /// <returns></returns>
        public static string Encode(string data, string KEYStr = "qnet9ew!", string IVStr = "56431212")
        {
            byte[] byKey = System.Text.ASCIIEncoding.ASCII.GetBytes(KEYStr);
            byte[] byIV = System.Text.ASCIIEncoding.ASCII.GetBytes(IVStr);

            DESCryptoServiceProvider cryptoProvider = new DESCryptoServiceProvider();
            int i = cryptoProvider.KeySize;
            MemoryStream ms = new MemoryStream();
            CryptoStream cst = new CryptoStream(ms, cryptoProvider.CreateEncryptor(byKey, byIV), CryptoStreamMode.Write);

            StreamWriter sw = new StreamWriter(cst);
            sw.Write(data);
            sw.Flush();
            cst.FlushFinalBlock();
            sw.Flush();
            return Convert.ToBase64String(ms.GetBuffer(), 0, (int)ms.Length);

        }

        /// <summary>
        /// 解密——秘钥必须8位
        /// </summary>
        /// <param name="data"></param>
        /// <param name="KEYStr"></param>
        /// <param name="IVStr"></param>
        /// <returns></returns>
        public static string Decode(string data, string KEYStr = "qnet9ew!", string IVStr = "56431212")
        {
            byte[] byKey = System.Text.ASCIIEncoding.ASCII.GetBytes(KEYStr);
            byte[] byIV = System.Text.ASCIIEncoding.ASCII.GetBytes(IVStr);

            byte[] byEnc;
            try
            {
                byEnc = Convert.FromBase64String(data);
            }
            catch
            {
                return null;
            }

            DESCryptoServiceProvider cryptoProvider = new DESCryptoServiceProvider();
            MemoryStream ms = new MemoryStream(byEnc);
            CryptoStream cst = new CryptoStream(ms, cryptoProvider.CreateDecryptor(byKey, byIV), CryptoStreamMode.Read);
            StreamReader sr = new StreamReader(cst);
            return sr.ReadToEnd();
        }
        #endregion

        #region desc加密

        /// <summary>
        /// DES加密
        /// </summary>
        /// <param name="strString"></param>
        /// <param name="strKey"></param>
        /// <returns></returns>
        public static string DES3Encrypt(string strString, string strKey)
        {
            if (string.IsNullOrEmpty(strString) || string.IsNullOrEmpty(strKey))
            {
                return string.Empty;
            }
            var des = new TripleDESCryptoServiceProvider();
            var hashMd5 = new MD5CryptoServiceProvider();
            des.Key = hashMd5.ComputeHash(Encoding.Default.GetBytes(strKey));
            des.Mode = CipherMode.ECB;
            var desEncrypt = des.CreateEncryptor();
            var buffer = Encoding.Default.GetBytes(strString);
            return Convert.ToBase64String(desEncrypt.TransformFinalBlock(buffer, 0, buffer.Length));
        }

        /// <summary>
        /// DES加密JSON UTF8
        /// </summary>
        /// <param name="strString"></param>
        /// <param name="strKey"></param>
        /// <returns></returns>
        public static string DES3EncryptUtf8(string strString, string strKey)
        {
            if (string.IsNullOrEmpty(strString) || string.IsNullOrEmpty(strKey))
            {
                return string.Empty;
            }
            var des = new TripleDESCryptoServiceProvider();
            var hashMd5 = new MD5CryptoServiceProvider();
            des.Key = hashMd5.ComputeHash(Encoding.UTF8.GetBytes(strKey));
            des.Mode = CipherMode.ECB;
            var desEncrypt = des.CreateEncryptor();
            var buffer = Encoding.UTF8.GetBytes(strString);
            return Convert.ToBase64String(desEncrypt.TransformFinalBlock(buffer, 0, buffer.Length));
        }

        /// <summary>
        /// DES解密
        /// </summary>
        /// <param name="strString"></param>
        /// <param name="strKey"></param>
        /// <returns></returns>
        public static string DES3DecryptUtf8(string strString, string strKey)
        {
            var des = new TripleDESCryptoServiceProvider();
            var hashMd5 = new MD5CryptoServiceProvider();
            des.Key = hashMd5.ComputeHash(Encoding.UTF8.GetBytes(strKey));
            des.Mode = CipherMode.ECB;
            var desDecrypt = des.CreateDecryptor();
            string result;
            try
            {
                var buffer = Convert.FromBase64String(strString);
                result = Encoding.UTF8.GetString(desDecrypt.TransformFinalBlock(buffer, 0, buffer.Length));
            }
            catch
            {
                return "";
            }
            return result;
        }

        /// <summary>
        /// DES解密
        /// </summary>
        /// <param name="strString"></param>
        /// <param name="strKey"></param>
        /// <returns></returns>
        public static string DES3Decrypt(string strString, string strKey)
        {
            var des = new TripleDESCryptoServiceProvider();
            var hashMd5 = new MD5CryptoServiceProvider();
            des.Key = hashMd5.ComputeHash(Encoding.Default.GetBytes(strKey));
            des.Mode = CipherMode.ECB;
            var desDecrypt = des.CreateDecryptor();
            string result;
            try
            {
                var buffer = Convert.FromBase64String(strString);
                result = Encoding.Default.GetString(desDecrypt.TransformFinalBlock(buffer, 0, buffer.Length));
            }
            catch
            {
                return "";
            }
            return result;
        }

        #endregion

        /// <summary>
        /// 获取DataTable中的列名
        /// </summary>
        /// <param name="dt">表</param>
        /// <param name="removeList">删除列名</param>
        /// <param name="replaceColumnName">替换的列名</param>
        /// <returns> </returns>
        public static List<string> GetTableHeadList(ref DataTable dt, List<string> removeList = null, Dictionary<string, string> replaceColumnName = null)
        {
            var listHead = new List<string>();
            if (dt == null)
            {
                return listHead;
            }

            if (removeList != null && removeList.Any())
            {
                foreach (string removeCol in removeList)
                {
                    if (dt.Columns.Contains(removeCol.Trim()))
                    {
                        dt.Columns.Remove(removeCol);
                    }
                }
            }

            foreach (DataColumn cl in dt.Columns)
            {
                if (replaceColumnName != null && replaceColumnName.ContainsKey(cl.ColumnName))
                {
                    listHead.Add(replaceColumnName[cl.ColumnName]);
                }
                else
                {
                    listHead.Add(cl.ColumnName);
                }
            }
            return listHead;
        }


        public static bool CheckIsImage(string ext)
        {
            if (ext.Equals("jpg", StringComparison.CurrentCultureIgnoreCase)
                || ext.Equals("gif", StringComparison.CurrentCultureIgnoreCase)
                || ext.Equals("png", StringComparison.CurrentCultureIgnoreCase)
                || ext.Equals("bmp", StringComparison.CurrentCultureIgnoreCase))
            {
                return true;
            }
            return false;
        }

        /// <summary>
        /// 是否正确格式邮箱
        /// </summary>
        /// <param name="emailStr"></param>
        /// <returns></returns>
        public static bool IsEmail(string emailStr)
        {
            return Regex.IsMatch(emailStr, @"^([\w-\.]+)@((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.)|(([\w-]+\.)+))([a-zA-Z]{2,4}|[0-9]{1,3})(\]?)$");
        }

        /// <summary>
        /// 是否正确格式电话号
        /// </summary>
        /// <param name="phoneNumber"></param>
        /// <returns></returns>
        public static bool IsTelephone(string phoneNumber)
        {
            return Regex.IsMatch(phoneNumber, @"^(\d{3,4}-)?\d{6,8}$");
        }


        /// <summary>
        /// 判断是否是手机号-只判断11位数字
        /// </summary>
        /// <param name="val"></param>
        /// <returns></returns>
        public static bool IsPhone(string val)
        {
            Regex rCode = new Regex(@"^\d{11}$");
            if (!rCode.IsMatch(val))
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        /// <summary>
        /// 是否正确格式手机号
        /// </summary>
        /// <param name="str_handset"></param>
        /// <returns></returns>
        public static bool IsMobile(string mobileNumber)
        {
            return Regex.IsMatch(mobileNumber, @"^[1]+[3,4,5,7,8]+\d{9}");
        }

        public static string GetExtDiv(string ext)
        {
            switch (ext.ToUpper())
            {
                case "XLS":
                case "XLSX":
                    return "ext_xls";
                case "DOC":
                case "DOCX":
                    return "ext_doc";
                case "PPT":
                case "PPTX":
                    return "ext_ppt";
                case "PDF":
                    return "ext_pdf";
                case "ZIP":
                case "RAR":
                    return "ext_zip";
                default:
                    return "ext_file";
            }
        }

        /// <summary>
        /// 获取最终字符串（排除DBNull，null，string.Empty 或空值后的真实值） 
        /// 注：如果为DBNull，null，string.Empty 或空值，则返回string.Empty (add by LiYundong at 2010-1-14)
        /// </summary>
        /// <param name="objString"></param>
        /// <returns></returns>
        public static string FinalString(object objString)
        {
            if (!IsDBNullOrNullOrEmptyString(objString))
                return objString.ToString();
            else
                return string.Empty;
        }

        /// <summary>
        /// DataRow的value或从数据库中取出的Object型数据验证,验证取出的object是否是DBNull,空或null]
        /// 如果是DBNull,null或空字符串则返回true
        /// </summary>
        /// <param name="objSource">待验证的object</param>
        /// <returns>
        /// 如果是DBNull,null或空字符串则返回true
        /// </returns>
        public static bool IsDBNullOrNullOrEmptyString(object objSource)
        {
            if ((objSource == DBNull.Value) || (objSource == null))
                return true;
            string strSource = objSource.ToString();
            if (strSource.Trim() == string.Empty)
                return true;
            return false;
        }

        /// <summary>
        /// 转换成整型数字，
        /// 转换失败则返回由nDefault指定的数字
        /// 转换成功则返回真实转换的数字
        /// </summary>
        /// <param name="strInfo">待转换的字符串</param>
        /// <param name="nDefault">指定传回的默认值</param>
        /// <returns>转换失败则返回由nDefault指定的数字;转换成功则返回真实转换的数字</returns>
        /// <remarks>
        /// edited by 王纪虎 at 2011-8-16 for 修改转换类型为Number，避免负数转换失败
        /// </remarks>
        public static int ToInt(string strInfo, int nDefault)
        {
            return ToInt(strInfo, nDefault, NumberStyles.Number);
        }

        /// <summary>
        /// 转换成整型数字，数字样式由numStyle指定
        /// 转换失败则返回由nDefault指定的数字
        /// 转换成功则返回真实转换的数字
        /// </summary>
        /// <param name="strInfo">待转换的字符串</param>
        /// <param name="nDefault">指定传回的默认值</param>
        /// <returns>转换失败则返回由nDefault指定的数字;转换成功则返回真实转换的数字</returns>
        public static int ToInt(string strInfo, int nDefault, NumberStyles numStyle)
        {
            string strRealInfo = null;
            if (IsNullOrEmptyString(strInfo, out strRealInfo))
                return nDefault;
            int nResult = 0;
            if (int.TryParse(strRealInfo, numStyle, null, out nResult))
                return nResult;
            else
                return nDefault;
        }

        /// <summary>
        /// 验证是否是空或null字符串
        /// 如果是空或null则返回true,strRealString为null或string.Empty
        /// 否则返回false,strRealString为经过Trim操作的String;
        /// </summary>
        /// <param name="strSource">待查看的string</param>
        /// <param name="strRealString">经过Trim操作的string</param>
        /// <returns>
        /// 如果是空或null则返回true,strRealString为null或string.Empty
        /// 否则返回false,strRealString为经过Trim操作的String;
        /// </returns>
        public static bool IsNullOrEmptyString(string strSource, out string strRealString)
        {
            strRealString = null;
            if (strSource == null)
                return true;
            strRealString = strSource.Trim();
            if (strRealString == string.Empty)
                return true;
            return false;
        }

        /// <summary>
        /// 截取字符串
        /// </summary>
        /// <param name="str"></param>
        /// <param name="len"></param>
        /// <returns></returns>
        public static string Substr(string str, int len)
        {
            if (!string.IsNullOrEmpty(str))
            {
                System.Text.RegularExpressions.Regex rChinese = new Regex(@"[\u4e00-\u9fa5]"); //验证中文
                Regex rEnglish = new Regex(@"^[A-Za-z0-9]+$");  //验证字母
                if (rChinese.IsMatch(str))
                {
                    //中文
                    return (str.Length > len) ? str.Substring(0, len) + "..." : str; ;
                }
                else if (rEnglish.IsMatch(str))
                {
                    //英文
                    return (str.Length > len * 2) ? str.Substring(0, len * 2) + "..." : str; ;
                }
                return (str.Length > len) ? str.Substring(0, len) + "..." : str; ;
            }
            return "";
        }

       

        /// <summary>
        /// 根据实体的属性，获得两个集合的交集（用法示例：GetIntersect(e1, e2, m => m.Id)）
        /// </summary>
        /// <typeparam name="T">实体类</typeparam>
        /// <typeparam name="V">比较的属性</typeparam>
        /// <param name="e1">第一个集合</param>
        /// <param name="e2">第二个集合</param>
        /// <param name="keySelector"></param>
        /// <returns>交集</returns>
        public static IEnumerable<T> GetIntersect<T, V>(IEnumerable<T> e1, IEnumerable<T> e2, Func<T, V> keySelector)
        {
            return e1.Intersect(e2, Equality<T>.CreateComparer(keySelector));
        }

        /// <summary>
        /// 根据实体的属性，获得两个集合的差集（用法示例：GetExcept(e1, e2, m => m.Id)）
        /// </summary>
        /// <typeparam name="T">实体类</typeparam>
        /// <typeparam name="V">比较的属性</typeparam>
        /// <param name="e1">第一个集合</param>
        /// <param name="e2">第二个集合</param>
        /// <param name="keySelector"></param>
        /// <returns>第一个集合与第二个集合的差集</returns>
        public static IEnumerable<T> GetExcept<T, V>(IEnumerable<T> e1, IEnumerable<T> e2, Func<T, V> keySelector)
        {
            return e1.Except(e2, Equality<T>.CreateComparer(keySelector));
        }

        #region 将数组去重复,转为List
        /// <summary>
        /// 将数组去重复,转为List
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="arr"></param>
        /// <returns></returns>
        public static List<T> DistinctArray<T>(object[] arr)
        {
            if (arr == null || arr.Length == 0)
            {
                return null;
            }

            var list = new List<T>();
            foreach (var item in arr)
            {
                if (list.Contains((T)item))
                {
                    continue;
                }
                else
                {
                    list.Add((T)item);
                }
            }

            return list;
        }
        #endregion

        /// <summary>
        /// 把字符串里包含的特殊字符替换成想要的字符。
        /// </summary>
        /// <param name="str">字符串</param>
        /// <param name="type">想替换成的字符</param>
        /// <returns></returns>
        public static string ReplaceStr(string str, string type)
        {
            if (!string.IsNullOrEmpty(str))
            {
                if (str.Contains("，"))//中文逗号
                {
                    str = str.Replace("，", type.Trim());
                }
                if (str.Contains("!"))//英文下的叹号
                {
                    str = str.Replace("!", type.Trim());
                }
                if (str.Contains("！"))//中文下的叹号
                {
                    str = str.Replace("！", type);
                }
                if (str.Contains("~"))
                {
                    str = str.Replace("~", type);
                }
                if (str.Contains("@"))
                {
                    str = str.Replace("@", type);
                }
                if (str.Contains("#"))
                {
                    str = str.Replace("#", type);
                }
                if (str.Contains("$"))
                {
                    str = str.Replace("$", type);
                }
                if (str.Contains("%"))
                {
                    str = str.Replace("%", type);
                }
                if (str.Contains("^"))
                {
                    str = str.Replace("^", type);
                }
                if (str.Contains("&"))
                {
                    str = str.Replace("&", type);
                }
                if (str.Contains("*"))
                {
                    str = str.Replace("*", type);
                }
                if (str.Contains("?"))
                {
                    str = str.Replace("?", type);
                }
                if (str.Contains(":"))//英文下的冒号
                {
                    str = str.Replace(":", type);
                }
                if (str.Contains("："))//中文下的冒号
                {
                    str = str.Replace("：", type);
                }
                if (str.Contains(";"))//英文下的分号
                {
                    str = str.Replace(";", type);
                }
                if (str.Contains("；"))//中文下的分号
                {
                    str = str.Replace("；", type);
                }
            }
            return str;
        }

        /// <summary>
        /// 把DateTime转换成yyyyMMdd格式
        /// </summary>
        /// <param name="dateTime"></param>
        /// <returns></returns>
        public static int ConvertDateToInt(DateTime dateTime)
        {
            return Convert.ToInt32(dateTime.ToString("yyyyMMdd"));
        }

        /// <summary>
        /// 把yyyyMMdd转换成DateTime类型
        /// </summary>
        /// <param name="dateTime"></param>
        /// <returns></returns>
        public static DateTime ConvertIntToDate(int dateTime)
        {
            DateTime date = DateTime.ParseExact(dateTime.ToString(), "yyyyMMdd", null, System.Globalization.DateTimeStyles.AllowWhiteSpaces);

            return date;
        }

     

        /// <summary>
        /// 描述：将base64转成图片
        /// 创建标识：瞿玲莉24592
        /// 创建时间：2016-08-29
        /// </summary>
        /// <param name="base64">base64</param>
        /// <param name="fileName">保存图片的文件名</param>
        /// <returns></returns>
        public static string Base64ToImageForSave(string base64, string fileName)
        {
            string byteStr = base64;
            int delLength = byteStr.IndexOf(',') + 1;
            string str = byteStr.Substring(delLength, byteStr.Length - delLength);
            byte[] arr = Convert.FromBase64String(str);
            var url = "";//TODO
            return url;
        }

     

        /// <summary>
        /// 拷贝对象
        /// </summary>
        /// <param name="origin">原始的</param>
        /// <param name="target">目标对象</param>
        public static void CopyValue(object origin, object target)
        {
            System.Reflection.PropertyInfo[] properties = (target.GetType()).GetProperties();
            System.Reflection.FieldInfo[] fields = (origin.GetType()).GetFields();
            for (int i = 0; i < fields.Length; i++)
            {
                for (int j = 0; j < properties.Length; j++)
                {
                    if (fields[i].Name == properties[j].Name && properties[j].CanWrite)
                    {
                        properties[j].SetValue(target, fields[i].GetValue(origin), null);
                    }
                }
            }
        }
    }

    internal static class Equality<T>
    {
        public static IEqualityComparer<T> CreateComparer<V>(Func<T, V> keySelector)
        {
            return new CommonEqualityComparer<V>(keySelector);
        }

        private class CommonEqualityComparer<V> : IEqualityComparer<T>
        {
            private Func<T, V> keySelector;
            private IEqualityComparer<V> comparer;

            public CommonEqualityComparer(Func<T, V> keySelector)
            {
                this.keySelector = keySelector;
                this.comparer = EqualityComparer<V>.Default;
            }

            public bool Equals(T x, T y)
            {
                return comparer.Equals(keySelector(x), keySelector(y));
            }

            public int GetHashCode(T obj)
            {
                return comparer.GetHashCode(keySelector(obj));
            }
        }
    }

    /// <summary>
    /// 描述：判断是否有权限请求实体
    /// </summary>
    public class IsHasPermissionRequest
    {
        /// <summary>
        /// 用户Id
        /// </summary>
        public string UserId { get; set; }
        /// <summary>
        /// 工号
        /// </summary>
        public string TcNum { get; set; }
        /// <summary>
        /// 统一权限Code
        /// </summary>
        public string Code { get; set; }
    }


}