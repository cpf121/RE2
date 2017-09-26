using Model.Home;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;

namespace Common
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
    public class ServerAuthorizeAttribute : AuthorizeAttribute
    {
        /// <summary>
        /// 登录用户
        /// </summary>
        protected UserLoginInfo LoginUser
        {
            get { return LoginHelper.GetUser(); }
        }
        /// <summary>
        /// 验证
        /// </summary>
        /// <param name="filterContext"></param>
        public override void OnAuthorization(AuthorizationContext filterContext)
        {
            try
            {
                if (LoginUser != null) return;

                //var redirectUri = string.Empty;
                //if (Request.Url != null)
                //{
                //    redirectUri = Request.Url.ToString();
                //    var thiscntroller = RouteData.Values["controller"].ToString().ToLower();
                //    var thisaction = RouteData.Values["action"].ToString().ToLower();
                //    if (thiscntroller == "saashome" && thisaction == "index")
                //    {
                //        redirectUri = string.Empty;
                //    }
                //}

                filterContext.Result = new RedirectResult("/" + CultureHelper.GetDefaultCulture() + "/login.html");
            }
            catch (Exception ex)
            {
                #region 系统异常记录天网日志

                var userinfo = LoginUser;
                var jobnum = userinfo == null ? "" : userinfo.UserName;

                #endregion
            }
        }
    }

}
