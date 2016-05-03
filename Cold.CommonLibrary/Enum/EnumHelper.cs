using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Cold.CommonLibrary
{
    public class EnumHelper
    {
        /// <summary>
        /// 根据Description获取枚举
        /// 说明：
        /// 单元测试-->通过
        /// </summary>
        /// <typeparam name="T">枚举类型</typeparam>
        /// <param name="description">枚举描述</param>
        /// <returns>枚举</returns>
        public static T GetEnumName<T>(string description)
        {
            Type _type = typeof(T);
            foreach (FieldInfo field in _type.GetFields())
            {
                DescriptionAttribute[] _curDesc = (DescriptionAttribute[])field.GetCustomAttributes(typeof(DescriptionAttribute), false);
                if (_curDesc != null && _curDesc.Length > 0)
                {
                    if (_curDesc[0].Description == description)
                        return (T)field.GetValue(null);
                }
                else
                {
                    if (field.Name == description)
                        return (T)field.GetValue(null);
                }
            }
            return default(T);
        }


        #region 获取枚举的描述信息
        /// <summary>
        /// 获取枚举的描述信息
        /// </summary>
        /// <param name="e"></param>
        /// <returns></returns>
        public static string GetDesc(Enum e)
        {
            try
            {
                FieldInfo EnumInfo = e.GetType().GetField(e.ToString());
                if (EnumInfo != null)
                {
                    DescriptionAttribute[] EnumAttributes
                        = (DescriptionAttribute[])EnumInfo.GetCustomAttributes(typeof(DescriptionAttribute), false);
                    if (EnumAttributes.Length > 0)
                    {
                        return EnumAttributes[0].Description;
                    }
                }
            }
            catch { }
            return e.ToString();

        }

        #endregion
    }
}
