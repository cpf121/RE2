using Model.Home;
using Common.Costant;

namespace Common
{
    /// <summary>
    /// SaaS Helper
    /// </summary>
    public class LoginHelper
    {
        /// <summary>
        /// 获取用户信息
        /// </summary>
        /// <returns></returns>
        public static UserLoginInfo GetUser()
        {
            UserLoginInfo userInfo = null;
            var key = CommonHelper.Md5(CookieKey.COOKIE_KEY_USERINFO);
            var data = CookieHelper.GetCookieValue(key);
            if (!string.IsNullOrEmpty(data))
            {
                data = CommonHelper.DesDecrypt(data, HomeContent.CookieKeyEncrypt);
                userInfo = JsonHelper.Deserialize<UserLoginInfo>(data);
            }
            return userInfo;
        }
    }
}