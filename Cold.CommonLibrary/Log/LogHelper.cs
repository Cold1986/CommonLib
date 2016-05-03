using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using log4net;
using log4net.Config;

namespace Cold.CommonLibrary
{
    public class LogHelper
    {
        public string Path { get; set; }
         
        public  LogHelper(string path)
        {
            this._loginfo = LogHandler.GetLogger(path);
        }

        private   ILog _loginfo = LogManager.GetLogger("loginfo");
        private ILog _logerror = LogManager.GetLogger("logerror");

        private  bool _isConfigured = false;

        public  ILog GetLogger(Type type)
        {
            return GetLogger(string.Empty, type);
        }

        public  ILog GetLogger(string path, Type type)
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

        public  ILog GetLogger(string name)
        {
            if (!_isConfigured)
            {
                DOMConfigurator.Configure();

                _isConfigured = true;
            }

            return LogManager.GetLogger(name);
        }

        public  void SetConfig()
        {
            log4net.Config.DOMConfigurator.Configure();
        }

        public  void SetConfig(FileInfo configFile)
        {
            log4net.Config.DOMConfigurator.Configure(configFile);
        }

        public  void WriteLog(string info)
        {
            if (_loginfo.IsInfoEnabled)
            {
                _loginfo.Info(info);
            }
        }

        public  void WriteLog(string info, Exception se)
        {
            if (_logerror.IsErrorEnabled)
            {
                _logerror.Error(info, se);
            }
        }
    }
}
