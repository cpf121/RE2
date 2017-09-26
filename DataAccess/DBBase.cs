using System.Collections.Generic;
using System.Text;
using System.Data;
using System.Data.SqlClient;
using System.Collections;
using System;
using System.Configuration;
using System.Data.Common;
using System.Text.RegularExpressions;
using System.Data.OleDb;
using System.Reflection;
using System.Web;
using Common.Costant;

namespace DataAccess
{
    /// <summary>
    /// 数据库的通用访问代码
    /// 此类为抽象类，
    /// 不允许实例化，在应用时直接调用即可
    /// </summary>
    public abstract class SqlHelper
    {
        /// <summary>
        /// 数据库连接字符串
        /// </summary>

        //public static string connectionString = ConfigurationManager.ConnectionStrings["ConnectionString2"].ConnectionString;
        //// Hashtable to store cached parameters
        private static Hashtable parmCache = Hashtable.Synchronized(new Hashtable());

        public SqlHelper() { }

        private static SqlConnection connec;

        public static SqlConnection Connection
        {
            get
            {
                string connectionString = string.Empty;
                //TODO:切换库
                //if (HttpContext.Current.Session[SessionKey.SESSION_KEY_DBINFO] != null)
                //{
                //    switch (HttpContext.Current.Session[SessionKey.SESSION_KEY_DBINFO])
                //    {
                //        case "0":
                //            connectionString = ConfigurationManager.ConnectionStrings["DefaultConnectingString"].ConnectionString;
                //            break;
                //        default:
                //            connectionString = ConfigurationManager.ConnectionStrings["DefaultConnectingString"].ConnectionString;
                //            break;
                //    }
                //}
                //else
                //{
                //    throw new Exception("您已离开太久请重新登录");
                //}
                connectionString = ConfigurationManager.ConnectionStrings["DefaultConnectingString"].ConnectionString;
                if (connec == null)
                {
                    connec = new SqlConnection(connectionString);
                    connec.Open();
                }
                else if (connec.State == System.Data.ConnectionState.Closed)
                {
                    connec = new SqlConnection(connectionString);
                    connec.Open();
                }
                else if (connec.State == System.Data.ConnectionState.Broken)
                {
                    connec.Close();
                    connec.Open();
                }
                return connec;
            }
        }
        #region//ExecteNonQuery方法

        /// <summary>
        ///执行一个不需要返回值的SqlCommand命令，通过指定专用的连接字符串。
        /// 使用参数数组形式提供参数列表 
        /// </summary>
        /// <param name="connectionString">一个有效的数据库连接字符串</param>
        /// <param name="cmdType">SqlCommand命令类型 (存储过程， T-SQL语句， 等等。)</param>
        /// <param name="cmdText">存储过程的名字或者 T-SQL 语句</param>
        /// <param name="commandParameters">以数组形式提供SqlCommand命令中用到的参数列表</param>
        /// <returns>返回一个数值表示此SqlCommand命令执行后影响的行数</returns>
        public static int ExecteNonQuery(CommandType cmdType, string cmdText, SqlTransaction tran = null,SqlParameter[] commandParameters=null)
        {
            SqlCommand cmd = new SqlCommand();
            using (Connection)
            {
                //通过PrePareCommand方法将参数逐个加入到SqlCommand的参数集合中
                PrepareCommand(cmd, Connection, tran, cmdType, cmdText, commandParameters);
                int val = cmd.ExecuteNonQuery();
                //清空SqlCommand中的参数列表
                cmd.Parameters.Clear();
                return val;
            }
        }

        /// <summary>
        ///执行一个不需要返回值的SqlCommand命令，通过指定专用的连接字符串。
        /// 使用参数数组形式提供参数列表 
        /// </summary>
        /// <param name="cmdType">SqlCommand命令类型 (存储过程， T-SQL语句， 等等。)</param>
        /// <param name="cmdText">存储过程的名字或者 T-SQL 语句</param>
        /// <param name="commandParameters">以数组形式提供SqlCommand命令中用到的参数列表</param>
        /// <returns>返回一个数值表示此SqlCommand命令执行后影响的行数</returns>
        //public static int ExecteNonQuery(CommandType cmdType, string cmdText, params SqlParameter[] commandParameters)
        //{
        //    return ExecteNonQuery(cmdType, cmdText, commandParameters);
        //}

        /// <summary>
        ///存储过程专用
        /// </summary>
        /// <param name="cmdText">存储过程的名字</param>
        /// <param name="commandParameters">以数组形式提供SqlCommand命令中用到的参数列表</param>
        /// <returns>返回一个数值表示此SqlCommand命令执行后影响的行数</returns>
        public static int ExecteNonQueryProducts(string cmdText, params SqlParameter[] commandParameters)
        {
            return ExecteNonQuery(CommandType.StoredProcedure, cmdText,null, commandParameters);
        }

        /// <summary>
        ///Sql语句专用
        /// </summary>
        /// <param name="cmdText">T_Sql语句</param>
        /// <param name="commandParameters">以数组形式提供SqlCommand命令中用到的参数列表</param>
        /// <returns>返回一个数值表示此SqlCommand命令执行后影响的行数</returns>
        public static int ExecteNonQueryText(string cmdText, params SqlParameter[] commandParameters)
        {
            return ExecteNonQuery(CommandType.Text, cmdText,null, commandParameters);
        }

        #endregion
        #region//GetTable方法

        /// <summary>
        /// 执行一条返回结果集的SqlCommand，通过一个已经存在的数据库连接
        /// 使用参数数组提供参数
        /// </summary>
        /// <param name="connecttionString">一个现有的数据库连接</param>
        /// <param name="cmdTye">SqlCommand命令类型</param>
        /// <param name="cmdText">存储过程的名字或者 T-SQL 语句</param>
        /// <param name="commandParameters">以数组形式提供SqlCommand命令中用到的参数列表</param>
        /// <returns>返回一个表集合(DataTableCollection)表示查询得到的数据集</returns>
        public static DataTableCollection GetTable(CommandType cmdTye, string cmdText, SqlParameter[] commandParameters, SqlTransaction tran = null)
        {
            SqlCommand cmd = new SqlCommand();
            DataSet ds = new DataSet();
            using (Connection)
            {
                PrepareCommand(cmd, Connection, tran, cmdTye, cmdText, commandParameters);
                SqlDataAdapter adapter = new SqlDataAdapter();
                adapter.SelectCommand = cmd;
                adapter.Fill(ds);
            }
            DataTableCollection table = ds.Tables;
            return table;
        }

        /// <summary>
        /// 执行一条返回结果集的SqlCommand，通过一个已经存在的数据库连接
        /// 使用参数数组提供参数
        /// </summary>
        /// <param name="cmdTye">SqlCommand命令类型</param>
        /// <param name="cmdText">存储过程的名字或者 T-SQL 语句</param>
        /// <param name="commandParameters">以数组形式提供SqlCommand命令中用到的参数列表</param>
        /// <returns>返回一个表集合(DataTableCollection)表示查询得到的数据集</returns>
        //public static DataTableCollection GetTable(CommandType cmdTye, string cmdText, SqlParameter[] commandParameters)
        //{
        //    return GetTable(cmdTye, cmdText, commandParameters);
        //}


        /// <summary>
        /// 存储过程专用
        /// </summary>
        /// <param name="cmdText">存储过程的名字或者 T-SQL 语句</param>
        /// <param name="commandParameters">以数组形式提供SqlCommand命令中用到的参数列表</param>
        /// <returns>返回一个表集合(DataTableCollection)表示查询得到的数据集</returns>
        public static DataTableCollection GetTableProducts(string cmdText, SqlParameter[] commandParameters)
        {
            return GetTable(CommandType.StoredProcedure, cmdText, commandParameters);
        }

        /// <summary>
        /// Sql语句专用
        /// </summary>
        /// <param name="cmdText"> T-SQL 语句</param>
        /// <param name="commandParameters">以数组形式提供SqlCommand命令中用到的参数列表</param>
        /// <returns>返回一个表集合(DataTableCollection)表示查询得到的数据集</returns>
        public static DataTableCollection GetTableText(string cmdText,params SqlParameter[] commandParameters)
        {
            return GetTable(CommandType.Text, cmdText, commandParameters);
        }
        #endregion


        /// <summary>
        /// 为执行命令准备参数
        /// </summary>
        /// <param name="cmd">SqlCommand 命令</param>
        /// <param name="conn">已经存在的数据库连接</param>
        /// <param name="trans">数据库事物处理</param>
        /// <param name="cmdType">SqlCommand命令类型 (存储过程， T-SQL语句， 等等。)</param>
        /// <param name="cmdText">Command text，T-SQL语句 例如 Select * from Products</param>
        /// <param name="cmdParms">返回带参数的命令</param>
        private static void PrepareCommand(SqlCommand cmd, SqlConnection conn, SqlTransaction trans, CommandType cmdType, string cmdText, SqlParameter[] cmdParms)
        {
            //判断数据库连接状态
            if (conn.State != ConnectionState.Open)
                conn.Open();
            cmd.Connection = conn;
            cmd.CommandText = cmdText;
            //判断是否需要事物处理
            if (trans != null)
                cmd.Transaction = trans;
            cmd.CommandType = cmdType;
            if (cmdParms != null)
            {
                foreach (SqlParameter parm in cmdParms)
                    cmd.Parameters.Add(parm);
            }
        }

        /// <summary>
        /// Execute a SqlCommand that returns a resultset against the database specified in the connection string 
        /// using the provided parameters.
        /// </summary>
        /// <param name="connectionString">一个有效的数据库连接字符串</param>
        /// <param name="cmdType">SqlCommand命令类型 (存储过程， T-SQL语句， 等等。)</param>
        /// <param name="cmdText">存储过程的名字或者 T-SQL 语句</param>
        /// <param name="commandParameters">以数组形式提供SqlCommand命令中用到的参数列表</param>
        /// <returns>A SqlDataReader containing the results</returns>
        public static SqlDataReader ExecuteReader(CommandType cmdType, string cmdText,SqlTransaction tran=null, SqlParameter[] commandParameters=null)
        {
            SqlCommand cmd = new SqlCommand();
            // we use a try/catch here because if the method throws an exception we want to 
            // close the connection throw code, because no datareader will exist, hence the 
            // commandBehaviour.CloseConnection will not work
            try
            {
                PrepareCommand(cmd, Connection, tran, cmdType, cmdText, commandParameters);
                SqlDataReader rdr = cmd.ExecuteReader(CommandBehavior.CloseConnection);
                cmd.Parameters.Clear();
                return rdr;
            }
            catch
            {
                Connection.Close();
                throw;
            }
        }
        #region//ExecuteDataSet方法

        /// <summary>
        /// return a dataset
        /// </summary>
        /// <param name="connectionString">一个有效的数据库连接字符串</param>
        /// <param name="cmdType">SqlCommand命令类型 (存储过程， T-SQL语句， 等等。)</param>
        /// <param name="cmdText">存储过程的名字或者 T-SQL 语句</param>
        /// <param name="commandParameters">以数组形式提供SqlCommand命令中用到的参数列表</param>
        /// <returns>return a dataset</returns>
        public static DataSet ExecuteDataSet(out int result, CommandType cmdType, string cmdText,SqlTransaction tran=null, SqlParameter[] commandParameters=null)
        {
            SqlCommand cmd = new SqlCommand();
            try
            {
                PrepareCommand(cmd, Connection, tran, cmdType, cmdText, commandParameters);
                SqlDataAdapter da = new SqlDataAdapter();
                DataSet ds = new DataSet();
                da.SelectCommand = cmd;
                result = da.Fill(ds);
                cmd.Parameters.Clear();
                return ds;
            }
            catch
            {
                Connection.Close();
                throw;
            }
        }

        public static DataSet ExecuteDataSet(out int result, DataSet ds, CommandType cmdType, string cmdText,SqlTransaction tran=null, SqlParameter[] commandParameters=null)
        {
            SqlCommand cmd = new SqlCommand();
            try
            {
                PrepareCommand(cmd, Connection, tran, cmdType, cmdText, commandParameters);
                SqlDataAdapter da = new SqlDataAdapter();
                da.SelectCommand = cmd;
                result = da.Fill(ds);
                cmd.Parameters.Clear();
                return ds;
            }
            catch
            {
                Connection.Close();
                throw;
            }
        }

        public static DataTable ExecuteDataSet(out int result, DataTable dt, CommandType cmdType, string cmdText,SqlTransaction tran=null, SqlParameter[] commandParameters=null)
        {
            SqlCommand cmd = new SqlCommand();
            try
            {
                PrepareCommand(cmd, Connection, tran, cmdType, cmdText, commandParameters);
                SqlDataAdapter da = new SqlDataAdapter();
                da.SelectCommand = cmd;
                result = da.Fill(dt);
                cmd.Parameters.Clear();
                return dt;
            }
            catch
            {
                Connection.Close();
                throw;
            }
        }

        public static DataSet ExecuteDataSet(CommandType cmdType, string cmdText,SqlTransaction tran=null, SqlParameter[] commandParameters=null)
        {
            SqlCommand cmd = new SqlCommand();
            try
            {
                PrepareCommand(cmd, Connection, tran, cmdType, cmdText, commandParameters);
                SqlDataAdapter da = new SqlDataAdapter();
                DataSet ds = new DataSet();
                da.SelectCommand = cmd;
                da.Fill(ds);
                cmd.Parameters.Clear();
                return ds;
            }
            catch
            {
                Connection.Close();
                throw;
            }
        }

        public static int ExecuteDataSet(DataSet ds, CommandType cmdType, string cmdText,SqlTransaction tran, SqlParameter[] commandParameters=null)
        {
            SqlCommand cmd = new SqlCommand();
            try
            {
                PrepareCommand(cmd, Connection, tran, cmdType, cmdText, commandParameters);
                SqlDataAdapter da = new SqlDataAdapter();
                da.SelectCommand = cmd;
                int i = da.Fill(ds);
                cmd.Parameters.Clear();
                return i;
            }
            catch
            {
                Connection.Close();
                throw;
            }
        }

        /// <summary>
        /// 执行返回SQL第一个值
        /// </summary>
        /// <param name="cmdType"></param>
        /// <param name="cmdText"></param>
        /// <param name="commandParameters"></param>
        /// <returns></returns>
        public static int ExecuteCount(CommandType cmdType, string cmdText,SqlTransaction tran=null, SqlParameter[] commandParameters=null)
        {
            SqlCommand cmd = new SqlCommand();
            using (Connection)
            {
                //通过PrePareCommand方法将参数逐个加入到SqlCommand的参数集合中
                PrepareCommand(cmd, Connection, tran, cmdType, cmdText, commandParameters);
                int val = (int)cmd.ExecuteScalar();
                //清空SqlCommand中的参数列表
                cmd.Parameters.Clear();
                return val;
            }
        }
        /// <summary>
        /// 返回一个DataSet
        /// </summary>
        /// <param name="cmdType">SqlCommand命令类型 (存储过程， T-SQL语句， 等等。)</param>
        /// <param name="cmdText">存储过程的名字或者 T-SQL 语句</param>
        /// <param name="commandParameters">以数组形式提供SqlCommand命令中用到的参数列表</param>
        /// <returns>return a dataset</returns>
        //public static DataSet ExecuteDataSet(CommandType cmdType, string cmdText, params SqlParameter[] commandParameters)
        //{
        //    return ExecuteDataSet(cmdType, cmdText, commandParameters);
        //}

        /// <summary>
        /// 返回一个DataSet
        /// </summary>
        /// <param name="cmdText">存储过程的名字</param>
        /// <param name="commandParameters">以数组形式提供SqlCommand命令中用到的参数列表</param>
        /// <returns>return a dataset</returns>
        public static DataSet ExecuteDataSetProducts(string cmdText, params SqlParameter[] commandParameters)
        {
            return ExecuteDataSet(CommandType.StoredProcedure, cmdText,null, commandParameters);
        }

        public static int ExecuteDataSetProducts(DataSet ds, string cmdText, params SqlParameter[] commandParameters)
        {
            int result = 0;
            ExecuteDataSet(out result, ds, CommandType.StoredProcedure, cmdText,null, commandParameters);
            return result;
        }

        public static int ExecuteDataSetProducts(DataTable dt, string cmdText, params SqlParameter[] commandParameters)
        {
            int result = 0;
            ExecuteDataSet(out result, dt, CommandType.StoredProcedure, cmdText,null, commandParameters);
            return result;
        }

        /// <summary>
        /// 返回一个DataSet
        /// </summary>

        /// <param name="cmdText">T-SQL 语句</param>
        /// <param name="commandParameters">以数组形式提供SqlCommand命令中用到的参数列表</param>
        /// <returns>return a dataset</returns>
        public static DataSet ExecuteDataSetText(string cmdText, params SqlParameter[] commandParameters)
        {
            return ExecuteDataSet(CommandType.Text, cmdText,null, commandParameters);
        }


        public static DataView ExecuteDataSet(string sortExpression, string direction, CommandType cmdType, string cmdText,SqlTransaction tran=null, SqlParameter[] commandParameters=null)
        {
            SqlCommand cmd = new SqlCommand();
            try
            {
                PrepareCommand(cmd, Connection, tran, cmdType, cmdText, commandParameters);
                SqlDataAdapter da = new SqlDataAdapter();
                DataSet ds = new DataSet();
                da.SelectCommand = cmd;
                da.Fill(ds);
                DataView dv = ds.Tables[0].DefaultView;
                dv.Sort = sortExpression + " " + direction;
                return dv;
            }
            catch
            {
                Connection.Close();
                throw;
            }
        }

        //public static int UpdateDataSet(DataSet ds, string cmdText)
        //{
        //    if (ds == null)
        //    {
        //        throw new ArgumentNullException("dataSet");
        //    }
        //    SqlConnection conn = new SqlConnection(connectionString);
        //    SqlCommand cmd = new SqlCommand();
        //    try
        //    {

        //        if (conn.State != ConnectionState.Open)
        //            conn.Open();
        //        cmd.Connection = conn;
        //        cmd.CommandText = cmdText;
        //        cmd.CommandType = CommandType.StoredProcedure;
        //        int processCount = 0;
        //        foreach (DataTable dt in ds.Tables)
        //        {
        //            int columnsNumber = dt.Columns.Count;
        //            foreach (DataRow row in dt.Rows)
        //            {
        //                for (int cn = 0; cn < columnsNumber; cn++)
        //                {
        //                    cmd.Parameters.Add(@"P_" + dt.Columns[cn].ColumnName.ToString(), GetSqlDbType(dt.Columns[cn]), dt.Columns[cn].MaxLength);
        //                    cmd.Parameters[(@"P_" + dt.Columns[cn].ColumnName.ToString())].Value = row[cn].ToString();
        //                }
        //                int val = cmd.ExecuteNonQuery();
        //                processCount += val;
        //                //清空SqlCommand中的参数列表
        //                cmd.Parameters.Clear();
        //            }
        //        }
        //        return processCount;
        //    }
        //    catch
        //    {
        //        conn.Close();
        //        throw;
        //    }
        //}

        public static int InsertAndUpdateDataSet(DataSet ds)
        {
            if (ds == null)
            {
                throw new ArgumentNullException("dataSet");
            }
            SqlCommand cmd = new SqlCommand();
            try
            {
                cmd.Connection = Connection;

                cmd.CommandType = CommandType.StoredProcedure;
                int processCount = 0;
                foreach (DataTable dt in ds.Tables)
                {
                    int columnsNumber = dt.Columns.Count;
                    foreach (DataRow row in dt.Rows)
                    {
                        if (row.RowState == DataRowState.Added)
                        {

                            for (int cn = 0; cn < columnsNumber; cn++)
                            {
                                if (dt.Columns[cn].ColumnName.ToString().ToUpper().Contains("_DESC"))
                                    continue;
                                cmd.Parameters.Add(@"P_" + dt.Columns[cn].ColumnName.ToString(), GetSqlDbType(dt.Columns[cn]), dt.Columns[cn].MaxLength);
                                cmd.Parameters[(@"P_" + dt.Columns[cn].ColumnName.ToString())].Value = row[cn];
                            }
                            cmd.CommandText = "P_" + dt.TableName.ToUpper() + "_I";
                            int val = cmd.ExecuteNonQuery();
                            processCount += val;

                        }
                        else if (row.RowState == DataRowState.Modified)
                        {
                            for (int cn = 0; cn < columnsNumber; cn++)
                            {
                                if (dt.Columns[cn].ColumnName.ToString().ToUpper().Contains("_DESC"))
                                    continue;
                                cmd.Parameters.Add(@"P_" + dt.Columns[cn].ColumnName.ToString(), GetSqlDbType(dt.Columns[cn]), dt.Columns[cn].MaxLength);
                                cmd.Parameters[(@"P_" + dt.Columns[cn].ColumnName.ToString())].Value = row[cn];
                            }
                            cmd.CommandText = "P_" + dt.TableName.ToUpper() + "_U";
                            int val = cmd.ExecuteNonQuery();
                            processCount += val;
                        }
                        else if (row.RowState == DataRowState.Deleted)
                        {
                            for (int cn = 0; cn < dt.PrimaryKey.Length; cn++)
                            {
                                cmd.Parameters.Add(@"P_" + dt.Columns[cn].ColumnName.ToString(), GetSqlDbType(dt.Columns[cn]), dt.Columns[cn].MaxLength);
                                cmd.Parameters[(@"P_" + dt.Columns[cn].ColumnName.ToString())].Value = row[cn, DataRowVersion.Original];
                            }
                            cmd.CommandText = "P_" + dt.TableName.ToUpper() + "_D";
                            int val = cmd.ExecuteNonQuery();
                            processCount -= val;
                        }
                        cmd.Parameters.Clear();
                    }

                }
                return processCount;
            }
            catch
            {
                Connection.Close();
                throw;
            }
        }

        public static int ModifiedDataSet(DataSet ds)
        {
            if (ds == null)
            {
                throw new ArgumentNullException("dataSet");
            }
            SqlCommand cmd = new SqlCommand();
            try
            {
                cmd.Connection = Connection;

                cmd.CommandType = CommandType.StoredProcedure;
                int processCount = 0;
                foreach (DataTable dt in ds.Tables)
                {
                    cmd.CommandText = "P_" + dt.TableName.ToUpper() + "_U";
                    int columnsNumber = dt.Columns.Count;
                    foreach (DataRow row in dt.Rows)
                    {
                        if (row.RowState == DataRowState.Modified)
                        {
                            for (int cn = 0; cn < columnsNumber; cn++)
                            {
                                cmd.Parameters.Add(@"P_" + dt.Columns[cn].ColumnName.ToString(), GetSqlDbType(dt.Columns[cn]), dt.Columns[cn].MaxLength);
                                cmd.Parameters[(@"P_" + dt.Columns[cn].ColumnName.ToString())].Value = row[cn].ToString();
                            }
                            int val = cmd.ExecuteNonQuery();
                            processCount += val;
                            //清空SqlCommand中的参数列表
                            cmd.Parameters.Clear();
                        }
                    }
                }
                return processCount;
            }
            catch
            {
                Connection.Close();
                throw;
            }
        }

        public static int UpdateDataSet(DataSet ds, params string[] cmdText)
        {
            if (ds == null)
            {
                throw new ArgumentNullException("dataSet");
            }
            SqlCommand cmd = new SqlCommand();
            try
            {
                cmd.Connection = Connection;

                cmd.CommandType = CommandType.StoredProcedure;
                int processCount = 0;
                foreach (DataTable dt in ds.Tables)
                {
                    foreach (string s in cmdText)
                    {
                        if (("P_" + dt.TableName.ToUpper() + "_I").Equals(s))
                        {
                            cmd.CommandText = s;
                            int columnsNumber = dt.Columns.Count;
                            foreach (DataRow row in dt.Rows)
                            {
                                for (int cn = 0; cn < columnsNumber; cn++)
                                {
                                    cmd.Parameters.Add(@"P_" + dt.Columns[cn].ColumnName.ToString(), GetSqlDbType(dt.Columns[cn]), dt.Columns[cn].MaxLength);
                                    cmd.Parameters[(@"P_" + dt.Columns[cn].ColumnName.ToString())].Value = row[cn].ToString();
                                }
                                int val = cmd.ExecuteNonQuery();
                                processCount += val;
                                //清空SqlCommand中的参数列表
                                cmd.Parameters.Clear();
                            }
                        }
                    }

                }
                return processCount;
            }
            catch
            {
                Connection.Close();
                throw;
            }
        }
        #endregion


        #region // ExecuteScalar方法


        /// <summary>
        /// 返回第一行的第一列
        /// </summary>
        /// <param name="cmdType">SqlCommand命令类型 (存储过程， T-SQL语句， 等等。)</param>
        /// <param name="cmdText">存储过程的名字或者 T-SQL 语句</param>
        /// <param name="commandParameters">以数组形式提供SqlCommand命令中用到的参数列表</param>
        /// <returns>返回一个对象</returns>
        //public static object ExecuteScalar(CommandType cmdType, string cmdText, params SqlParameter[] commandParameters)
        //{
        //    return ExecuteScalar(cmdType, cmdText, commandParameters);
        //}

        /// <summary>
        /// 返回第一行的第一列存储过程专用
        /// </summary>
        /// <param name="cmdText">存储过程的名字</param>
        /// <param name="commandParameters">以数组形式提供SqlCommand命令中用到的参数列表</param>
        /// <returns>返回一个对象</returns>
        public static object ExecuteScalarProducts(string cmdText, params SqlParameter[] commandParameters)
        {
            return ExecuteScalar(CommandType.StoredProcedure, cmdText,null, commandParameters);
        }

        /// <summary>
        /// 返回第一行的第一列Sql语句专用
        /// </summary>
        /// <param name="cmdText">者 T-SQL 语句</param>
        /// <param name="commandParameters">以数组形式提供SqlCommand命令中用到的参数列表</param>
        /// <returns>返回一个对象</returns>
        public static object ExecuteScalarText(string cmdText, params SqlParameter[] commandParameters)
        {
            return ExecuteScalar(CommandType.Text, cmdText,null, commandParameters);
        }

        /// <summary>
        /// Execute a SqlCommand that returns the first column of the first record against the database specified in the connection string 
        /// using the provided parameters.
        /// </summary>
        /// <remarks>
        /// e.g.:  
        ///  Object obj = ExecuteScalar(connString, CommandType.StoredProcedure, "PublishOrders", new SqlParameter("@prodid", 24));
        /// </remarks>
        /// <param name="connectionString">一个有效的数据库连接字符串</param>
        /// <param name="cmdType">SqlCommand命令类型 (存储过程， T-SQL语句， 等等。)</param>
        /// <param name="cmdText">存储过程的名字或者 T-SQL 语句</param>
        /// <param name="commandParameters">以数组形式提供SqlCommand命令中用到的参数列表</param>
        /// <returns>An object that should be converted to the expected type using Convert.To{Type}</returns>
        public static object ExecuteScalar(CommandType cmdType, string cmdText,SqlTransaction tran=null, SqlParameter[] commandParameters=null)
        {
            SqlCommand cmd = new SqlCommand();

            using (Connection)
            {
                PrepareCommand(cmd, Connection, tran, cmdType, cmdText, commandParameters);
                object val = cmd.ExecuteScalar();
                cmd.Parameters.Clear();
                return val;
            }
        }

        /// <summary>
        /// Execute a SqlCommand that returns the first column of the first record against an existing database connection 
        /// using the provided parameters.
        /// </summary>
        /// <remarks>
        /// e.g.:  
        ///  Object obj = ExecuteScalar(connString, CommandType.StoredProcedure, "PublishOrders", new SqlParameter("@prodid", 24));
        /// </remarks>
        /// <param name="connectionString">一个有效的数据库连接字符串</param>
        /// <param name="cmdType">SqlCommand命令类型 (存储过程， T-SQL语句， 等等。)</param>
        /// <param name="cmdText">存储过程的名字或者 T-SQL 语句</param>
        /// <param name="commandParameters">以数组形式提供SqlCommand命令中用到的参数列表</param>
        /// <returns>An object that should be converted to the expected type using Convert.To{Type}</returns>
        public static object ExecuteScalar(SqlConnection connection, CommandType cmdType, string cmdText,SqlTransaction tran, SqlParameter[] commandParameters=null)
        {
            SqlCommand cmd = new SqlCommand();
            PrepareCommand(cmd, connection, tran, cmdType, cmdText, commandParameters);
            object val = cmd.ExecuteScalar();
            cmd.Parameters.Clear();
            return val;
        }

        #endregion


        /// <summary>
        /// add parameter array to the cache
        /// </summary>
        /// <param name="cacheKey">Key to the parameter cache</param>
        /// <param name="cmdParms">an array of SqlParamters to be cached</param>
        public static void CacheParameters(string cacheKey, params SqlParameter[] commandParameters)
        {
            parmCache[cacheKey] = commandParameters;
        }

        /// <summary>
        /// Retrieve cached parameters
        /// </summary>
        /// <param name="cacheKey">key used to lookup parameters</param>
        /// <returns>Cached SqlParamters array</returns>
        public static SqlParameter[] GetCachedParameters(string cacheKey)
        {
            SqlParameter[] cachedParms = (SqlParameter[])parmCache[cacheKey];
            if (cachedParms == null)
                return null;
            SqlParameter[] clonedParms = new SqlParameter[cachedParms.Length];
            for (int i = 0, j = cachedParms.Length; i < j; i++)
                clonedParms[i] = (SqlParameter)((ICloneable)cachedParms[i]).Clone();
            return clonedParms;
        }


        /// <summary>
        /// 检查是否存在
        /// </summary>
        /// <param name="strSql">Sql语句</param>
        /// <returns>bool结果</returns>
        public static bool Exists(string strSql)
        {
            int cmdresult = Convert.ToInt32(ExecuteScalar(CommandType.Text, strSql, null));
            if (cmdresult == 0)
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        /// <summary>
        /// 检查是否存在
        /// </summary>
        /// <param name="strSql">Sql语句</param>
        /// <param name="cmdParms">参数</param>
        /// <returns>bool结果</returns>
        public static bool Exists(string strSql, params SqlParameter[] cmdParms)
        {
            int cmdresult = Convert.ToInt32(ExecuteScalar(CommandType.Text, strSql,null, cmdParms));
            if (cmdresult == 0)
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        /// <summary>
        /// 检查是否存在
        /// </summary>
        /// <param name="strSql">Sql语句</param>
        /// <returns>bool结果</returns>
        public static bool ExistsProcedure(string strSql)
        {
            int cmdresult = Convert.ToInt32(ExecuteScalar(CommandType.StoredProcedure, strSql, null));
            if (cmdresult == 0)
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        /// <summary>
        /// 检查是否存在
        /// </summary>
        /// <param name="strSql">Sql语句</param>
        /// <param name="cmdParms">参数</param>
        /// <returns>bool结果</returns>
        public static bool ExistsProcedure(string strSql, params SqlParameter[] cmdParms)
        {
            int cmdresult = Convert.ToInt32(ExecuteScalar(CommandType.StoredProcedure, strSql,null, cmdParms));
            if (cmdresult == 0)
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        /// <summary>
        /// 检查是否存在
        /// </summary>
        /// <param name="strSql">Sql语句</param>
        /// <param name="cmdParms">参数</param>
        /// <returns>bool结果</returns>
        public static bool ExistsProcedure(string strSql, out string result, params SqlParameter[] cmdParms)
        {
            object resulto = ExecuteScalar(CommandType.StoredProcedure, strSql, null,cmdParms);
            result = Convert.ToString(resulto);
            int cmdresult = Convert.ToInt32(resulto);
            if (cmdresult == 0)
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        /// <summary>
        /// 执行一条计算查询结果语句，返回查询结果（object）。
        /// </summary>
        /// <param name="SQLString">计算查询结果语句</param>
        /// <returns>查询结果（object）</returns>
        public static object GetSingle(string SQLString)
        {
            using (Connection)
            {
                using (SqlCommand cmd = new SqlCommand(SQLString, Connection))
                {
                    try
                    {
                        Connection.Open();
                        object obj = cmd.ExecuteScalar();
                        if ((Object.Equals(obj, null)) || (Object.Equals(obj, System.DBNull.Value)))
                        {
                            return null;
                        }
                        else
                        {
                            return obj;
                        }
                    }
                    catch (System.Data.SqlClient.SqlException e)
                    {
                        Connection.Close();
                        throw e;
                    }
                }
            }
        }

        //dataset转实体类 
        public static IList<T> FillModel<T>(DataSet ds)
        {
            List<T> l = new List<T>();
            T model = default(T);

            if (ds.Tables[0].Columns[0].ColumnName == "rowId")
            {
                ds.Tables[0].Columns.Remove("rowId");
            }

            foreach (DataRow dr in ds.Tables[0].Rows)
            {


                model = Activator.CreateInstance<T>();

                foreach (DataColumn dc in dr.Table.Columns)
                {

                    PropertyInfo pi = model.GetType().GetProperty(dc.ColumnName);
                    if (dr[dc.ColumnName] != DBNull.Value)
                        pi.SetValue(model, dr[dc.ColumnName], null);
                    else
                        pi.SetValue(model, null, null);

                }
                l.Add(model);
            }

            return l;


        }

        /// <summary> 
        /// 将实体类转换成DataTable 
        /// </summary> 
        /// <typeparam name="T"></typeparam> 
        /// <param name="i_objlist"></param> 
        /// <returns></returns> 
        public static DataTable Fill<T>(IList<T> objlist)
        {
            if (objlist == null || objlist.Count <= 0)
            {
                return null;
            }
            DataTable dt = new DataTable(typeof(T).Name);
            DataColumn column;
            DataRow row;

            System.Reflection.PropertyInfo[] myPropertyInfo = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance);

            foreach (T t in objlist)
            {
                if (t == null)
                {
                    continue;
                }

                row = dt.NewRow();

                for (int i = 0, j = myPropertyInfo.Length; i < j; i++)
                {
                    System.Reflection.PropertyInfo pi = myPropertyInfo[i];

                    string name = pi.Name;

                    if (dt.Columns[name] == null)
                    {
                        column = new DataColumn(name, pi.PropertyType);
                        dt.Columns.Add(column);
                    }

                    row[name] = pi.GetValue(t, null);
                }

                dt.Rows.Add(row);
            }
            return dt;
        }

        public static SqlDbType GetSqlDbType(DataColumn columns)
        {
            if (columns.DataType.Name.Equals("Int32"))
                return SqlDbType.Int;
            else if (columns.DataType.Name.Equals("String"))
                return SqlDbType.NVarChar;
            else if (columns.DataType.Name.Equals("DateTime"))
                return SqlDbType.DateTime;
            else if (columns.DataType.Name.Equals("Boolean"))
                return SqlDbType.Bit;
            else if (columns.DataType.Name.Equals("Decimal"))
                return SqlDbType.Decimal;
            return SqlDbType.Char;
        }

        public static SqlTransaction BeginTransaction()
        {
            var tran = connec.BeginTransaction();
            return tran;
        }
    }
}