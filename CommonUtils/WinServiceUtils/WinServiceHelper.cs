using System;
using System.Collections;
using System.Configuration.Install;
using System.Linq;
using System.ServiceProcess;

namespace CommonUtils.WinServiceUtils
{
    public class WinServiceHelper
    {
        /// <summary>
        /// 打开系统服务
        /// </summary>
        /// <param name="serviceName">系统服务名称</param>
        /// <returns></returns>
        public static bool SystemServiceOpen(string serviceName)
        {
            try
            {
                using (var control = new ServiceController(serviceName))
                {
                    if (control.Status != ServiceControllerStatus.Running)
                    {
                        control.Start();
                    }
                }
                return true;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// 关闭系统服务
        /// </summary>
        /// <param name="serviceName">系统服务名称</param>
        /// <returns></returns>
        public static bool SystemServiceClose(string serviceName)
        {
            try
            {
                using (var control = new ServiceController(serviceName))
                {
                    if (control.Status == ServiceControllerStatus.Running)
                    {
                        control.Stop();
                    }
                }
                return true;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// 重启系统服务
        /// </summary>
        /// <param name="serviceName">系统服务名称</param>
        /// <returns></returns>
        public static bool SystemServiceReStart(string serviceName)
        {
            try
            {
                using (var control = new ServiceController(serviceName))
                {
                    if (control.Status == System.ServiceProcess.ServiceControllerStatus.Running)
                    {
                        control.Continue();
                    }
                }
                return true;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// 返回服务状态
        /// </summary>
        /// <param name="serviceName">系统服务名称</param>
        /// <returns>1:服务未运行 2:服务正在启动 3:服务正在停止 4:服务正在运行 5:服务即将继续 6:服务即将暂停 7:服务已暂停 0:未知状态</returns>
        public static int GetSystemServiceStatus(string serviceName)
        {
            try
            {
                using (var control = new ServiceController(serviceName))
                {
                    return (int)control.Status;
                }
            }
            catch
            {
                return 0;
            }
        }

        /// <summary>
        /// 返回服务状态
        /// </summary>
        /// <param name="serviceName">系统服务名称</param>
        /// <returns>1:服务未运行 2:服务正在启动 3:服务正在停止 4:服务正在运行 5:服务即将继续 6:服务即将暂停 7:服务已暂停 0:未知状态</returns>
        public static string GetSystemServiceStatusString(string serviceName)
        {
            try
            {
                using (var control = new ServiceController(serviceName))
                {
                    var status = string.Empty;
                    switch ((int)control.Status)
                    {
                        case 1:
                            status = "服务未运行";
                            break;

                        case 2:
                            status = "服务正在启动";
                            break;

                        case 3:
                            status = "服务正在停止";
                            break;

                        case 4:
                            status = "服务正在运行";
                            break;

                        case 5:
                            status = "服务即将继续";
                            break;

                        case 6:
                            status = "服务即将暂停";
                            break;

                        case 7:
                            status = "服务已暂停";
                            break;

                        case 0:
                            status = "未知状态";
                            break;
                    }
                    return status;
                }
            }
            catch
            {
                return "未知状态";
            }
        }

        /// <summary>
        /// 安装服务 必须新建domain，否则再次调用方法时将installer的属性仍是第一次调用时的值，用完AssemblyInstaller必须unload
        /// </summary>
        /// <param name="stateSaver"></param>
        /// <param name="filepath"></param>
        public static void InstallService(IDictionary stateSaver, string filepath)
        {
            try
            {
                var domain = AppDomain.CreateDomain("MyDomain");
                using (AssemblyInstaller installer = domain.CreateInstanceAndUnwrap(typeof(AssemblyInstaller).Assembly.FullName, typeof(AssemblyInstaller).FullName) as AssemblyInstaller)
                {
                    installer.UseNewContext = true;
                    installer.Path = filepath;
                    installer.Install(stateSaver);
                    installer.Commit(stateSaver);
                }
                AppDomain.Unload(domain);
            }
            catch (Exception ex)
            {
                throw new Exception("installServiceError/n" + ex.Message);
            }
        }

        public static bool ServiceIsExisted(string serviceName)
        {
            ServiceController[] services = ServiceController.GetServices();
            return services.Any(s => s.ServiceName.ToLower() == serviceName.ToLower());
        }

        /// <summary>
        /// 卸载服务 必须新建domain，否则再次调用方法时将installer的属性仍是第一次调用时的值，用完AssemblyInstaller必须unload
        /// </summary>
        /// <param name="filepath">路径和文件名</param>
        public static void UnInstallService(string filepath)
        {
            try
            {
                var domain = AppDomain.CreateDomain("MyDomain");
                using (AssemblyInstaller installer = domain.CreateInstanceAndUnwrap(typeof(AssemblyInstaller).Assembly.FullName, typeof(AssemblyInstaller).FullName) as AssemblyInstaller)
                {
                    installer.UseNewContext = true;
                    installer.Path = filepath;
                    installer.Uninstall(null);
                }
                AppDomain.Unload(domain);
            }
            catch (Exception ex)
            {
                throw new Exception("unInstallServiceError/n" + ex.Message);
            }
        }
    }
}