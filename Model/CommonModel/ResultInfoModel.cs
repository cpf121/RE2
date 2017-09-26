using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Model.CommonModel
{
    public class ResultInfoModel
    {
        /// <summary>
        /// 是否成功
        /// </summary>
        public bool IsSuccess { get; set; }

        /// <summary>
        /// 返回代码
        /// </summary>
        public string Code { get; set; }

        /// <summary>
        /// 返回信息
        /// </summary>
        public string Message
        {
            get { return _message ?? string.Empty; }
            set { _message = value; }
        }
        private string _message = string.Empty;

    }
}
