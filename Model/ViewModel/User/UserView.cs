using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Model.ViewModel.User
{
    public class UserView
    {
        /// <summary>
        /// 人员表Id
        /// </summary>
        public string UserId { get; set; }

        /// <summary>
        /// 姓
        /// </summary>
        public string BUSurname { get; set; }

        /// <summary>
        /// 名
        /// </summary>
        public string BUGivenname { get; set; }

        /// <summary>
        /// 工号
        /// </summary>
        public string BUJobNumber { get; set; }

        /// <summary>
        /// 性别(1-男，2-女，0-其他)
        /// </summary>
        public int BUSex { get; set; }

        /// <summary>
        /// 头像
        /// </summary>
        public string BUAvatars { get; set; }

        /// <summary>
        /// 电话
        /// </summary>
        public string BUPhoneNum { get; set; }

        /// <summary>
        /// 邮箱
        /// </summary>
        public string BUEmail { get; set; }

        /// <summary>
        /// 部门Id
        /// </summary>
        public string  DepartId { get; set; }

        /// <summary>
        /// 岗位头衔
        /// </summary>
        public string BUTitle { get; set; }
        
        /// <summary>
        /// 有效性（0.无效 1.有效）
        /// </summary>
        public int BUIsValid { get; set; }
    }
}
