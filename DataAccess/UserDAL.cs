using Common;
using Common.Enum;
using Model.TableModel;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;

namespace DataAccess
{
    public class UserDAL
    {
        private const string tableName = "T_BASE_USER";
        public User GetUserById(int id)
        {
            var user = new User();
            string sql = @"SELECT Id,BUSurname,BUGivenname,BUJobNumber,BUSex,BUAvatars,BUPhoneNum,BUEmail,BUDepartId,BUIsValid,BUTitle FROM " + tableName
                + " WHERE Id=@Id";
            SqlParameter[] para = { new SqlParameter("@Id", id) };
            using (var dr = SqlHelper.ExecuteReader(CommandType.Text, sql,null, para))
            {
                if (dr.Read())
                {
                    user.Id = Convert.ToInt32(dr["Id"]);
                    user.BUSurname = dr["BUSurname"].ToString();
                    user.BUGivenname = dr["BUGivenname"].ToString();
                    user.BUJobNumber = dr["BUJobNumber"].ToString();
                    user.BUSex = Convert.ToInt32(dr["BUSex"]);
                    user.BUAvatars = dr["BUAvatars"].ToString();
                    user.BUPhoneNum = dr["BUPhoneNum"].ToString();
                    user.BUEmail = dr["BUEmail"].ToString();
                    user.BUDepartId = Convert.ToInt32(dr["BUDepartId"]);
                    user.BUTitle = dr["BUTitle"].ToString();
                    user.BUIsValid = Convert.ToInt32(dr["BUIsValid"]);
                }
                else
                {
                    user = null;
                }
            }
            return user;
        }

        /// <summary>
        /// 描述：获取所有的有效人员
        /// </summary>
        /// <returns></returns>
        public List<User> GetAllIsValidUser()
        {
            var list = new List<User>();
            string sql = @"SELECT Id,BUSurname,BUGivenname,BUJobNumber,BUSex,BUAvatars,BUPhoneNum,BUEmail,BUDepartId,BUIsValid, 
                           BUTitle,BUCreateUserId,BUCreateUserName,BUCreateTime,BUOperateUserId,BUOperateUserName,BUOperateTime FROM " + tableName;
            var ds = SqlHelper.ExecuteDataSet(CommandType.Text, sql);
            if (ds != null && ds.Tables.Count > 0)
            {
                var dt = new DataTable();
                dt = ds.Tables[0];
                list = DataConvertHelper.DataTableToList<User>(dt);
            }
            return list;
        }

        /// <summary>
        /// 描述:根据部门Id获取该部门下的所有有效的员工
        /// </summary>
        /// <param name="departId"></param>
        /// <returns></returns>
        public List<User> GetUserByDepartId(long departId)
        {
            var list = new List<User>();
            var sql = @"SELECT Id,BUSurname,BUGivenname,BUJobNumber,BUSex,BUAvatars,BUPhoneNum,BUEmail,BUDepartId,BUIsValid, 
                           BUTitle,BUCreateUserId,BUCreateUserName,BUCreateTime,BUOperateUserId,BUOperateUserName,BUOperateTime FROM " + tableName
                           + " WHERE BUIsValid=@BUIsValid AND BUDepartId=@BUDepartId";
            SqlParameter[] para = {
                new SqlParameter("@BUIsValid",EnabledEnum.Enabled.GetHashCode()),
                new SqlParameter("@BUDepartId",departId)
            };
            var ds = SqlHelper.ExecuteDataSet(CommandType.Text, sql);
            if (ds != null && ds.Tables.Count > 0)
            {
                var dt = new DataTable();
                dt = ds.Tables[0];
                list = DataConvertHelper.DataTableToList<User>(dt);
            }
            return list;
        }

        /// <summary>
        /// 描述：获取某部门下的员工信息
        /// 创建标识：cpf
        /// 创建时间：2017-9-24 17:40:52
        /// </summary>
        /// <param name="dpId"></param>
        /// <returns></returns>
        public List<User> GetUserListByDeptId(long dpId)
        {
            var list = new List<User>();
            var user = new User();
            string sql = @"SELECT Id,BUSurname,BUGivenname,BUJobNumber,BUSex,BUAvatars,BUPhoneNum,BUEmail,BUDepartId,BUIsValid,BUTitle FROM " + tableName
                + " WHERE BUDepartId=@BUDepartId AND " +
                " BUIsValid=@BUIsValid";
            SqlParameter[] para = {
                new SqlParameter("@BUIsValid",EnabledEnum.Enabled.GetHashCode()),
                new SqlParameter("@BUDepartId", dpId)
            };
            var ds = SqlHelper.ExecuteDataSet(CommandType.Text,sql,null,para);
            if (ds != null && ds.Tables.Count > 0)
            {
                var dt = new DataTable();
                dt = ds.Tables[0];
                list = DataConvertHelper.DataTableToList<User>(dt);
            }
            return list;
        }

        /// <summary>
        /// 描述：插入员工并返回员工id
        /// 创建标识：cpf
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public long InsertUser(User model,SqlTransaction tran)
        {
            var sql = @"INSERT INTO " + tableName +
                " (BUSurname,BUGivenname,BUJobNumber,BUSex,BUAvatars,BUPhoneNum,BUEmail,BUDepartId,BUTitle,BUCreateUserId,BUCreateUserName,BUCreateTime,BUOperateUserId,BUOperateUserName,BUOperateTime,BUIsValid )" +
                " VALUES (@BUSurname,@BUGivenname,@BUJobNumber,@BUSex,@BUAvatars,@BUPhoneNum,@BUEmail,@BUDepartId,@BUTitle,@BUCreateUserId,@BUCreateUserName,@BUCreateTime,@BUOperateUserId,@BUOperateUserName,@BUOperateTime,@BUIsValid)" +
                " select id = scope_identity()";
            SqlParameter[] para = {
                new SqlParameter("@BUSurname",model.BUSurname),
                new SqlParameter("@BUGivenname",model.BUGivenname),
                new SqlParameter("@BUJobNumber",model.BUJobNumber),
                new SqlParameter("@BUSex",model.BUSex),
                new SqlParameter("@BUAvatars",model.BUAvatars),
                new SqlParameter("@BUPhoneNum",model.BUPhoneNum),
                new SqlParameter("@BUEmail",model.BUEmail),
                new SqlParameter("@BUDepartId",model.BUDepartId),
                new SqlParameter("@BUTitle",model.BUTitle),
                new SqlParameter("@BUCreateUserId",model.BUCreateUserId),
                new SqlParameter("@BUCreateUserName",model.BUCreateUserName),
                new SqlParameter("@BUCreateTime",model.BUCreateTime),
                new SqlParameter("@BUOperateUserId",model.BUOperateUserId),
                new SqlParameter("@BUOperateUserName",model.BUOperateUserName),
                new SqlParameter("@BUOperateTime",model.BUOperateTime),
                new SqlParameter("@BUIsValid",model.BUIsValid),
            };
            long result = 0;
            var ds = SqlHelper.ExecuteDataSet(CommandType.Text, sql.ToString(),tran, para);
            if (ds != null && ds.Tables.Count > 0)
            {
                var IdString = ds.Tables[0].Rows[0][0].ToString();
                result = string.IsNullOrEmpty(IdString) ? 0 : long.Parse(IdString);
            }
            return result;
        }

        public List<User> GetUserByPhone(string phoneNum)
        {
            var list = new List<User>();
            var user = new User();
            string sql = @"SELECT Id,BUSurname,BUGivenname,BUJobNumber,BUSex,BUAvatars,BUPhoneNum,BUEmail,BUDepartId,BUIsValid,BUTitle FROM " + tableName
                + " WHERE BUPhoneNum=@BUPhoneNum AND " +
                " BUIsValid=@BUIsValid";
            SqlParameter[] para = {
                new SqlParameter("@BUIsValid",EnabledEnum.Enabled.GetHashCode()),
                new SqlParameter("@BUPhoneNum", phoneNum)
            };
            var ds = SqlHelper.ExecuteDataSet(CommandType.Text, sql, null,para);
            if (ds != null && ds.Tables.Count > 0)
            {
                var dt = new DataTable();
                dt = ds.Tables[0];
                list = DataConvertHelper.DataTableToList<User>(dt);
            }
            return list;
        }

        /// <summary>
        /// 描述：根据工号获取用户
        /// </summary>
        /// <param name="jobnumber"></param>
        /// <returns></returns>
        public List<User> GetUserByJobNumber(string jobnumber)
        {
            var list = new List<User>();
            var user = new User();
            string sql = @"SELECT Id,BUSurname,BUGivenname,BUJobNumber,BUSex,BUAvatars,BUPhoneNum,BUEmail,BUDepartId,BUIsValid,BUTitle FROM " + tableName
                + " WHERE BUJobNumber=@BUJobNumber AND " +
                " BUIsValid=@BUIsValid";
            SqlParameter[] para = {
                new SqlParameter("@BUIsValid",EnabledEnum.Enabled.GetHashCode()),
                new SqlParameter("@BUJobNumber", jobnumber)
            };
            var ds = SqlHelper.ExecuteDataSet(CommandType.Text, sql, null,para);
            if (ds != null && ds.Tables.Count > 0)
            {
                var dt = new DataTable();
                dt = ds.Tables[0];
                list = DataConvertHelper.DataTableToList<User>(dt);
            }
            return list;
        }
    }
}
