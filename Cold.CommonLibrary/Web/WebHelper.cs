using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace Cold.CommonLibrary.Web
{
    public class WebHelper
    {
        /// <summary>
        /// 获取当前url地址
        /// </summary>
        /// <returns></returns>
        public static string GetLocalUrl()
        {
            if (System.Web.HttpContext.Current.Request.Url != null)
            {
                return System.Web.HttpContext.Current.Request.Url.AbsoluteUri;
            }
            return string.Empty;
        }

        /// <summary>
        /// 获取当前地址主机头
        /// </summary>
        /// <returns></returns>
        public static string GetLocalHost()
        {
            if (System.Web.HttpContext.Current.Request.Url != null)
            {
                return System.Web.HttpContext.Current.Request.Url.Host;
            }
            return string.Empty;
        }

        /// <summary>
        /// 获取当前url地址（不包含参数）
        /// </summary>
        /// <returns></returns>
        public static string GetLocalUrlWithNoQuery()
        {
            if (System.Web.HttpContext.Current.Request.Url != null)
            {
                return System.Web.HttpContext.Current.Request.Url.AbsoluteUri.Split('?')[0];
            }
            return string.Empty;
        }

        /// <summary>
        /// 获取url请求参数
        /// </summary>
        /// <returns></returns>
        public static string GetQueryString()
        {
            string query = System.Web.HttpContext.Current.Request.QueryString.ToString();
            if (string.IsNullOrEmpty(query))
            {
                query = System.Web.HttpContext.Current.Request.Form.ToString();
            }
            else if (!string.IsNullOrEmpty(query))
            {
                query = HttpUtility.UrlDecode(query, Encoding.GetEncoding("gb2312"));
            }
            return query;
        }

        /// <summary>
        /// 获取客户端IP
        /// </summary>
        /// <returns></returns>
        public static string GetClientIp()
        {
            string returnResult = string.Empty;
            returnResult = System.Web.HttpContext.Current.Request.ServerVariables["HTTP_X_FORWARDED_FOR"];
            if (string.IsNullOrEmpty(returnResult))
            {
                returnResult = System.Web.HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];
            }
            if (string.IsNullOrEmpty(returnResult))
            {
                returnResult = System.Web.HttpContext.Current.Request.UserHostAddress;
            }
            if (string.IsNullOrEmpty(returnResult))
            {
                returnResult = "0.0.0.0";
            }
            return returnResult;
        }

        /// <summary>
        /// 获取本机的计算机名
        /// </summary>
        public static string LocalHostName
        {
            get
            {
                return Dns.GetHostName();
            }
        }

        /// <summary>
        /// 获取本机的局域网IP
        /// </summary>        
        public static string LANIP
        {
            get
            {
                //获取本机的IP列表,IP列表中的第一项是局域网IP，第二项是广域网IP
                IPAddress[] addressList = Dns.GetHostEntry(Dns.GetHostName()).AddressList;

                //如果本机IP列表为空，则返回空字符串
                if (addressList.Length < 1)
                {
                    return "";
                }

                //返回本机的局域网IP
                return addressList[0].ToString();
            }
        }

        /// <summary>
        /// 获取本机在Internet网络的广域网IP
        /// </summary>        
        public static string WANIP
        {
            get
            {
                //获取本机的IP列表,IP列表中的第一项是局域网IP，第二项是广域网IP
                IPAddress[] addressList = Dns.GetHostEntry(Dns.GetHostName()).AddressList;

                //如果本机IP列表小于2，则返回空字符串
                if (addressList.Length < 2)
                {
                    return "";
                }

                //返回本机的广域网IP
                return addressList[1].ToString();
            }
        }

        #region 获取ip4地址
        /// <summary>
        /// 获取ip4地址
        /// </summary>
        /// <returns></returns>
        public static string GetIP4Address()
        {
            string IP4Address = String.Empty;

            foreach (IPAddress IPA in Dns.GetHostAddresses(HttpContext.Current.Request.UserHostAddress))
            {
                if (IPA.AddressFamily.ToString() == "InterNetwork")
                {
                    IP4Address = IPA.ToString();
                    break;
                }
            }

            if (!string.IsNullOrEmpty(IP4Address))
            {
                return IP4Address;
            }

            foreach (IPAddress IPA in Dns.GetHostAddresses(Dns.GetHostName()))
            {
                if (IPA.AddressFamily.ToString() == "InterNetwork")
                {
                    IP4Address = IPA.ToString();
                    break;
                }
            }
            return IP4Address;
        }
        #endregion

        /// <summary>
        /// 请求参数排序
        /// </summary>
        public static string OrderByRequestParam()
        {
            var resultStr = string.Empty;
            List<string> requestParamList = new List<string>();
            if (System.Web.HttpContext.Current.Request.QueryString.AllKeys.Length > 0)
            {
                foreach (var item in System.Web.HttpContext.Current.Request.QueryString.AllKeys)
                {
                    if (item == null) { continue; }
                    if (item.ToLower() != "sign" && item.ToLower() != "callback")
                    {
                        requestParamList.Add(item.ToLower() + "=" + System.Web.HttpContext.Current.Request.QueryString[item]);
                    }
                }
            }
            else if (System.Web.HttpContext.Current.Request.Form.AllKeys.Length > 0)
            {
                foreach (var item in System.Web.HttpContext.Current.Request.Form.AllKeys)
                {
                    if (item == null) { continue; }
                    if (item.ToLower() != "sign" && item.ToLower() != "callback")
                    {
                        requestParamList.Add(item.ToLower() + "=" + System.Web.HttpContext.Current.Request.Form[item]);
                    }
                }
            }
            requestParamList = requestParamList.OrderBy(p => p).ToList();
            for (int i = 0; i < requestParamList.Count; i++)
            {
                resultStr += requestParamList[i] + "&";
            }
            return resultStr;
        }

        /// <summary>
        /// 根据传入对象属性进行排序，返回字符串，空属性将被过滤
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="tmodel">需要被排序的泛型对象</param>
        /// <returns></returns>
        public static string OrderByObjectParam<T>(T tmodel) where T : class
        {
            Type type = tmodel.GetType();
            StringBuilder str = new StringBuilder();
            foreach (PropertyInfo info in type.GetProperties().OrderBy(t => t.Name.ToLower()).ToArray())
            {
                if (info.Name.ToLower() == "sign")
                {
                    continue;
                }
                str.Append(GetParamAndValue(tmodel, info.Name));
            }
            return str.ToString();
        }

        /// <summary>
        /// 根据传入对象和属性名称，获取属性值
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="tmodel">泛型对象</param>
        /// <param name="attributeName">属性名称</param>
        /// <returns></returns>
        public static string GetParamAndValue<T>(T tmodel, string attributeName) where T : class
        {
            string value = "";
            MemberInfo[] myMember = tmodel.GetType().GetMember(attributeName);
            if (myMember.Length > 0)
            {
                object obj = null;
                switch (myMember[0].MemberType)
                {
                    case System.Reflection.MemberTypes.Field:
                        obj = ((System.Reflection.FieldInfo)myMember[0]).GetValue(tmodel);
                        break;
                    case System.Reflection.MemberTypes.Property:
                        obj = ((System.Reflection.PropertyInfo)myMember[0]).GetValue(tmodel, null);
                        break;
                    default:
                        break;
                }
                value = obj == null ? "" : obj.ToString();
            }
            if (value.Length > 0)
            {
                return attributeName.ToLower() + "=" + value + "&";
            }
            else if (attributeName.ToLower() == "platid")
            {
                return attributeName.ToLower() + "=" + value + "&";
            }
            else
            {
                return "";
            }
        }

        /// <summary>
        /// 验证客户端请求方式是否是POST方式
        /// </summary>
        /// <returns></returns>
        public static bool CheckRequestMethod()
        {
            bool ispost = false;
            switch (System.Web.HttpContext.Current.Request.HttpMethod.ToUpper())
            {
                case "POST": ispost = true; break;
                default: ispost = false; break;
            }
            return ispost;
        }

    }
}
