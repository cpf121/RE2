using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Model.ViewModel.Department
{
    public class DepartmentInfo
    {
        /// <summary>
        /// 部门id
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// 部门名称
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 上级部门Id
        /// </summary>
        public string ParentId { get; set; }

        /// <summary>
        /// 上级部门名称
        /// </summary>
        public string ParentName { get; set; }

        /// <summary>
        /// 部门描述
        /// </summary>
        public string Description { get; set; }
    }

    /// <summary>
    /// 描述：部门信息保存Model实体
    /// </summary>
    public class DepartmentSaveModel
    {
        /// <summary>
        /// 部门名称
        /// </summary>
        public string DepartName { get; set; }

        /// <summary>
        /// 部门Id
        /// </summary>
        public string DepartId { get; set; }

        /// <summary>
        /// 父部门Id
        /// </summary>
        public string ParentId { get; set; }

        /// <summary>
        /// 是否启用
        /// </summary>
        public int IsValid { get; set; }

        /// <summary>
        /// 部门描述
        /// </summary>
        public string DepartDesc { get; set; }
    }
}
