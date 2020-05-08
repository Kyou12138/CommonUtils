using CommonUtils.ConfigUtils;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.Text;

namespace CommonUtils.FtpUtils
{
    public class FtpHelper
    {
        /// <summary>
        /// 配置文件位置
        /// </summary>
        private static readonly string FtpConfigFilePath = @"FtpUtils\ftp.config";

        /// <summary>
        /// 配置文件
        /// </summary>
        public FtpConfig config;

        /// <summary>
        /// ip + path
        /// </summary>
        private string ftpUri;

        /// <summary>
        /// FTP请求对象
        /// </summary>
        private FtpWebRequest request;

        /// <summary>
        /// FTP响应对象
        /// </summary>
        private FtpWebResponse response;

        #region 构造函数

        public FtpHelper()
        {
            if (config == null) config = ConfigHelper.GetConfig<FtpConfig>(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, FtpConfigFilePath));
            ftpUri = string.Format("ftp://{0}:{1}/", config.FtpServerIp, config.FtpServerPort);
        }

        public FtpHelper(FtpConfig ftpConfig)
        {
            config = ftpConfig;
            ftpUri = string.Format("ftp://{0}:{1}/", config.FtpServerIp, config.FtpServerPort);
        }

        ~FtpHelper()
        {
            if (response != null)
            {
                response.Close();
                response = null;
            }
            if (request != null)
            {
                request.Abort();
                request = null;
            }
            if (Directory.Exists("temp"))
            {
                Directory.Delete("temp");
            }
        }

        #endregion 构造函数

        #region 方法

        /// <summary>
        /// 建立FTP链接,返回响应对象
        /// </summary>
        /// <param name="uri">FTP地址</param>
        /// <param name="ftpMethod">操作命令</param>
        private FtpWebResponse Open(Uri uri, string ftpMethod)
        {
            try
            {
                request = (FtpWebRequest)FtpWebRequest.Create(uri);
                request.Method = ftpMethod;
                request.UseBinary = config.UseBinary;
                request.KeepAlive = config.KeepAlive;
                request.UsePassive = config.UsePassive;//被动模式
                request.EnableSsl = config.EnableSsl;
                request.Credentials = new NetworkCredential(config.FtpUserName, config.FtpPassword);
                request.Timeout = config.Timeout;
                //首次连接FTP Server时，会有一个证书分配过程。
                //根据验证过程，远程证书无效。
                ServicePoint sp = request.ServicePoint;
                ServicePointManager.ServerCertificateValidationCallback = new RemoteCertificateValidationCallback(ValidateServerCertificate);
                return (FtpWebResponse)request.GetResponse();
            }
            catch (Exception e)
            {
                LoggerHelper.Error("获取【" + uri + "】FTP响应对象失败", e);
                return null;
            }
        }

        /// <summary>
        /// 证书验证回调
        /// </summary>
        private bool ValidateServerCertificate(object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors)
        {
            return true;
        }

        /// <summary>
        /// 建立FTP链接,返回请求对象
        /// </summary>
        /// <param name="uri">FTP地址</param>
        /// <param name="ftpMethod">操作命令</param>
        private FtpWebRequest OpenRequest(Uri uri, string ftpMethod)
        {
            try
            {
                request = (FtpWebRequest)WebRequest.Create(uri);
                request.Method = ftpMethod;
                request.UseBinary = config.UseBinary;
                request.KeepAlive = config.KeepAlive;
                request.UsePassive = config.UsePassive;//被动模式
                request.EnableSsl = config.EnableSsl;
                request.Credentials = new NetworkCredential(config.FtpUserName, config.FtpPassword);
                request.Timeout = config.Timeout;

                ServicePoint sp = request.ServicePoint;
                ServicePointManager.ServerCertificateValidationCallback = new RemoteCertificateValidationCallback(ValidateServerCertificate);
                return request;
            }
            catch (Exception e)
            {
                LoggerHelper.Error("获取【" + uri + "】FTP请求对象失败", e);
                return null;
            }
        }

        /// <summary>
        /// 下载文件
        /// </summary>
        /// <param name="remoteFileName">远程文件</param>
        /// <param name="localFileName">本地文件</param>
        public bool Get(string remoteFileName, string localFileName)
        {
            response = Open(new Uri(ftpUri + remoteFileName), WebRequestMethods.Ftp.DownloadFile);
            if (response == null) return false;

            try
            {
                using (FileStream outputStream = new FileStream(localFileName, FileMode.Create))
                {
                    using (Stream ftpStream = response.GetResponseStream())
                    {
                        long length = response.ContentLength;
                        int bufferSize = 2048;
                        int readCount;
                        byte[] buffer = new byte[bufferSize];
                        readCount = ftpStream.Read(buffer, 0, bufferSize);
                        while (readCount > 0)
                        {
                            outputStream.Write(buffer, 0, readCount);
                            readCount = ftpStream.Read(buffer, 0, bufferSize);
                        }
                    }
                }
                return true;
            }
            catch (Exception e)
            {
                LoggerHelper.Error("下载【" + remoteFileName + "】失败", e);
                return false;
            }
        }

        /// <summary>
        /// 文件上传
        /// </summary>
        /// <param name="localFileName">本地文件</param>
        /// <param name="localFileName">远程文件</param>
        public bool Put(string localFileName, string remoteFileName)
        {
            FileInfo fi = new FileInfo(localFileName);
            if (fi.Exists == false) return false;
            request = OpenRequest(new Uri(ftpUri + remoteFileName), WebRequestMethods.Ftp.UploadFile);
            if (request == null) return false;

            request.ContentLength = fi.Length;
            int buffLength = 2048;
            byte[] buff = new byte[buffLength];
            int contentLen;
            try
            {
                using (var fs = fi.OpenRead())
                {
                    using (var strm = request.GetRequestStream())
                    {
                        contentLen = fs.Read(buff, 0, buffLength);
                        while (contentLen != 0)
                        {
                            strm.Write(buff, 0, contentLen);
                            contentLen = fs.Read(buff, 0, buffLength);
                        }
                    }
                }
                return true;
            }
            catch (Exception e)
            {
                LoggerHelper.Error("上传【" + localFileName + "】失败", e);
                return false;
            }
        }

        /// <summary>
        /// 删除文件
        /// </summary>
        public bool DeleteFile(string fileName)
        {
            response = Open(new Uri(ftpUri + fileName), WebRequestMethods.Ftp.DeleteFile);
            return response == null ? false : true;
        }

        /// <summary>
        /// 创建目录
        /// </summary>
        public bool CreateDirectory(string dirName)
        {
            response = Open(new Uri(ftpUri + dirName), WebRequestMethods.Ftp.MakeDirectory);
            return response == null ? false : true;
        }

        /// <summary>
        /// 删除目录(包括下面所有子目录和子文件)
        /// </summary>
        public bool DeleteDirectory(string dirName)
        {
            var listAll = GetDirectoryAndFiles(dirName);
            if (listAll == null) return false;

            foreach (var m in listAll)
            {
                if (m.IsDirectory)
                    DeleteDirectory(m.Path);
                else
                    DeleteFile(m.Path);
            }
            response = Open(new Uri(ftpUri + dirName), WebRequestMethods.Ftp.RemoveDirectory);
            return response == null ? false : true;
        }

        /// <summary>
        /// 获取目录的文件和一级子目录信息
        /// </summary>
        public List<FileStruct> GetDirectoryAndFiles(string dirName)
        {
            var fileList = new List<FileStruct>();
            response = Open(new Uri(ftpUri + dirName), WebRequestMethods.Ftp.ListDirectoryDetails);
            if (response == null) return fileList;

            try
            {
                using (var stream = response.GetResponseStream())
                {
                    using (var sr = new StreamReader(stream, Encoding.Default))
                    {
                        string line = null;
                        while ((line = sr.ReadLine()) != null)
                        {
                            //line的格式如下：serv-u(文件夹为第1位为d)
                            //drw-rw-rw-   1 user     group           0 Jun 10  2019 BStatus
                            //-rw-rw-rw-   1 user     group         625 Dec  7  2018 FTP文档.txt
                            string[] arr = line.Split(' ');
                            if (arr.Length < 12) continue;//remotePath不为空时，第1行返回值为：total 10715

                            var model = new FileStruct()
                            {
                                IsDirectory = line.Substring(0, 3) == "drw" ? true : false,
                                Name = arr[arr.Length - 1],
                                Path = dirName + "/" + arr[arr.Length - 1]
                            };

                            if (model.Name != "." && model.Name != "..")//排除.和..
                            {
                                fileList.Add(model);
                            }
                        }
                    }
                }
                return fileList;
            }
            catch (Exception e)
            {
                LoggerHelper.Error("获取【" + dirName + "】目录失败", e);
                return fileList;
            }
        }

        /// <summary>
        /// 获取目录的文件
        /// </summary>
        public List<FileStruct> GetFiles(string dirName)
        {
            var fileList = new List<FileStruct>();
            response = Open(new Uri(ftpUri + dirName), WebRequestMethods.Ftp.ListDirectory);
            if (response == null) return fileList;

            try
            {
                using (var stream = response.GetResponseStream())
                {
                    using (var sr = new StreamReader(stream, Encoding.Default))
                    {
                        string line = null;
                        while ((line = sr.ReadLine()) != null)
                        {
                            var model = new FileStruct()
                            {
                                Name = line,
                                Path = dirName + "/" + line
                            };
                            fileList.Add(model);
                        }
                    }
                }
                return fileList;
            }
            catch (Exception e)
            {
                LoggerHelper.Error("获取目录【" + dirName + "】的文件失败", e);
                return fileList;
            }
        }

        /// <summary>
        /// 获得远程文件大小
        /// </summary>
        public long GetFileSize(string fileName)
        {
            response = Open(new Uri(ftpUri + fileName), WebRequestMethods.Ftp.GetFileSize);
            return response == null ? -1 : response.ContentLength;
        }

        /// <summary>
        /// 文件是否存在
        /// </summary>
        public bool FileExist(string fileName)
        {
            long length = GetFileSize(fileName);
            return length == -1 ? false : true;
        }

        /// <summary>
        /// 目录是否存在
        /// </summary>
        public bool DirectoryExist(string dirName)
        {
            var list = GetDirectoryAndFiles(Path.GetDirectoryName(dirName));
            return list.Count(m => m.IsDirectory == true && m.Name == dirName) > 0 ? true : false;
        }

        /// <summary>
        /// 更改目录或文件名
        /// </summary>
        /// <param name="oldName">老名称</param>
        /// <param name="newName">新名称</param>
        public bool ReName(string oldName, string newName)
        {
            request = OpenRequest(new Uri(ftpUri + oldName), WebRequestMethods.Ftp.Rename);
            request.RenameTo = newName;
            try
            {
                response = (FtpWebResponse)request.GetResponse();
                return response == null ? false : true;
            }
            catch (Exception e)
            {
                LoggerHelper.Error("更改【" + oldName + "】的名称失败", e);
                return false;
            }
        }

        /// <summary>
        /// 获取文件最后修改时间
        /// </summary>
        /// <param name="fileName"></param>
        /// <returns></returns>
        public string GetLastModifyTime(string fileName, string format = "yyyy-MM-dd HH:mm:ss")
        {
            try
            {
                response = Open(new Uri(ftpUri + fileName), WebRequestMethods.Ftp.GetDateTimestamp);
                return response.LastModified.ToString(format);
            }
            catch (Exception e)
            {
                LoggerHelper.Error("获取【" + fileName + "】的最后修改时间失败", e);
                return string.Empty;
            }
        }

        public Stream GetStream(string remoteFileName)
        {
            if (!Directory.Exists("temp"))
                Directory.CreateDirectory("temp");
            response = Open(new Uri(ftpUri + remoteFileName), WebRequestMethods.Ftp.DownloadFile);
            if (response == null) return null;
            try
            {
                string tempPath = $"temp/{new DateTime().ToString()}";
                using (FileStream outputStream = new FileStream(tempPath, FileMode.Create))
                {
                    using (Stream ftpStream = response.GetResponseStream())
                    {
                        long length = response.ContentLength;
                        int bufferSize = 2048;
                        int readCount;
                        byte[] buffer = new byte[bufferSize];
                        readCount = ftpStream.Read(buffer, 0, bufferSize);
                        while (readCount > 0)
                        {
                            outputStream.Write(buffer, 0, readCount);
                            readCount = ftpStream.Read(buffer, 0, bufferSize);
                        }
                    }
                    return new FileStream(tempPath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
                }
            }
            catch (Exception e)
            {
                //LoggerHelper.Error("下载【" + remoteFileName + "】失败", e);
                return null;
            }
        }

        #endregion 方法
    }

    /// <summary>
    /// FTP文件类
    /// </summary>
    public class FileStruct
    {
        /// <summary>
        /// 是否为目录
        /// </summary>
        public bool IsDirectory { get; set; }

        /// <summary>
        /// 创建时间(FTP上无法获得时间)
        /// </summary>
        //public DateTime CreateTime { get; set; }
        /// <summary>
        /// 文件或目录名称
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 路径
        /// </summary>
        public string Path { get; set; }
    }
}