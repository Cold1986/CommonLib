using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Cold.CommonLibrary
{
    public class ConvertHelper
    {
        public static T ByteArrayToObject<T>(Byte[] buffer)
        {
            return JsonConvert.DeserializeObject<T>(Encoding.UTF8.GetString(buffer));
        }
        public static byte[] ObjectToByteArray(Object obj)
        {
            if (obj == null)
                return null;
            return Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(obj));
        }

        /// <summary>
        /// object转换为decimal类型
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static decimal ToDec(object obj)
        {
            decimal result = 0;
            if (obj != null)
            {
                var success = decimal.TryParse(obj.ToString(), out result);
            }
            return result;
        }
        /// <summary>
        /// object转换为int类型
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static int ToInt(object obj)
        {
            int result = 0;
            if (obj != null)
            {
                var success = int.TryParse(obj.ToString(), out result);
            }
            return result;
        }

        public static long ToLong(object obj)
        {
            long result = 0;
            if (obj != null)
            {
                var success = long.TryParse(obj.ToString(), out result);
            }
            return result;
        }

        public static DateTime ToDateTime(object obj)
        {
            DateTime result = default(DateTime);
            if (obj != null)
            {
                var success = DateTime.TryParse(obj.ToString(), out result);
            }
            return result;
        }

        public static Double ToDouble(object obj)
        {
            Double reValue;
            Double.TryParse(obj.ToString(), out reValue);
            return reValue;
        }

        /// <summary>
        /// 字符串根据分割数组转换成HashSet
        /// </summary>
        /// <typeparam name="T">Set类型</typeparam>
        /// <param name="str">字符串</param>
        /// <param name="arry">分割字符</param>
        /// <param name="function"></param>
        /// <returns></returns>
        public static HashSet<T> ToHash<T>(string str, char[] arry, Func<string, T> function)
        {
            HashSet<T> set = new HashSet<T>();
            if (!string.IsNullOrEmpty(str) && function != null)
            {
                string[] arry1 = str.Split(arry, StringSplitOptions.RemoveEmptyEntries);
                if (arry1 != null && arry1.Any())
                {
                    foreach (string str1 in arry1)
                    {
                        set.Add(function(str1));
                    }
                }
            }
            return set;
        }

        /// <summary>
        /// 将实体值赋值到指定实体中
        /// </summary>
        /// <typeparam name="To">目标类型</typeparam>
        /// <typeparam name="From">源类型</typeparam>
        /// <param name="source">源数据</param>
        /// <returns></returns>
        public static To CopyEntityFromSource<To, From>(From source)
            where To : new()
            where From : new()
        {
            To result = new To();
            if (source == null)
            {
                return result;
            }
            Type Totype = typeof(To);
            Type FromType = typeof(From);
            PropertyInfo property = null;
            foreach (PropertyInfo currentPro in FromType.GetProperties(BindingFlags.Public | BindingFlags.Instance))
            {
                property = Totype.GetProperty(currentPro.Name);
                if (null != property)
                {
                    property.SetValue(result, currentPro.GetValue(source, null), null);
                }
            }
            return result;
        }

        /// <summary>
        /// 实现各进制数间的转换。ConvertBase("15",10,16)表示将十进制数15转换为16进制的数。
        /// </summary>
        /// <param name="value">要转换的值,即原值</param>
        /// <param name="from">原值的进制,只能是2,8,10,16四个值。</param>
        /// <param name="to">要转换到的目标进制，只能是2,8,10,16四个值。</param>
        public static string ConvertBase(string value, int from, int to)
        {
            try
            {
                int intValue = Convert.ToInt32(value, from);  //先转成10进制
                string result = Convert.ToString(intValue, to);  //再转成目标进制
                if (to == 2)
                {
                    result = result.PadLeft(8, '0');
                }
                return result;
            }
            catch
            {

                //LogHelper.WriteTraceLog(TraceLogLevel.Error, ex.Message);
                return "0";
            }
        }

        /// <summary>
        /// 使用指定字符集将string转换成byte[]
        /// </summary>
        /// <param name="text">要转换的字符串</param>
        /// <param name="encoding">字符编码</param>
        public static byte[] StringToBytes(string text, Encoding encoding)
        {
            return encoding.GetBytes(text);
        }

        /// <summary>
        /// 使用指定字符集将byte[]转换成string
        /// </summary>
        /// <param name="bytes">要转换的字节数组</param>
        /// <param name="encoding">字符编码</param>
        public static string BytesToString(byte[] bytes, Encoding encoding)
        {
            return encoding.GetString(bytes);
        }

        /// <summary>
        /// 将byte[]转换成int
        /// </summary>
        /// <param name="data">需要转换成整数的byte数组</param>
        public static int BytesToInt32(byte[] data)
        {
            //如果传入的字节数组长度小于4,则返回0
            if (data.Length < 4)
            {
                return 0;
            }

            //定义要返回的整数
            int num = 0;

            //如果传入的字节数组长度大于4,需要进行处理
            if (data.Length >= 4)
            {
                //创建一个临时缓冲区
                byte[] tempBuffer = new byte[4];

                //将传入的字节数组的前4个字节复制到临时缓冲区
                Buffer.BlockCopy(data, 0, tempBuffer, 0, 4);

                //将临时缓冲区的值转换成整数，并赋给num
                num = BitConverter.ToInt32(tempBuffer, 0);
            }

            //返回整数
            return num;
        }

        /// <summary>
        /// 将集合类转换成DataTable
        /// </summary>
        /// <param name="list">集合</param>
        public static DataTable ToDataTable(IList list)
        {
            DataTable result = new DataTable();
            if (list.Count > 0)
            {
                PropertyInfo[] propertys = list[0].GetType().GetProperties();
                foreach (PropertyInfo pi in propertys)
                {
                    result.Columns.Add(pi.Name, pi.PropertyType);
                }

                for (int i = 0; i < list.Count; i++)
                {
                    ArrayList tempList = new ArrayList();
                    foreach (PropertyInfo pi in propertys)
                    {
                        object obj = pi.GetValue(list[i], null);
                        tempList.Add(obj);
                    }
                    object[] array = tempList.ToArray();
                    result.LoadDataRow(array, true);
                }
            }
            return result;
        }

        /// <summary>
        /// 将泛型集合类转换成DataTable
        /// </summary>
        /// <typeparam name="T">集合项类型</typeparam>
        /// <param name="list">集合</param>
        /// <param name="propertyName">需要返回的列的列名</param>
        /// <returns>数据集(表)</returns>
        public static DataTable ToDataTable<T>(IList<T> list, params string[] propertyName)
        {
            List<string> propertyNameList = new List<string>();
            if (propertyName != null) propertyNameList.AddRange(propertyName);

            DataTable result = new DataTable();
            if (list.Count > 0)
            {
                PropertyInfo[] propertys = list[0].GetType().GetProperties();
                foreach (PropertyInfo pi in propertys)
                {
                    if (propertyNameList.Count == 0)
                    {
                        result.Columns.Add(pi.Name, pi.PropertyType);
                    }
                    else
                    {
                        if (propertyNameList.Contains(pi.Name)) result.Columns.Add(pi.Name, pi.PropertyType);
                    }
                }

                for (int i = 0; i < list.Count; i++)
                {
                    ArrayList tempList = new ArrayList();
                    foreach (PropertyInfo pi in propertys)
                    {
                        if (propertyNameList.Count == 0)
                        {
                            object obj = pi.GetValue(list[i], null);
                            tempList.Add(obj);
                        }
                        else
                        {
                            if (propertyNameList.Contains(pi.Name))
                            {
                                object obj = pi.GetValue(list[i], null);
                                tempList.Add(obj);
                            }
                        }
                    }
                    object[] array = tempList.ToArray();
                    result.LoadDataRow(array, true);
                }
            }
            return result;
        }
    }
}
