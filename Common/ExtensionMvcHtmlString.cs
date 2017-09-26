using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;

namespace Common
{
    public static class ExtensionMvcHtmlString
    {
        //#region 静态文件（js、css）引用
        /// <summary>
        /// 添加js引用
        /// </summary>
        /// <param name="helper"></param>
        /// <param name="paths">相对路径 应用程序根目录 /js/main.js</param>
        /// <returns></returns>
        public static MvcHtmlString RenderJs(this HtmlHelper helper, params string[] paths)
        {
            StringBuilder builder = new StringBuilder();
            if (paths != null)
            {
                string wrapper = @"<script src='{0}' type='text/javascript'></script>";
                foreach (var item in paths)
                {
                    if (string.IsNullOrEmpty(item))
                    {
                        continue;
                    }
                    var relativePath = string.Concat(CommonHelper.WebCatalog(), item);
                    //获取版本号
                    var version = GetLastAccessTime(relativePath);
                    relativePath = string.Concat(relativePath, "?_t=", version);
                    builder.Append(string.Format(wrapper, relativePath));
                }
            }
            return new MvcHtmlString(builder.ToString());
        }

        /// <summary>
        /// 添加Http中js版本号引用
        /// </summary>
        /// <param name="helper"></param>
        /// <param name="paths">携带有Http直接的js</param>
        /// <returns></returns>
        public static MvcHtmlString RenderHttpJs(this HtmlHelper helper, params string[] paths)
        {
            StringBuilder builder = new StringBuilder();
            if (paths != null)
            {
                string wrapper = @"<script src='{0}' type='text/javascript'></script>";
                foreach (var item in paths)
                {
                    if (string.IsNullOrEmpty(item))
                    {
                        continue;
                    }
                    var relativePath = item;
                    builder.Append(string.Format(wrapper, relativePath));
                }
            }
            return new MvcHtmlString(builder.ToString());
        }

        /// <summary>
        /// 添加指定目录下js引用
        /// </summary>
        /// <param name="helper"></param>
        /// <param name="catalog">相对目录</param>
        /// <param name="files">完整文件名 main.js</param>
        /// <returns></returns>
        public static MvcHtmlString RenderCatalogJs(this HtmlHelper helper, string catalog, params string[] files)
        {
            if (files != null)
            {
                var paths = files.Select(e => string.Concat(catalog, "/", e)).ToArray();
                return RenderJs(helper, paths);

            }
            return new MvcHtmlString(string.Empty);
        }


        /// <summary>
        /// 添加css引用
        /// </summary>
        /// <param name="helper"></param>
        /// <param name="path">相对路径 应用程序根目录 /css/main.css</param>
        /// <returns></returns>
        public static MvcHtmlString RenderCss(this HtmlHelper helper, params string[] path)
        {
            StringBuilder builder = new StringBuilder();
            if (path != null)
            {
                string wrapper = @"<link href='{0}' rel='stylesheet' type='text/css' />";
                foreach (var item in path)
                {
                    if (string.IsNullOrEmpty(item))
                    {
                        continue;
                    }
                    var relativePath = string.Concat(CommonHelper.WebCatalog(), item);
                    //获取版本号
                    var version = GetLastAccessTime(relativePath);
                    relativePath = string.Concat(relativePath, "?_t=", version);
                    builder.Append(string.Format(
                        wrapper, relativePath));
                }
            }
            return new MvcHtmlString(builder.ToString());
        }

        /// <summary>
        /// 添加Http中css引用
        /// </summary>
        /// <param name="helper"></param>
        /// <param name="path">携带有Http直接的css</param>
        /// <returns></returns>
        public static MvcHtmlString RenderHttpCss(this HtmlHelper helper, params string[] path)
        {
            StringBuilder builder = new StringBuilder();
            if (path != null)
            {
                string wrapper = @"<link href='{0}' rel='stylesheet' type='text/css' />";
                foreach (var item in path)
                {
                    if (string.IsNullOrEmpty(item))
                    {
                        continue;
                    }
                    var relativePath = item;
                    builder.Append(string.Format(
                        wrapper, relativePath));
                }
            }
            return new MvcHtmlString(builder.ToString());
        }

        /// <summary>
        /// 添加指定目录下css引用
        /// </summary>
        /// <param name="helper"></param>
        /// <param name="catalog">相对目录</param>
        /// <param name="files">完整文件名 main.css</param>
        /// <returns></returns>
        public static MvcHtmlString RenderCatalogCss(this HtmlHelper helper, string catalog, params string[] files)
        {
            if (files != null)
            {
                var paths = files.Select(e => string.Concat(catalog, "/", e)).ToArray();
                return RenderCss(helper, paths);
            }
            return new MvcHtmlString(string.Empty);
        }

        /// <summary>
        /// 供应商文本框
        /// </summary>
        public static MvcHtmlString Supplier(this HtmlHelper helper, int pageSize = 10)
        {
            return Supplier(helper, null, pageSize);
        }

        /// <summary>
        /// 供应商文本框
        /// </summary>
        public static MvcHtmlString CssPath(this HtmlHelper helper, string url)
        {
            var relativePath = string.Concat(CommonHelper.WebCatalog(), url);
            return new MvcHtmlString(relativePath.ToString());
        }



        /// <summary>
        /// 供应商文本框
        /// </summary>
        public static MvcHtmlString Supplier(this HtmlHelper helper, object htmlAttributes, int pageSize = 10)
        {
            return Supplier(helper, (IDictionary<string, object>)HtmlHelper.AnonymousObjectToHtmlAttributes(htmlAttributes), pageSize);
        }

        /// <summary>
        /// 供应商文本框
        /// </summary>
        public static MvcHtmlString Supplier(this HtmlHelper helper, IDictionary<string, object> htmlAttributes, int pageSize = 10)
        {
            StringBuilder attributes = new StringBuilder();
            string htmlId = string.Empty;
            if (htmlAttributes != null)
            {
                //供应商页面选择后要给文本框赋值，所以文本框一定要有id，如果使用的人自己定义了就用自己的，没有定义给他赋个默认值
                foreach (var dic in htmlAttributes)
                {
                    if (dic.Key.Equals("id", StringComparison.CurrentCultureIgnoreCase))
                    {
                        htmlId = dic.Value.ToString();
                        continue;
                    }
                    attributes.AppendFormat(" {0}=\"{1}\" ", dic.Key, dic.Value);
                }
            }

            string supplierRequestUrl = string.Format("{0}/CruiseStock/SupplierList", HttpContext.Current.Request.ApplicationPath);
            string onClickString = "onclick=\"if($('#supplierDiv').length>0) return;var $dom = $(this);$.get('" + supplierRequestUrl + "', { domId: $dom.attr('id'), xPos: $dom.offset().left, yPos: $dom.offset().top, pageSize: " + pageSize + " }, function (data) {$('body').append(data);});\"";

            string html = string.Format("<input type=\"text\" readonly=\"readonly\" id=\"{0}\" {1} {2} />",
                string.IsNullOrWhiteSpace(htmlId) ? "autoInitSupplierId" : htmlId,
                attributes.ToString(),
                onClickString);

            return MvcHtmlString.Create(html);
        }

        /// <summary>
        /// 促销文本框
        /// </summary>
        public static MvcHtmlString SPInfo(this HtmlHelper helper, IDictionary<string, object> htmlAttributes)
        {
            StringBuilder attributes = new StringBuilder();
            string htmlId = string.Empty;
            if (htmlAttributes != null)
            {
                foreach (var dic in htmlAttributes)
                {
                    //if (dic.Key.Equals("id", StringComparison.CurrentCultureIgnoreCase))
                    //{
                    //    htmlId = dic.Value.ToString();
                    //    continue;
                    //}
                    attributes.AppendFormat(" {0}=\"{1}\" ", dic.Key, dic.Value);
                }
            }

            string spRequestUrl = string.Format("{0}/PromotionManage/SPList", HttpContext.Current.Request.ApplicationPath);
            string onClickString = "onclick=\"var $dom = $(this);$.get('" + spRequestUrl + "', { domId: $dom.attr('spid'), xPos: $dom.offset().left, yPos: $dom.offset().top }, function (data) {$('body').append(data);});\"";//onblur=\"SelectSPTitleChange(this);\"
            onClickString += " onblur=\"BindChangeInfo(this);\"";
            //string html = string.Format("<input type=\"text\" readonly=\"readonly\" id=\"{0}\" {1} {2} />",
            //    string.IsNullOrWhiteSpace(htmlId) ? "autoInitSPId" : htmlId,
            //    attributes.ToString(),
            //    onClickString);

            string html = string.Format("<input type=\"text\" readonly=\"readonly\"  {0} {1} />",
                attributes.ToString(),
                onClickString);

            return MvcHtmlString.Create(html);
        }

        /// <summary>
        /// 获取文件最新写时间
        /// </summary>
        /// <param name="path">相对路径</param>
        /// <returns>最新写时间</returns>
        private static string GetLastAccessTime(string path)
        {
            string result = string.Empty;
            string abPath = HttpContext.Current.Server.MapPath(path);
            if (File.Exists(abPath))
            {
                result = File.GetLastWriteTime(abPath).ToString("yyyyMMddHHmmssfff");
            }
            return result;
        }

        //#endregion
    }
}
