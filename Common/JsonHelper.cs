using System;
using System.IO;
using System.Xml;
using System.Text;
using System.Web.Mvc;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Runtime.Serialization.Json;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Converters;
using Common.Costant;

namespace Common
{
    public static class JsonHelper
    {
        public static string Serializer<T>(T obj)
        {
            DataContractJsonSerializer json = new DataContractJsonSerializer(obj.GetType());
            using (MemoryStream stream = new MemoryStream())
            {
                json.WriteObject(stream, obj);
                string szJson = Encoding.UTF8.GetString(stream.ToArray());
                szJson = ConvertToDateTimeString(szJson);
                return szJson;
            }
        }

        /// <summary>
        /// 将"\/Date(673286400000+0800)\/"Json时间格式替换"yyyy-MM-dd HH:mm:ss"格式的字符串
        /// </summary>
        /// <param name="jsonDateTimeString">"\/Date(673286400000+0800)\/"Json时间格式</param>
        /// <returns></returns>
        private static string ConvertToDateTimeString(string jsonDateTimeString)
        {
            string result = string.Empty;
            string p = @"\\/Date\((\d+)\+\d+\)\\/";
            MatchEvaluator matchEvaluator = new MatchEvaluator(ConvertJsonDateToDateString);
            Regex reg = new Regex(p);
            result = reg.Replace(jsonDateTimeString, matchEvaluator);
            return result.Replace(@"\/Date(-2209017600000+0800)\/", DateContent.DefaultDateTimeSeconds);
        }

        private static string ConvertJsonDateToDateString(Match match)
        {
            string result = string.Empty;
            DateTime dt = new DateTime(1970, 1, 1);
            dt = dt.AddMilliseconds(long.Parse(match.Groups[1].Value));
            dt = dt.ToLocalTime();
            result = dt.ToString(DateContent.DateTimeFormatDaySeconds);
            return result;
        }

        public static string SerializerObject(object obj)
        {
            return JsonConvert.SerializeObject(obj);
        }

        public static T Deserialize<T>(string jsonString)
        {
            DataContractJsonSerializer ser = new DataContractJsonSerializer(typeof(T));
            MemoryStream ms = new MemoryStream(Encoding.UTF8.GetBytes(jsonString));
            T obj = (T)ser.ReadObject(ms);
            return obj;
        }

        /// <summary> 
        /// Json数据绑定类 
        /// </summary> 
        /// <typeparam name="T"></typeparam> 
        public class JsonBinder<T> : IModelBinder
        {
            public object BindModel(ControllerContext controllerContext, ModelBindingContext bindingContext)
            {
                IList<T> list = new List<T>();
                //从请求中获取提交的参数数据 
                var json = controllerContext.HttpContext.Request.Form[bindingContext.ModelName] as string;
                //提交参数是对象 
                if (json.StartsWith("{") && json.EndsWith("}"))
                {
                    JObject jsonBody = JObject.Parse(json);
                    JsonSerializer js = new JsonSerializer();
                    object obj = js.Deserialize(jsonBody.CreateReader(), typeof(T));
                    list.Add((T)obj);
                    return list;
                }
                //提交参数是数组 
                if (json.StartsWith("[") && json.EndsWith("]"))
                {
                    JArray jsonRsp = JArray.Parse(json);
                    if (jsonRsp != null)
                    {
                        for (int i = 0; i < jsonRsp.Count; i++)
                        {
                            JsonSerializer js = new JsonSerializer();
                            object obj = js.Deserialize(jsonRsp[i].CreateReader(), typeof(T));
                            list.Add((T)obj);
                        }
                    }
                    return list;
                }
                return null;
            }
        }

        public static XmlDocument JsonToXML(string json)
        {
            XmlDocument doc = (XmlDocument)JsonConvert.DeserializeXmlNode(json);
            return doc;
        }


        #region JSON序列化
        
        public static string JsonSerializer(object obj)
        {
            if (obj == null)
            {
                return string.Empty;
            }
            return JsonConvert.SerializeObject(obj, new IsoDateTimeConverter { DateTimeFormat = DateContent.DateTimeFormatDaySeconds });
        }

        /// <summary>
        /// 将指定的对象序列化成 JSON 数据。
        /// </summary>
        /// <param name="obj">要序列化的对象。</param>
        /// <returns></returns>
        public static string ToJson(this object obj)
        {
            try
            {
                return null == obj
                    ? null
                    : JsonConvert.SerializeObject(obj, new IsoDateTimeConverter { DateTimeFormat = DateContent.DateTimeFormatDaySeconds });
            }
            catch
            {
                return null;
            }
        }

        public static T JsonDeserialize<T>(string json) where T : new()
        {
            if (String.IsNullOrEmpty(json))
            {
                return new T();
            }
            return JsonConvert.DeserializeObject<T>(json);
        }

        
        #endregion
    }
}