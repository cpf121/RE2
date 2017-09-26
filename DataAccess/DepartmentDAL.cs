using Common;
using Common.Enum;
using Model.TableModel;
using Model.ViewModel.Department;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

namespace DataAccess
{
    public class DepartmentDAL : SqlHelper
    {
        private const string tableName = "T_BASE_DEPARTMENT";

        /// <summary>
        /// 描述：根据部门Id获取部门信息
        /// </summary>
        /// <param name="departmentId">部门Id</param>
        /// <returns></returns>
        public Department GetDpById(int departmentId)
        {
            var departmentInfo = new Department();
            SqlParameter[] para = {
                new SqlParameter("@Id", departmentId)
            };

            var sql = @"SELECT Id,BDParentId,BDDeptName,BDDeptDesc,BDIsMin,BDIsValid FROM " + tableName
                + " WHERE Id=@Id";

            using (var dr = SqlHelper.ExecuteReader(CommandType.Text, sql,null, para))
            {
                if (dr.Read())
                {
                    departmentInfo.Id = Convert.ToInt32(dr["Id"]);
                    departmentInfo.BDParentId = Convert.ToInt32(dr["BDParentId"]);
                    departmentInfo.BDDeptName = dr["BDDeptName"].ToString();
                    departmentInfo.BDDeptDescription = dr["BDDeptDesc"].ToString();
                    departmentInfo.BDIsMin = Convert.ToInt32(dr["BDIsMin"]);
                    departmentInfo.BDIsValid = Convert.ToInt32(dr["BDIsValid"]);
                }
                else
                {
                    departmentInfo = null;
                }
            }

            return departmentInfo;
        }

        public Department GetRootDepartment()
        {
            var departmentInfo = new Department();

            var sql = @"SELECT Id,BDParentId,BDDeptName,BDDeptDesc,BDIsMin,BDIsValid,BDCreateUserId,
                       BDCreateUserName,BDCreateTime,BDOperateUserId,BDOperateUserName,BDOperateTime FROM " + tableName
                + " WHERE BDParentId=0";
            using (var dr = SqlHelper.ExecuteReader(CommandType.Text, sql))
            {
                if (dr.Read())
                {
                    departmentInfo.Id = Convert.ToInt32(dr["Id"]);
                    departmentInfo.BDParentId = Convert.ToInt32(dr["BDParentId"]);
                    departmentInfo.BDDeptName = dr["BDDeptName"].ToString();
                    departmentInfo.BDDeptDescription = dr["BDDeptDesc"].ToString();
                    departmentInfo.BDIsMin = Convert.ToInt32(dr["BDIsMin"]);
                    departmentInfo.BDIsValid = Convert.ToInt32(dr["BDIsValid"]);
                    departmentInfo.BDCreateUserId = Convert.ToInt32(dr["BDCreateUserId"]);
                    departmentInfo.BDCreateUserName = dr["BDCreateUserName"].ToString();
                    departmentInfo.BDCreateTime = DateTime.Parse(dr["BDCreateTime"].ToString());
                    departmentInfo.BDOperateUserId = Convert.ToInt32(dr["BDOperateUserId"]);
                    departmentInfo.BDOperateUserName = dr["BDOperateUserName"].ToString();
                    departmentInfo.BDOperateTime = DateTime.Parse(dr["BDOperateTime"].ToString());
                }
                else
                {
                    departmentInfo = null;
                }
            }
            return departmentInfo;
        }

        /// <summary>
        /// 描述：获取所有的组织框架
        /// 创建标识：cpf
        /// 创建时间：2017-9-19 20:37:36
        /// </summary>
        /// <returns></returns>
        public List<Department> GetAllDepartMent()
        {
            var list = new List<Department>();
            var sql = @"SELECT Id,BDParentId,BDDeptName,BDDeptDesc,BDIsMin,BDIsValid,BDCreateUserId,
                       BDCreateUserName,BDCreateTime,BDOperateUserId,BDOperateUserName,BDOperateTime FROM " + tableName;
            var ds = SqlHelper.ExecuteDataSet(CommandType.Text, sql);
            if (ds != null && ds.Tables.Count > 0)
            {
                DataTable dt = new DataTable();
                dt = ds.Tables[0];
                list = DataConvertHelper.DataTableToList<Department>(dt);
            }
            return list;
        }

        /// <summary>
        /// 描述：根据id获取部门信息
        /// </summary>
        /// <param name="departmentId"></param>
        /// <returns></returns>
        public DepartmentInfo GetDepartById(int departmentId)
        {
            var departmentInfo = new DepartmentInfo();
            SqlParameter[] para = {
                new SqlParameter("@Id", departmentId)
            };
            var sql = @"SELECT a.Id Id,a.BDParentId ParentId,a.BDDeptName Name,a.BDDeptDesc Description,b.BDDeptName ParentName FROM " + tableName
                + " a WITH(NOLOCK ) LEFT JOIN " + tableName + " b WITH(NOLOCK ) ON a.BDParentId = b.Id WHERE a.Id=@Id";

            using (var dr = SqlHelper.ExecuteReader(CommandType.Text, sql, null,para))
            {
                if (dr.Read())
                {
                    departmentInfo.Id = dr["Id"].ToString();
                    departmentInfo.ParentId = dr["ParentId"].ToString();
                    departmentInfo.Name = dr["Name"].ToString();
                    departmentInfo.Description = dr["Description"].ToString();
                    departmentInfo.ParentName = dr["ParentName"].ToString();
                }
                else
                {
                    departmentInfo = null;
                }
            }

            return departmentInfo;
        }

        /// <summary>
        /// 描述：判断部门名称是否重名
        /// </summary>
        /// <param name="departmentName"></param>
        /// <returns></returns>
        public bool CheckDepartment(string departmentName)
        {
            SqlParameter[] para = {
                new SqlParameter("@BDDeptName", departmentName),
                new SqlParameter("@BDIsValid", EnabledEnum.Enabled.GetHashCode()),
            };

            var sql = @"SELECT Id,BDParentId,BDDeptName,BDDeptDesc,BDIsMin,BDIsValid FROM " + tableName
                + " WHERE BDDeptName=@BDDeptName AND BDIsValid=@BDIsValid";

            var count = SqlHelper.ExecteNonQuery(CommandType.Text, sql,null, para);
            return count > 0;

        }

        /// <summary>
        /// 插入数据
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public long Insert(Department model)
        {
            var sql = @"INSERT INTO " + tableName +
                " (BDParentId,BDDeptName,BDDeptDesc,BDIsMin,BDIsValid,BDCreateUserId,BDCreateUserName,BDCreateTime,BDOperateUserId,BDOperateUserName,BDOperateTime)" +
                " VALUES (@BDParentId,@BDDeptName,@BDDeptDesc,@BDIsMin,@BDIsValid,@BDCreateUserId,@BDCreateUserName,@BDCreateTime,@BDOperateUserId,@BDOperateUserName,@BDOperateTime)" +
                " select id = scope_identity()";
            SqlParameter[] para = {
                new SqlParameter("@BDParentId",model.BDParentId),
                new SqlParameter("@BDDeptName",model.BDDeptName),
                new SqlParameter("@BDDeptDesc",model.BDDeptDescription),
                new SqlParameter("@BDIsMin",model.BDIsMin),
                new SqlParameter("@BDIsValid",model.BDIsValid),
                new SqlParameter("@BDCreateUserId",model.BDCreateUserId),
                new SqlParameter("@BDCreateUserName",model.BDCreateUserName),
                new SqlParameter("@BDCreateTime",model.BDCreateTime),
                new SqlParameter("@BDOperateUserId",model.BDOperateUserId),
                new SqlParameter("@BDOperateUserName",model.BDOperateUserName),
                new SqlParameter("@BDOperateTime",model.BDOperateTime),
            };
            long result = 0;
            var ds = SqlHelper.ExecuteDataSet(CommandType.Text, sql.ToString(),null, para);
            if (ds != null && ds.Tables.Count > 0)
            {
                var IdString = ds.Tables[0].Rows[0][0].ToString();
                result = string.IsNullOrEmpty(IdString) ? 0 : long.Parse(IdString);
            }
            return result;
        }

        public bool Update(Department model)
        {
            var sql = @"UPDATE  " + tableName +
                " SET BDDeptName=@BDDeptName,BDDeptDesc=@BDDeptDesc," +
                "BDOperateUserId=@BDOperateUserId,BDOperateUserName=@BDOperateUserName,BDOperateTime=@BDOperateTime WHERE Id=@Id";
            SqlParameter[] para = {
                new SqlParameter("@Id",model.Id),
                new SqlParameter("@BDDeptName",model.BDDeptName),
                new SqlParameter("@BDDeptDesc",model.BDDeptDescription),
                new SqlParameter("@BDOperateUserId",model.BDOperateUserId),
                new SqlParameter("@BDOperateUserName",model.BDOperateUserName),
                new SqlParameter("@BDOperateTime",model.BDOperateTime),
            };
            return SqlHelper.ExecteNonQuery(CommandType.Text, sql,null, para) > 0;
        }

        /// <summary>
        /// 描述：根据部门Id获取下一级部门信息
        /// </summary>
        /// <param name="parentId"></param>
        /// <returns></returns>
        public List<Department> GetDepartmentByParentId(long parentId)
        {
            var list = new List<Department>();
            var departmentInfo = new Department();
            SqlParameter[] para = {
                new SqlParameter("@BDParentId", parentId)
            };

            var sql = @"SELECT Id,BDParentId,BDDeptName,BDDeptDesc,BDIsMin,BDIsValid FROM " + tableName
                + " WHERE BDParentId=@BDParentId";

            var ds = SqlHelper.ExecuteDataSet(CommandType.Text, sql,null,para);
            if (ds != null && ds.Tables.Count > 0)
            {
                DataTable dt = new DataTable();
                dt = ds.Tables[0];
                list = DataConvertHelper.DataTableToList<Department>(dt);
            }
            return list;
        }

        /// <summary>
        /// 描述：根据部门Id更新有效性
        /// 创建标识：cpf
        /// 创建时间：2017-9-24 17:50:28
        /// </summary>
        /// <param name="dpId"></param>
        /// <param name="isvalid"></param>
        /// <returns></returns>
        public bool UpdateIsValid(Department model)
        {
            var sql = @"UPDATE  " + tableName +
                " SET BDIsValid=@BDIsValid, " +
                " BDOperateUserId=@BDOperateUserId,BDOperateUserName=@BDOperateUserName,BDOperateTime=@BDOperateTime WHERE Id=@Id";
            SqlParameter[] para = {
                new SqlParameter("@Id",model.Id),
                new SqlParameter("@BDIsValid",model.BDIsValid),
                new SqlParameter("@BDOperateUserId",model.BDOperateUserId),
                new SqlParameter("@BDOperateUserName",model.BDOperateUserName),
                new SqlParameter("@BDOperateTime",model.BDOperateTime),
            };
            return SqlHelper.ExecteNonQuery(CommandType.Text, sql,null, para) > 0;
        }
    }
}
