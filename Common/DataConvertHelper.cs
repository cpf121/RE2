using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Common
{
    public class DataConvertHelper
    {
        #region DataRow转实体
        /// <summary>
        /// DataRow转实体
        /// </summary>
        /// <typeparam name="T">数据型类</typeparam>
        /// <param name="dr">DataRow</param>
        /// <returns>模式</returns>
        public static T DataRowToModel<T>(DataRow dr) where T : new()
        {
            //T t = (T)Activator.CreateInstance(typeof(T));
            T t = new T();
            if (dr == null) return default(T);
            // 获得此模型的公共属性
            PropertyInfo[] propertys = t.GetType().GetProperties();
            DataColumnCollection Columns = dr.Table.Columns;
            foreach (PropertyInfo p in propertys)
            {
                string columnName = p.Name;
                //columnName = ((DBColumnAttribute)p.GetCustomAttributes(typeof(DBColumnAttribute), false)[0]).ColName;

                if (Columns.Contains(columnName))
                {
                    // 判断此属性是否有Setter或columnName值是否为空
                    object value = dr[columnName];
                    if (!p.CanWrite || value is DBNull || value == DBNull.Value) continue;
                    try
                    {
                        #region SetValue
                        switch (p.PropertyType.ToString())
                        {
                            case "System.String":
                                p.SetValue(t, Convert.ToString(value), null);
                                break;
                            case "System.Int32":
                                p.SetValue(t, Convert.ToInt32(value), null);
                                break;
                            case "System.Int64":
                                p.SetValue(t, Convert.ToInt64(value), null);
                                break;
                            case "System.DateTime":
                                p.SetValue(t, Convert.ToDateTime(value), null);
                                break;
                            case "System.Boolean":
                                p.SetValue(t, Convert.ToBoolean(value), null);
                                break;
                            case "System.Double":
                                p.SetValue(t, Convert.ToDouble(value), null);
                                break;
                            case "System.Decimal":
                                p.SetValue(t, Convert.ToDecimal(value), null);
                                break;
                            default:
                                p.SetValue(t, value, null);
                                break;
                        }
                        #endregion
                    }
                    catch
                    {
                        continue;
                    }
                }
            }
            return t;
        }
        #endregion

        #region DataTable转List<T>
        /// <summary>
        /// DataTable转List<T>
        /// </summary>
        /// <typeparam name="T">数据项类型</typeparam>
        /// <param name="dt">DataTable</param>
        /// <returns>List数据集</returns>
        public static List<T> DataTableToList<T>(DataTable dt) where T : new()
        {
            List<T> tList = new List<T>();
            if (dt != null && dt.Rows.Count > 0)
            {
                foreach (DataRow dr in dt.Rows)
                {
                    T t = DataRowToModel<T>(dr);
                    tList.Add(t);
                }
            }
            return tList;
        }
        #endregion

        #region DataReader转实体
        /// <summary>
        /// DataReader转实体
        /// </summary>
        /// <typeparam name="T">数据类型</typeparam>
        /// <param name="dr">DataReader</param>
        /// <returns>实体</returns>
        public static T DataReaderToModel<T>(IDataReader dr) where T : new()
        {
            T t = new T();
            if (dr == null) return default(T);
            using (dr)
            {
                if (dr.Read())
                {
                    // 获得此模型的公共属性
                    PropertyInfo[] propertys = t.GetType().GetProperties();
                    List<string> listFieldName = new List<string>(dr.FieldCount);
                    for (int i = 0; i < dr.FieldCount; i++)
                    {
                        listFieldName.Add(dr.GetName(i).ToLower());
                    }

                    foreach (PropertyInfo p in propertys)
                    {
                        string columnName = p.Name;
                        if (listFieldName.Contains(columnName.ToLower()))
                        {
                            // 判断此属性是否有Setter或columnName值是否为空
                            object value = dr[columnName];
                            if (!p.CanWrite || value is DBNull || value == DBNull.Value) continue;
                            try
                            {
                                #region SetValue
                                switch (p.PropertyType.ToString())
                                {
                                    case "System.String":
                                        p.SetValue(t, Convert.ToString(value), null);
                                        break;
                                    case "System.Int32":
                                        p.SetValue(t, Convert.ToInt32(value), null);
                                        break;
                                    case "System.DateTime":
                                        p.SetValue(t, Convert.ToDateTime(value), null);
                                        break;
                                    case "System.Boolean":
                                        p.SetValue(t, Convert.ToBoolean(value), null);
                                        break;
                                    case "System.Double":
                                        p.SetValue(t, Convert.ToDouble(value), null);
                                        break;
                                    case "System.Decimal":
                                        p.SetValue(t, Convert.ToDecimal(value), null);
                                        break;
                                    default:
                                        p.SetValue(t, value, null);
                                        break;
                                }
                                #endregion
                            }
                            catch
                            {
                                //throw (new Exception(ex.Message));
                            }
                        }
                    }
                }
            }
            return t;
        }
        #endregion

        #region DataReader转List<T>
        /// <summary>
        /// DataReader转List<T>
        /// </summary>
        /// <typeparam name="T">数据类型</typeparam>
        /// <param name="dr">DataReader</param>
        /// <returns>List数据集</returns>
        public static List<T> DataReaderToList<T>(IDataReader dr) where T : new()
        {
            List<T> tList = new List<T>();
            if (dr == null) return tList;
            T t1 = new T();
            // 获得此模型的公共属性
            PropertyInfo[] propertys = t1.GetType().GetProperties();
            using (dr)
            {
                List<string> listFieldName = new List<string>(dr.FieldCount);
                for (int i = 0; i < dr.FieldCount; i++)
                {
                    listFieldName.Add(dr.GetName(i).ToLower());
                }
                while (dr.Read())
                {
                    T t = new T();
                    foreach (PropertyInfo p in propertys)
                    {
                        string columnName = p.Name;
                        if (listFieldName.Contains(columnName.ToLower()))
                        {
                            // 判断此属性是否有Setter或columnName值是否为空
                            object value = dr[columnName];
                            if (!p.CanWrite || value is DBNull || value == DBNull.Value) continue;
                            try
                            {
                                #region SetValue
                                switch (p.PropertyType.ToString())
                                {
                                    case "System.String":
                                        p.SetValue(t, Convert.ToString(value), null);
                                        break;
                                    case "System.Int32":
                                        p.SetValue(t, Convert.ToInt32(value), null);
                                        break;
                                    case "System.DateTime":
                                        p.SetValue(t, Convert.ToDateTime(value), null);
                                        break;
                                    case "System.Boolean":
                                        p.SetValue(t, Convert.ToBoolean(value), null);
                                        break;
                                    case "System.Double":
                                        p.SetValue(t, Convert.ToDouble(value), null);
                                        break;
                                    case "System.Decimal":
                                        p.SetValue(t, Convert.ToDecimal(value), null);
                                        break;
                                    default:
                                        p.SetValue(t, value, null);
                                        break;
                                }
                                #endregion
                            }
                            catch
                            {
                                //throw (new Exception(ex.Message));
                            }
                        }
                    }
                    tList.Add(t);
                }
            }
            return tList;
        }
        #endregion

        #region 泛型集合转DataTable
        /// <summary>
        /// 泛型集合转DataTable
        /// </summary>
        /// <typeparam name="T">集合类型</typeparam>
        /// <param name="entityList">泛型集合</param>
        /// <returns>DataTable</returns>
        public static DataTable ListToDataTable<T>(IList<T> entityList)
        {
            if (entityList == null) return null;
            DataTable dt = CreateTable<T>();
            Type entityType = typeof(T);
            //PropertyInfo[] properties = entityType.GetProperties();
            PropertyDescriptorCollection properties = TypeDescriptor.GetProperties(entityType);
            foreach (T item in entityList)
            {
                DataRow row = dt.NewRow();
                foreach (PropertyDescriptor property in properties)
                {
                    row[property.Name] = property.GetValue(item);
                }
                dt.Rows.Add(row);
            }

            return dt;
        }

        #endregion

        #region 创建DataTable的结构
        private static DataTable CreateTable<T>()
        {
            Type entityType = typeof(T);
            //PropertyInfo[] properties = entityType.GetProperties();
            PropertyDescriptorCollection properties = TypeDescriptor.GetProperties(entityType);
            //生成DataTable的结构
            DataTable dt = new DataTable();
            foreach (PropertyDescriptor prop in properties)
            {
                dt.Columns.Add(prop.Name);
            }
            return dt;
        }
        #endregion
    }
}
