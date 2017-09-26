using Common;
using Common.Enum;
using Model.Material;
using Model.TableModel;
using Model.ViewModel.Department;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Text;

namespace DataAccess
{
    public class MaterialDAL : SqlHelper
    {
        private const string tableName = "T_MATERIAL_WORKORDER_INFO";

        /// <summary>
        /// 插入数据
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public int Insert(MaterialInfoModel model)
        {
            var sql = @"INSERT INTO " + tableName +
                "([MICustomerPart],[MIProductName],[MICustomer],[MIPicture],[MIIsValid],[MICreateUserId],[MICreateUserName],[MICreateTime],[MIOperateUserId],[MIOperateUserName],[MIOperateTime],[MIWorkOrder],[MIMaterial],[MIMaterialText],[MITool],[MITotalQty])" +
                " VALUES (@MICustomerPart,@MIProductName,@MICustomer,@MIPicture,@MIIsValid,@MICreateUserId,@MICreateUserName,@MICreateTime,@MIOperateUserId,@MIOperateUserName,@MIOperateTime,@MIWorkOrder,@MIMaterial,@MIMaterialText,@MITool,@MITotalQty) " +
                "  select id = scope_identity()";
            SqlParameter[] para = {
                new SqlParameter("@MICustomerPart",model.MICustomerPart),
                new SqlParameter("@MIProductName",model.MIProductName),
                new SqlParameter("@MICustomer",model.MICustomer),
                new SqlParameter("@MIPicture",model.MIPicture),
                new SqlParameter("@MIIsValid",model.MIIsValid),
                new SqlParameter("@MICreateUserId",model.MICreateUserId),
                new SqlParameter("@MICreateUserName",model.MICreateUserName),
                new SqlParameter("@MICreateTime",model.MICreateTime),
                new SqlParameter("@MIOperateUserId",model.MIOperateUserId),
                new SqlParameter("@MIOperateUserName",model.MIOperateUserName),
                new SqlParameter("@MIOperateTime",model.MIOperateTime),
                new SqlParameter("@MIWorkOrder",model.MIWorkOrder),
                new SqlParameter("@MIMaterial",model.MIMaterial),
                new SqlParameter("@MIMaterialText",model.MIMaterialText),
                new SqlParameter("@MITool",model.MITool),
                new SqlParameter("@MITotalQty",model.MITotalQty),
            };
            var result = 0;
            var ds = SqlHelper.ExecuteDataSet(CommandType.Text, sql.ToString(), null,para);
            if (ds != null && ds.Tables.Count > 0)
            {
                var Idstring = ds.Tables[0].Rows[0][0].ToString();
                result = string.IsNullOrEmpty(Idstring) ? 0 : Convert.ToInt32(Idstring);
            }
            return result;
        }

        public bool Update(MaterialInfoModel model)
        {
            var sql = @"UPDATE  " + tableName +
                 @" SET [MICustomerPart] = @MICustomerPart
                      ,[MIProductName] = @MIProductName
                      ,[MICustomer] = @MICustomer
                      ,[MIPicture] = @MIPicture
                      ,[MIIsValid] = @MIIsValid
                      ,[MIOperateUserId] = @MIOperateUserId
                      ,[MIOperateUserName] = @MIOperateUserName
                      ,[MIOperateTime] = @MIOperateTime
                      ,[MIWorkOrder] = @MIWorkOrder
                      ,[MIMaterial] = @MIMaterial
                      ,[MIMaterialText] = @MIMaterialText
                      ,[MITool] = @MITool
                      ,[MITotalQty] = @MITotalQty WHERE Id = @Id";
            SqlParameter[] para = {
                new SqlParameter("@Id",model.Id),
               new SqlParameter("@MICustomerPart",model.MICustomerPart),
                new SqlParameter("@MIProductName",model.MIProductName),
                new SqlParameter("@MICustomer",model.MICustomer),
                new SqlParameter("@MIPicture",model.MIPicture),
                new SqlParameter("@MIIsValid",model.MIIsValid),
                new SqlParameter("@MIOperateUserId",model.MIOperateUserId),
                new SqlParameter("@MIOperateUserName",model.MIOperateUserName),
                new SqlParameter("@MIOperateTime",model.MIOperateTime),
                new SqlParameter("@MIWorkOrder",model.MIWorkOrder),
                new SqlParameter("@MIMaterial",model.MIMaterial),
                new SqlParameter("@MIMaterialText",model.MIMaterialText),
                new SqlParameter("@MITool",model.MITool),
                new SqlParameter("@MITotalQty",model.MITotalQty),
            };
            return SqlHelper.ExecteNonQuery(CommandType.Text, sql, null,para) > 0;
        }

        /// <summary>
        /// 描述：获取所有的组织框架
        /// 创建标识：cpf
        /// 创建时间：2017-9-19 20:37:36
        /// </summary>
        /// <returns></returns>
        public List<MaterialInfoModel> SearchMaterialPageList(MaterialSearchModel param, out int totalCount)
        {
            var list = new List<MaterialInfoModel>();
            var selectSql = new StringBuilder();
            var countSql = new StringBuilder();
            var whereSql = new StringBuilder();
            whereSql.Append(" WHERE 1 = 1 ");
            if (string.IsNullOrEmpty(param.ProductName))
            {
                whereSql.Append(string.Format(" AND MIProductName like '%{0}'", param.ProductName));
            }
            if (string.IsNullOrEmpty(param.CustomerPart))
            {
                whereSql.Append(string.Format(" AND MICustomerPart like '%{0}'", param.CustomerPart));
            }
            if (string.IsNullOrEmpty(param.WorkOrder))
            {
                whereSql.Append(string.Format(" AND MIWorkOrder like '%{0}'", param.WorkOrder));
            }
            selectSql.Append(string.Format(@"
                SELECT  newTable.*
                FROM    ( 
                        SELECT TOP ( {0} * {1} )
                                ROW_NUMBER() OVER ( ORDER BY MIOperateTime DESC) RowNum
                                ,[Id]
                                ,[MICustomerPart]
                                ,[MIProductName]
                                ,[MICustomer]
                                ,[MIPicture]
                                ,[MIIsValid]
                                ,[MICreateUserId]
                                ,[MICreateUserName]
                                ,[MICreateTime]
                                ,[MIOperateUserId]
                                ,[MIOperateUserName]
                                ,[MIOperateTime]
                                ,[MIWorkOrder]
                                ,[MIMaterial]
                                ,[MIMaterialText]
                                ,[MITool]
                                ,[MITotalQty]
                            FROM {2} with(NOLOCK) {3} 
                            ORDER BY MIOperateTime DESC) newTable
                WHERE   newTable.RowNum > ( ( {0} - 1 ) * {1} )  
            ", param.CurrentPage, param.PageSize, tableName, whereSql.ToString()));
            countSql.Append(string.Format(@"SELECT COUNT(1) FROM {0} with(NOLOCK) {1} ", tableName, whereSql.ToString()));

            var ds = SqlHelper.ExecuteDataSet(CommandType.Text, selectSql.ToString());
            totalCount = SqlHelper.ExecuteCount(CommandType.Text, countSql.ToString());
            if (ds != null && ds.Tables.Count > 0)
            {
                DataTable dt = new DataTable();
                dt = ds.Tables[0];
                list = DataConvertHelper.DataTableToList<MaterialInfoModel>(dt);
            }
            return list;
        }


        /// <summary>
        /// 描述：获取物料信息
        /// </summary>
        /// <param name="materialId">物料Id</param>
        /// <returns></returns>
        public MaterialInfoModel GetMaterialById(int materialId)
        {
            var material = new MaterialInfoModel();
            SqlParameter[] para = {
                new SqlParameter("@Id", materialId)
            };

            var sql = @"SELECT TOP 1 [Id]
                        ,[MICustomerPart]
                        ,[MIProductName]
                        ,[MICustomer]
                        ,[MIPicture]
                        ,[MIIsValid]
                        ,[MICreateUserId]
                        ,[MICreateUserName]
                        ,[MICreateTime]
                        ,[MIOperateUserId]
                        ,[MIOperateUserName]
                        ,[MIOperateTime]
                        ,[MIWorkOrder]
                        ,[MIMaterial]
                        ,[MIMaterialText]
                        ,[MITool]
                        ,[MITotalQty]
                        FROM " + tableName + " WHERE Id=@Id";

            var ds = SqlHelper.ExecuteDataSet(CommandType.Text, sql.ToString(), null,para);
            if (ds != null && ds.Tables.Count > 0)
            {
                DataTable dt = new DataTable();
                dt = ds.Tables[0];
                material = DataConvertHelper.DataTableToList<MaterialInfoModel>(dt)[0];
            }
            else
            {
                return null;
            }
            return material;
        }
    }
}
