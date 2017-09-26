using Common.Costant;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;

namespace Common
{
    public class ServerInfo
    {
        /// <summary>
        /// 取得网站的根目录的URL
        /// </summary>
        /// <returns></returns>
        public static string RootURI
        {
            get
            {
                string appPath = "";
                HttpContext HttpCurrent = HttpContext.Current;
                HttpRequest Req;
                if (HttpCurrent != null)
                {
                    Req = HttpCurrent.Request;
                    string UrlAuthority = Req.Url.GetLeftPart(UriPartial.Authority);
                    if (Req.ApplicationPath == null || Req.ApplicationPath == "/")
                    {
                        appPath = UrlAuthority;
                    }
                    else
                    {
                        appPath = UrlAuthority + Req.ApplicationPath;
                    }
                }
                if (!appPath.EndsWith("/"))
                {
                    appPath += "/";
                }
                return appPath;
            }
        }

        /// <summary>
        /// 获取网站的根目录的物理路径
        /// </summary>
        /// <returns></returns>
        public static string RootPath
        {
            get
            {
                string appPath = "";
                HttpContext HttpCurrent = HttpContext.Current;
                if (HttpCurrent != null)
                {
                    appPath = HttpCurrent.Server.MapPath("~");
                }
                else
                {
                    appPath = AppDomain.CurrentDomain.BaseDirectory;
                    if (Regex.Match(appPath, @"\\$", RegexOptions.Compiled).Success)
                    {
                        appPath = appPath.Substring(0, appPath.Length - 1);
                    }
                }
                if (!appPath.EndsWith("\\"))
                {
                    appPath += "\\";
                }
                return appPath;
            }
        }

        /// <summary>
        /// 获取网站虚拟目录路径
        /// </summary>
        /// <returns></returns>
        public static string AppPath
        {
            get
            {
                if (HttpContext.Current.Request.ApplicationPath == "/")
                {
                    return string.Empty;
                }
                else
                {
                    return HttpContext.Current.Request.ApplicationPath;
                }
            }
        }

        /// <summary>
        /// 取得网站的物理路径
        /// </summary>
        /// <returns></returns>
        public static string ServerPath
        {
            get
            {
                return HttpContext.Current.Request.PhysicalApplicationPath;
            }
        }

        /// <summary>
        /// 服务器名
        /// </summary>
        public static string ServerName
        {
            get
            {
                return string.Format("http://{0}/", HttpContext.Current.Request.Url.Host);
            }
        }

        /// <summary>
        /// IP地址
        /// </summary>
        public static string IPAddress
        {
            get
            {
                return HttpContext.Current.Request.ServerVariables["LOCAl_ADDR"];
            }
        }

        /// <summary>
        /// 当前域名
        /// </summary>
        public static string Domain
        {
            get
            {
                return HttpContext.Current.Request.ServerVariables["SERVER_NAME"].ToString();
            }
        }

        /// <summary>
        /// WEB端口
        /// </summary>
        public static string Server_Port
        {
            get
            {
                return HttpContext.Current.Request.ServerVariables["Server_Port"].ToString();
            }
        }

        /// <summary>
        /// IIS版本
        /// </summary>
        public static string Server_IIS
        {
            get
            {
                return HttpContext.Current.Request.ServerVariables["Server_SoftWare"].ToString();
            }
        }

        /// <summary>
        /// 服务器操作系统
        /// </summary>
        public static string OSVersion
        {
            get
            {
                return Environment.OSVersion.ToString();
            }
        }

        /// <summary>
        /// 系统所在文件夹
        /// </summary>
        public static string SystemDirectory
        {
            get
            {
                return Environment.SystemDirectory.ToString();
            }
        }

        /// <summary>
        /// 脚本默认超时时间
        /// </summary>
        public static string ScriptTimeout
        {
            get
            {
                return (HttpContext.Current.Server.ScriptTimeout).ToString() + "秒";
            }
        }

        /// <summary>
        /// 服务器的语言种类
        /// </summary>
        public static string InstalledUICulture
        {
            get
            {
                return CultureInfo.InstalledUICulture.EnglishName;
            }
        }

        /// <summary>
        /// .NET Framework 版本
        /// </summary>
        public static string AspNetVer
        {
            get
            {
                return string.Concat(new object[] { Environment.Version.Major, ".", Environment.Version.Minor, Environment.Version.Build, ".", Environment.Version.Revision });
            }
        }

        /// <summary>
        /// 服务器当前时间
        /// </summary>
        public static string CurrentTime
        {
            get
            {
                return DateTime.Now.ToString(CommonConstant.DateTimeFormatDaySeconds);
            }
        }

        /// <summary>
        /// 服务器IE版本
        /// </summary>
        public static string IEVersion
        {
            get
            {
                RegistryKey key = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\Microsoft\Internet Explorer\Version Vector");
                return key.GetValue("IE", "未检测到").ToString();
            }
        }

        /// <summary>
        /// 上次启动到现在已运行
        /// </summary>
        public static string LastStartToNow
        {
            get
            {
                return ((Environment.TickCount / 0x3e8) / 60).ToString() + "分钟";
            }
        }

        /// <summary>
        /// 逻辑驱动器
        /// </summary>
        public static string LogicDriver
        {
            get
            {
                var tmp = string.Empty;
                string[] achDrives = Directory.GetLogicalDrives();
                for (int i = 0; i < Directory.GetLogicalDrives().Length - 1; i++)
                {
                    tmp += achDrives[i].ToString();
                }

                return tmp;
            }
        }

        /// <summary>
        /// CPU 总数
        /// </summary>
        public static string CpuNum
        {
            get
            {
                return Environment.GetEnvironmentVariable("NUMBER_OF_PROCESSORS").ToString();
            }
        }

        /// <summary>
        /// CPU 类型
        /// </summary>
        public static string CpuType
        {
            get
            {
                return Environment.GetEnvironmentVariable("PROCESSOR_IDENTIFIER").ToString();
            }
        }

        /// <summary>
        /// 虚拟内存
        /// </summary>
        public static string TotalMemory
        {
            get
            {
                ;
                return (Environment.WorkingSet / 1048576).ToString() + "M";
                //return ((Double)GC.GetTotalMemory(false) / 1048576).ToString("N2") + "M";
            }
        }

        /// <summary>
        /// 当前程序占用内存    
        /// </summary>
        public static string WorkingMemory
        {
            get
            {
                return ((Double)GC.GetTotalMemory(false) / 1048576).ToString("N2") + "M";
            }
        }

        /// <summary>
        /// Asp.net所占内存
        /// </summary>
        public static string MemoryForNet
        {
            get
            {
                return ((Double)Process.GetCurrentProcess().WorkingSet64 / 1048576).ToString("N2") + "M";
            }
        }

        /// <summary>
        /// Asp.net所占CPU处理时间
        /// </summary>
        public static string CpuForNet
        {
            get
            {
                return ((TimeSpan)Process.GetCurrentProcess().TotalProcessorTime).TotalSeconds.ToString("N2");
            }
        }

        /// <summary>
        /// 当前Session数量
        /// </summary>
        public static string SessionNum
        {
            get
            {
                return HttpContext.Current.Session.Contents.Count.ToString();
            }
        }

        /// <summary>
        /// 当前SessionID
        /// </summary>
        public static string SessionID
        {
            get
            {
                return HttpContext.Current.Session.Contents.SessionID;
            }
        }

        /// <summary>
        /// 当前系统用户名
        /// </summary>
        public static string SystemUserName
        {
            get
            {
                return Environment.UserName;
            }
        }

        /// <summary>
        /// 获取顶级域名
        /// </summary>
        /// <param name="sDomain"></param>
        /// <returns></returns>
        public static string GetTopDomain
        {
            get
            {
                string domain = HttpContext.Current.Request.Url.Host;
                //https://www.safsd.asdfasdf.baidu.com.cn/ssssd/s/b/d/hhh.html?domain=sfsdf.com.cn&id=1
                domain = domain.Trim().ToLower();
                string rootDomain = ".com.cn|.gov.cn|.cn|.com|.net|.org|.so|.co|.mobi|.tel|.biz|.info|.name|.me|.cc|.tv|.asiz|.hk";
                if (domain.StartsWith("http://")) domain = domain.Replace("http://", "");
                if (domain.StartsWith("https://")) domain = domain.Replace("https://", "");
                if (domain.StartsWith("www.")) domain = domain.Replace("www.", "");
                //safsd.asdfasdf.baidu.com.cn/ssssd/s/b/d/hhh.html?domain=sfsdf.com.cn&id=1
                if (domain.IndexOf("/") > 0)
                    domain = domain.Substring(0, domain.IndexOf("/"));
                //safsd.asdfasdf.baidu.com.cn
                foreach (string item in rootDomain.Split('|'))
                {
                    if (domain.EndsWith(item))
                    {
                        domain = domain.Replace(item, "");
                        if (domain.LastIndexOf(".") > 0)//adfasd.asdfas.cn
                        {
                            domain = domain.Replace(domain.Substring(0, domain.LastIndexOf(".") + 1), "");
                        }
                        return domain + item;
                    }
                    continue;
                }
                return "";
            }

        }

        /// <summary>
        /// 获取客户端IP
        /// </summary>
        public static string ClientIP
        {
            get
            {
                var request = HttpContext.Current.Request;
                var clientip = request.ServerVariables["HTTP_X_FORWARDED_FOR"] != ""
                    ? request.ServerVariables["REMOTE_ADDR"]
                    : request.ServerVariables["HTTP_X_FORWARDED_FOR"];
                if (string.IsNullOrEmpty(clientip))
                {
                    clientip = request.UserHostAddress;
                }
                return clientip;
            }
        }

    }
}
