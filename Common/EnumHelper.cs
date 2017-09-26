using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Web.Mvc;

namespace Common
{
    public class EnumHelper
    {
        public static List<EnumDataModel> GetEnumDataList<T>()
        {
            return EnumUtilData<T>.enumDataList;
        }
        public static List<SelectListItem> SelectListEnum<T>(int? defaultValue = null, bool addChose = true, List<T> removeEnum = null) where T : struct
        {
            var enumSelectListItem = new List<SelectListItem>();
            if (addChose)
            {
                var listItem = new SelectListItem { Text = "全部", Value = "-1" };
                enumSelectListItem.Add(listItem);
            }
            var enumDataList = EnumHelper.GetEnumDataList<T>();
            foreach (var item in enumDataList)
            {
                if (removeEnum != null)
                {
                    var isNext = removeEnum.All(ritem => item.Value != ritem.GetHashCode());
                    if (!isNext)
                    {
                        continue;
                    }
                }
                bool b1 = defaultValue != null && item.Value == defaultValue;
                enumSelectListItem.Add(new SelectListItem { Text = item.Description, Value = item.Value.ToString(), Selected = b1 });
            }
            return enumSelectListItem;
        }

        /// <summary>
        /// 内部实现类，缓存
        /// </summary>
        /// <typeparam name="Tenum">枚举类型</typeparam>
        private static class EnumUtilData<Tenum>
        {
            internal static readonly List<EnumDataModel> enumDataList;

            static EnumUtilData()
            {
                enumDataList = InitData();
            }

            private static List<EnumDataModel> InitData()
            {
                List<EnumDataModel> enumDataList = new List<EnumDataModel>();

                EnumDataModel enumData = new EnumDataModel();
                Type t = typeof(Tenum);
                FieldInfo[] fieldInfoList = t.GetFields();
                foreach (FieldInfo tField in fieldInfoList)
                {
                    if (!tField.IsSpecialName)
                    {
                        enumData = new EnumDataModel();
                        enumData.Name = tField.Name;
                        enumData.Value= ((Tenum)System.Enum.Parse(t, enumData.Name)).GetHashCode();

                        DescriptionAttribute[] enumAttributelist= (DescriptionAttribute[])tField.GetCustomAttributes(typeof(DescriptionAttribute), false);
                        if (enumAttributelist != null && enumAttributelist.Length > 0)
                        {
                            enumData.Description = enumAttributelist[0].Description;
                        }
                        else
                        {
                            enumData.Description = tField.Name;
                        }
                        enumDataList.Add(enumData);
                    }
                }
                return enumDataList;
            }
        }

        public class EnumDataModel
        {
            /// <summary>
            /// get or set 枚举名称
            /// </summary>
            public string Name { get; set; }

            /// <summary>
            /// get or set 枚举值
            /// </summary>
            public int Value { get; set; }

            /// <summary>
            /// get or set 枚举描述
            /// </summary>
            public string Description { get; set; }
        }
    }
}
