
using Business;
using Common;
using Common.Costant;
using Common.Enum;
using Model.Home;
using System;
using System.Web.Mvc;

namespace RoechlingEquipment.Controllers
{
    public class LoginController : BaseController
    {
        //TODO:CPF
        public ActionResult LoginPage()
        {
            // Validate input
            var workNo = string.Empty;
            //HomeBusiness.UserLogin("", "", "", out workNo);
            ViewBag.BasePath = BasePath;
            ViewBag.CurrentCulture = CultureHelper.GetCurrentCulture();
            ViewBag.DBType = EnumHelper.SelectListEnum<DBType>(null, false, null);
            return View();
        }

        [HttpPost]
        public ActionResult Login(string account,string password,int DBType)
        {
            var msg = string.Empty;
            var success = false;
            
            Session[SessionKey.SESSION_KEY_DBINFO] = DBType;

            try
            {
                var result = HomeBusiness.Login(account, password);

                //写入cookie
                string key = CommonHelper.Md5(CookieKey.COOKIE_KEY_USERINFO);
                string data = JsonHelper.Serializer<UserLoginInfo>(result);
                CookieHelper.SetCookie(
                    key,
                    CommonHelper.DesEncrypt(data, CookieKey.COOKIE_KEY_ENCRYPT),
                    DateTime.Now.AddDays(1).Date,
                    ServerInfo.GetTopDomain);
                success = true;
            }
            catch (Exception ex)
            {
                msg = ex.Message;
            }
            return Json(new { Success = success, Message = msg });
        }
    }
}
