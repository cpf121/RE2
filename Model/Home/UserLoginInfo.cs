using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model.Home
{
    /// <summary>
    /// 用户信息
    /// </summary>
    public class UserLoginInfo
    {
        public UserLoginInfo()
        {
            Account = string.Empty;
            PhoneNum = string.Empty;
            UserName = string.Empty;
            Avatars = string.Empty;
            DepartName = string.Empty;
        }

        /// <summary>
        /// 登录账号
        /// </summary>
        public string Account { get; set; }

        /// <summary>
        /// 账号类型(1.管理员 2.员工)
        /// </summary>
        public int AccountType { get; set; }

        /// <summary>
        /// 用户Id
        /// </summary>
        public long UserId { get; set; }

        /// <summary>
        /// 用户姓名
        /// </summary>
        public string UserName { get; set; }

        /// <summary>
        /// 手机号码
        /// </summary>
        public string PhoneNum { get; set; }

        /// <summary>
        /// 头像
        /// </summary>
        public string Avatars { get; set; }

        /// <summary>
        /// 部门Id
        /// </summary>
        public long DepartId { get; set; }

        /// <summary>
        /// 部门
        /// </summary>
        public string DepartName { get; set; }
    }
}
