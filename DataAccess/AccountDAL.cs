using Model.TableModel;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;

namespace DataAccess
{
    public class AccountDAL : SqlHelper
    {
        private const string tableName = "T_BASE_ACCOUNT";
        /// <summary>
        /// 描述：根据登录账号获取账号信息
        /// 创建标识;cpf23568
        /// 创建时间：2017-9-10 21:36:38
        /// </summary>
        /// <param name="account"></param>
        /// <returns></returns>
        public Account GetAccountByAccount(string account)
        {
            var accountInfo = new Account();
            SqlParameter[] para = {
                new SqlParameter("@Account", account)
            };

            var sql = @"SELECT Id,BAAccount,BAPassword,BAUserId,BAType,BAIsValid FROM " + tableName
                + " WHERE BAAccount=@Account";

            using (SqlDataReader dr = SqlHelper.ExecuteReader(CommandType.Text, sql, null,para))
            {
                if (dr.Read())
                {
                    accountInfo.Id = Convert.ToInt32(dr["Id"]);
                    accountInfo.BAPassword = dr["BAPassword"].ToString();
                    accountInfo.BAAccount = dr["BAAccount"].ToString();
                    accountInfo.BAUserId = Convert.ToInt32(dr["BAUserId"]);
                    accountInfo.BAType = Convert.ToInt32(dr["BAType"]);
                    accountInfo.BAIsValid = Convert.ToInt32(dr["BAIsValid"]);
                }
                else
                {
                    accountInfo = null;
                }
            }

            return accountInfo;
        }
    }
}
