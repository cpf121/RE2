using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Model.Home
{
    /// <summary>
    /// 描述：组织架构实体
    /// 创建标识：cpf
    /// 创建时间：2017-9-19 20:11:49
    /// </summary>
    public class OrganizationEntity
    {
        /// <summary>
        /// 上级部门ID
        /// </summary>
        public string ParentId { get; set; }

        /// <summary>
        /// 部门Id
        /// </summary>
        public string DepartmentId { get; set; }

        /// <summary>
        /// 部门名称
        /// </summary>
        public string DepartmentName { get; set; }

        /// <summary>
        /// 是否有效
        /// </summary>
        public byte DepartmentIsValid { get; set; }

        /// <summary>
        /// 部门员工数量
        /// </summary>
        public int DepartmentUserNum { get; set; }

        /// <summary>
        /// 子项目列表
        /// </summary>
        public List<OrganizationEntity> DepartmentChildList { get; set; }
    }

    /// <summary>
    /// 部门查找类
    /// </summary>
    public class OrganizationSearch
    {
        public string id { get; set; }

        public string text { get; set; }
    }
}
