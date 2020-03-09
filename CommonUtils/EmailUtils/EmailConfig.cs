namespace CommonUtils.EmailUtils
{
    public class EmailConfig
    {
        /// <summary>
        /// 发送人邮箱用户名
        /// </summary>
        public string User { get; set; }

        /// <summary>
        /// 发送人邮箱密码（授权码）
        /// </summary>
        public string Password { get; set; }

        /// <summary>
        /// 发送人邮箱地址
        /// </summary>
        public string UserEmailAddress { get; set; }

        /// <summary>
        /// 邮箱服务器地址,e.g. smtphz.qiye.163.com
        /// </summary>
        public string Host { get; set; }

        /// <summary>
        /// 是否开启SSL
        /// </summary>
        public bool EnableSsl { get; set; }

        /// <summary>
        /// 收件人邮箱
        /// </summary>
        public string ReceiveEmailAddress { get; set; }
    }
}