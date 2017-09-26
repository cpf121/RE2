using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Model.TableModel
{
    public class Department
    {
        public long Id { get; set; }

        public long BDParentId { get; set; }

        public string BDDeptName { get; set; }

        public string BDDeptDescription { get; set; }

        public int BDIsMin { get; set; }

        public int BDIsValid { get; set; }

        public int BDCreateUserId { get; set; }

        public string BDCreateUserName { get; set; }

        public DateTime BDCreateTime { get; set; }

        public int BDOperateUserId { get; set; }

        public string BDOperateUserName { get; set; }

        public DateTime BDOperateTime { get; set; }
    }

    public class DepartView : Department
    {
        /// <summary>
        /// 加密后的部门Id
        /// </summary>
        public string DesId { get; set; }

        /// <summary>
        /// 加密后的父部门Id
        /// </summary>
        public string DesParentId { get; set; }
    }
}
