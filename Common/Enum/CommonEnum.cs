using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace Common.Enum
{
    public enum EnabledEnum
    {
        /// <summary>
        /// 是
        /// </summary>
        [Description("有效")]
        Enabled = 1,

        /// <summary>
        /// 否
        /// </summary>
        [Description("无效")]
        UnEnabled = 0,
    }

    /// <summary>
    /// 描述：数据库种类
    /// </summary>
    public enum DBType
    {
        [Description("默认数据库")]
        Default =0,
    }
}
