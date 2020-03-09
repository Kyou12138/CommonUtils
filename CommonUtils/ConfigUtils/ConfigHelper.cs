using System;
using System.Configuration;
using System.IO;
using System.Xml.Serialization;

namespace CommonUtils.ConfigUtils
{
    public class ConfigHelper
    {
        /// <summary>
        /// 在app.congig的AppSetting节点中获取key值对应的value
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public static string GetAppSettingConfig(string key)
        {
            string _value = string.Empty;
            Configuration config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
            if (config.AppSettings.Settings[key] != null)
            {
                _value = config.AppSettings.Settings[key].Value;
            }
            return _value;
        }

        /// <summary>
        /// 读取Config文件
        /// </summary>
        /// <param name="configFilePath"></param>
        /// <returns></returns>
        public static T GetConfig<T>(string configFilePath)
        {
            try
            {
                XmlSerializer xs = new XmlSerializer(typeof(T));
                StreamReader sr = new StreamReader(configFilePath);
                T t = (T)(xs.Deserialize(sr));
                sr.Close();
                return t;
            }
            catch (Exception e)
            {
                LoggerHelper.Error(string.Format("读取{0}配置失败", configFilePath), e);
                throw e;
            }
        }

        /// <summary>
        /// 保存Config文件
        /// </summary>
        /// <param name="t"></param>
        /// <param name="configFilePath"></param>
        public static void SaveConfig<T>(T t, string configFilePath)
        {
            try
            {
                XmlSerializer xs = new XmlSerializer(typeof(T));
                StreamWriter sw = new StreamWriter(configFilePath);
                xs.Serialize(sw, t);
                sw.Close();
            }
            catch (Exception e)
            {
                LoggerHelper.Error(string.Format("保存{0}配置失败", configFilePath), e);
                throw e;
            }
        }
    }
}