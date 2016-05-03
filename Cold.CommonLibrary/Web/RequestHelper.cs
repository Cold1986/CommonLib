using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace Cold.CommonLibrary.Web
{
    public class RequestHelper
    {
        /// <summary>
        /// 通讯函数
        /// </summary>
        /// <param name="url">请求Url</param>
        /// <param name="para">请求参数</param>
        /// <param name="method">请求方式GET/POST</param>
        /// <returns></returns>
        public static string SendRequest(string url, string para, string method = "GET")
        {
            string strResult = "";
            if (url == null || url == "")
                return null;
            if (method == null || method == "")
                method = "GET";
            // GET方式
            if (method.ToUpper() == "GET")
            {
                try
                {
                    System.Net.WebRequest wrq = System.Net.WebRequest.Create(url + para);
                    wrq.Method = "GET";
                    ServicePointManager.SecurityProtocol = SecurityProtocolType.Ssl3;
                    System.Net.WebResponse wrp = wrq.GetResponse();
                    System.IO.StreamReader sr = new System.IO.StreamReader(wrp.GetResponseStream(), System.Text.Encoding.GetEncoding("gb2312"));
                    strResult = sr.ReadToEnd();
                }
                catch (Exception ex)
                {
                    return ex.Message;
                }
            }
            // POST方式
            else if (method.ToUpper() == "POST")
            {
                if (para.Length > 0 && para.IndexOf('?') == 0)
                {
                    para = para.Substring(1);
                }
                WebRequest req = WebRequest.Create(url);
                req.Method = "POST";
                req.ContentType = "application/x-www-form-urlencoded";
                
                ServicePointManager.SecurityProtocol = SecurityProtocolType.Ssl3;
                StringBuilder UrlEncoded = new StringBuilder();
                Char[] reserved = { '?', '=', '&' };
                byte[] SomeBytes = null;
                if (para != null)
                {
                    int i = 0, j;
                    while (i < para.Length)
                    {
                        j = para.IndexOfAny(reserved, i);
                        if (j == -1)
                        {
                            UrlEncoded.Append(HttpUtility.UrlEncode(para.Substring(i, para.Length - i), System.Text.Encoding.GetEncoding("gb2312")));
                            break;
                        }
                        UrlEncoded.Append(HttpUtility.UrlEncode(para.Substring(i, j - i), System.Text.Encoding.GetEncoding("gb2312")));
                        UrlEncoded.Append(para.Substring(j, 1));
                        i = j + 1;
                    }
                    SomeBytes = Encoding.Default.GetBytes(UrlEncoded.ToString());
                    req.ContentLength = SomeBytes.Length;
                    Stream newStream = req.GetRequestStream();
                    newStream.Write(SomeBytes, 0, SomeBytes.Length);
                    newStream.Close();
                }
                else
                {
                    req.ContentLength = 0;
                }
                try
                {
                    WebResponse result = req.GetResponse();
                    Stream ReceiveStream = result.GetResponseStream();
                    Byte[] read = new Byte[512];
                    int bytes = ReceiveStream.Read(read, 0, 512);
                    while (bytes > 0)
                    {
                        // 注意：
                        // 下面假定响应使用 UTF-8 作为编码方式。
                        // 如果内容以 ANSI 代码页形式（例如，932）发送，则使用类似下面的语句：
                        // Encoding encode = System.Text.Encoding.GetEncoding("shift-jis");
                        Encoding encode = System.Text.Encoding.GetEncoding("gb2312");
                        strResult += encode.GetString(read, 0, bytes);
                        bytes = ReceiveStream.Read(read, 0, 512);
                    }
                    return strResult;
                }
                catch (Exception ex)
                {
                    return ex.Message;
                }
            }
            return strResult;
        }

        public static string GetHtml(string url)
        {
            Encoding encoding = Encoding.GetEncoding("utf-8");
            string html = string.Empty;
            try
            {
                WebRequest request;
                request = WebRequest.Create(url);
                request.Credentials = CredentialCache.DefaultCredentials;
                request.Timeout = 20000;
                WebResponse response;
                response = request.GetResponse();
                html = new StreamReader(response.GetResponseStream(), encoding).ReadToEnd();
            }
            catch (System.UriFormatException uex)
            {
                Console.WriteLine(uex.Message);
            }
            catch (System.Net.WebException ex)
            {
                Console.WriteLine(ex.Message);
            }
            return html;
        }

        #region 同步通过POST方式发送数据
        /// <summary>
        /// 通过POST方式发送数据
        /// </summary>
        /// <param name="Url">url</param>
        /// <param name="postDataStr">Post数据</param>
        /// <param name="cookie">Cookie容器</param>
        /// <returns></returns>
        public static string SendDataByPost(string Url, string postDataStr, ref CookieContainer cookie)
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(Url);
            if (cookie.Count == 0)
            {
                request.CookieContainer = new CookieContainer();
                cookie = request.CookieContainer;
            }
            else
            {
                request.CookieContainer = cookie;
            }

            request.Referer = "http://3g.uhut.com/phone/activity?from=singlemessage&isappinstalled=0";
            request.Method = "POST";
            request.ContentType = "application/x-www-form-urlencoded";
            request.ContentLength = postDataStr.Length;
            //request.KeepAlive = true;
            Stream myRequestStream = request.GetRequestStream();
            StreamWriter myStreamWriter = new StreamWriter(myRequestStream, Encoding.GetEncoding("gb2312"));
            myStreamWriter.Write(postDataStr);
            myStreamWriter.Close();

            HttpWebResponse response = (HttpWebResponse)request.GetResponse();
            Stream myResponseStream = response.GetResponseStream();
            StreamReader myStreamReader = new StreamReader(myResponseStream, Encoding.GetEncoding("utf-8"));
            string retString = myStreamReader.ReadToEnd();
            myStreamReader.Close();
            myResponseStream.Close();

            return retString;
        }
        #endregion

        #region 同步通过GET方式发送数据
        /// <summary>
        /// 通过GET方式发送数据
        /// </summary>
        /// <param name="Url">url</param>
        /// <param name="postDataStr">GET数据</param>
        /// <param name="cookie">GET容器</param>
        /// <returns></returns>
        public static string SendDataByGET(string Url, string postDataStr, ref CookieContainer cookie)
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(Url + (postDataStr == "" ? "" : "?") + postDataStr);
            if (cookie.Count == 0)
            {
                request.CookieContainer = new CookieContainer();
                cookie = request.CookieContainer;
            }
            else
            {
                request.CookieContainer = cookie;
            }
            request.Referer = "http://3g.uhut.com/phone/activity?from=singlemessage&isappinstalled=0";
            request.Method = "GET";
            request.ContentType = "text/html;charset=UTF-8";
            //request.KeepAlive = true;
            HttpWebResponse response = (HttpWebResponse)request.GetResponse();
            Stream myResponseStream = response.GetResponseStream();
            StreamReader myStreamReader = new StreamReader(myResponseStream, Encoding.GetEncoding("utf-8"));
            string retString = myStreamReader.ReadToEnd();
            myStreamReader.Close();
            myResponseStream.Close();

            return retString;
        }
        #endregion
    }
}
