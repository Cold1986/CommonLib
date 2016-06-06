using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using log4net;
using log4net.Config;

[assembly: log4net.Config.XmlConfigurator(Watch = true)]
namespace Cold.CommonTools
{
    [Serializable]
    public class LogHandler
    {
        public static readonly ILog _loginfo = LogManager.GetLogger("loginfo");

        public static readonly ILog _logerror = LogManager.GetLogger("logerror");

        private static bool _isConfigured = false;

        public static ILog GetLogger(Type type)
        {
            return GetLogger(string.Empty, type);
        }

        public static ILog GetLogger(string path, Type type)
        {
            if (!_isConfigured)
            {
                if (!string.IsNullOrEmpty(path))
                {
                    FileInfo fileInfo = new FileInfo(path);
                    if (fileInfo.Exists)
                    {
                        DOMConfigurator.Configure(fileInfo);
                    }
                    else
                    {
                        DOMConfigurator.Configure();
                    }
                }
                else
                {
                    DOMConfigurator.Configure();
                }

                _isConfigured = true;
            }

            return LogManager.GetLogger(type);
        }

        public static ILog GetLogger(string name)
        {
            if (!_isConfigured)
            {
                DOMConfigurator.Configure();

                _isConfigured = true;
            }

            return LogManager.GetLogger(name);
        }

        public static void SetConfig()
        {
            log4net.Config.DOMConfigurator.Configure();
        }

        public static void SetConfig(FileInfo configFile)
        {
            log4net.Config.DOMConfigurator.Configure(configFile);
        }

        public static void WriteLog(string info)
        {
            if (_loginfo.IsInfoEnabled)
            {
                _loginfo.Info(info);
            }
        }

        public static void WriteLog(string info, Exception se)
        {
            if (_logerror.IsErrorEnabled)
            {
                _logerror.Error(info, se);
            }
        }
    }
}
