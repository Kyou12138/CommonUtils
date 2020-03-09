using ICSharpCode.SharpZipLib.Zip;
using System;
using System.IO;

namespace CommonUtils.FileUtils
{
    public class FileHelper
    {
        /// <summary>
        /// 压缩文件
        /// </summary>
        /// <param name="zipFilePath">压缩文件的路径与名称</param>
        /// <param name="filePath">被压缩的文件路径</param>
        /// <param name="zipPwd">解压密码（null代表无密码)</param>
        /// <returns></returns>
        public static bool FileToZip(string zipFilePath, string filePath, string zipPwd = null)
        {
            try
            {
                FastZip fz = new FastZip();
                FileInfo file = new FileInfo(filePath);
                string fileName = file.Name;
                string dirName = file.DirectoryName;
                fz.Password = zipPwd;
                fz.CreateZip(zipFilePath, dirName, false, fileName);
                return true;
            }
            catch (Exception e)
            {
                LoggerHelper.Error("压缩文件【" + filePath + "】失败", e);
                return false;
            }
        }

        /// <summary>
        /// 压缩文件夹
        /// </summary>
        /// <param name="dirPath">被压缩的文件夹路径</param>
        /// <param name="zipPath">压缩文件夹的路径与名称</param>
        /// <param name="zipPwd">解压密码（null代表无密码）</param>
        /// <returns></returns>
        public static bool DirToZip(string dirPath, string zipPath, string zipPwd = null)
        {
            try
            {
                FastZip fz = new FastZip();
                fz.Password = zipPwd;
                fz.CreateZip(zipPath, dirPath, false, null);
                return true;
            }
            catch (Exception e)
            {
                LoggerHelper.Error("压缩文件夹【" + dirPath + "】失败", e);
                return false;
            }
        }

        /// <summary>
        /// 解压Zip
        /// </summary>
        /// <param name="dirPath"></param>
        /// <param name="zipPath"></param>
        /// <param name="zipPwd"></param>
        /// <returns></returns>
        public static bool ExtractZip(string dirPath, string zipPath, string zipPwd = null)
        {
            try
            {
                FastZip fz = new FastZip();
                fz.Password = zipPwd;
                fz.ExtractZip(zipPath, dirPath, null);
                return true;
            }
            catch (Exception e)
            {
                LoggerHelper.Error("解压文件【" + zipPath + "】失败", e);
                return false;
            }
        }

    }
}