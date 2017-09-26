using System;
using System.Data;
using System.Data.SqlClient;

namespace DataAccess
{
    public class HomeDAL : SqlHelper
    {
        public static string P_OA_LOGIN_CHECK = "P_OA_LOGIN_CHECK";
        public static string P_OA_LOGIN_USERINFOR = "P_OA_LOGIN_USERINFOR";

        #region loginDetails

        public bool UserLogin(string loginName, string password, string isAdmin, out string workNo)
        {
            workNo = string.Empty;
            SqlParameter LoginNameparam = new SqlParameter(@"P_LoginName", string.IsNullOrEmpty(loginName) ? string.Empty : loginName);
            SqlParameter Passwordparam = new SqlParameter(@"P_Password", string.IsNullOrEmpty(password) ? string.Empty : password);
            SqlParameter isAdminparam = new SqlParameter(@"P_IsAdmin", string.IsNullOrEmpty(isAdmin) ? string.Empty : isAdmin);
            return SqlHelper.ExistsProcedure(P_OA_LOGIN_CHECK, out workNo, LoginNameparam, Passwordparam, isAdminparam);
        }

        public DataSet GetUserInfor(string workNo)
        {
            SqlParameter param = new SqlParameter(@"P_RoleWorkNo", string.IsNullOrEmpty(workNo) ? string.Empty : workNo);
            return SqlHelper.ExecuteDataSetProducts(P_OA_LOGIN_USERINFOR, param);
        }

        #endregion
    }
}
