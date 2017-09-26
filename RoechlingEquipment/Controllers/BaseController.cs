
using Common;
using Model.Home;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;

namespace RoechlingEquipment.Controllers
{
    public class BaseController : Controller
    {
        /// <summary>
        /// 获取跟目录
        /// </summary>
        protected string BasePath
        {
            get { return ServerInfo.AppPath; }
        }

        /// <summary>
        /// 登录用户
        /// </summary>
        protected UserLoginInfo LoginUser
        {
            get { return LoginHelper.GetUser(); }
        }

        protected override IAsyncResult BeginExecuteCore(AsyncCallback callback, object state)
        {
            string cultureName = RouteData.Values["culture"] as string;

            // Attempt to read the culture cookie from Request
            if (cultureName == null)
                cultureName = Request.UserLanguages != null && Request.UserLanguages.Length > 0 ? Request.UserLanguages[0] : "en-US"; // obtain it from HTTP header AcceptLanguages

            // Validate culture name
            cultureName = CultureHelper.GetImplementedCulture(cultureName); // This is safe


            if (RouteData.Values["culture"] as string != cultureName)
            {

                // Force a valid culture in the URL
                RouteData.Values["culture"] = cultureName.ToLowerInvariant(); // lower case too

                // Redirect user
                Response.RedirectToRoute(RouteData.Values);
            }


            // Modify current thread's cultures            
            Thread.CurrentThread.CurrentCulture = new CultureInfo(cultureName);
            Thread.CurrentThread.CurrentUICulture = Thread.CurrentThread.CurrentCulture;


            return base.BeginExecuteCore(callback, state);
        }


    }
}