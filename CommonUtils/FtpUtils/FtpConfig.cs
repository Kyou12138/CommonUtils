namespace CommonUtils.FtpUtils
{
    public class FtpConfig
    {
        /// <summary>
        /// FTP服务器IP地址
        /// </summary>
        public string FtpServerIp { get; set; }

        /// <summary>
        /// FTP服务端口
        /// </summary>
        public int FtpServerPort { get; set; } = 21;

        /// <summary>
        /// FTP用户名
        /// </summary>
        public string FtpUserName { get; set; }

        /// <summary>
        /// FTP用户密码
        /// </summary>
        public string FtpPassword { get; set; }

        /// <summary>
        /// 超时时间（毫秒）
        /// </summary>
        public int Timeout { get; set; } = 30000;

        /// <summary>
        /// 是否以二进制数据传输，否则为text
        /// </summary>
        public bool UseBinary { get; set; } = true;

        /// <summary>
        /// 当请求完成后是否保持连接
        /// </summary>
        public bool KeepAlive { get; set; } = false;

        /// <summary>
        /// 是否启用SSL
        /// </summary>
        public bool EnableSsl { get; set; } = false;

        /// <summary>
        /// 是否启用被动模式
        /// </summary>
        public bool UsePassive { get; set; } = true;
    }
}