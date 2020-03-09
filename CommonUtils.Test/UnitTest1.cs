using System;
using CommonUtils.EmailUtils;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace CommonUtils.Test
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void Demo()
        {
            EmailConfig config = new EmailConfig();
        }

        [TestMethod]
        public void Demo1()
        {
            LoggerHelper.Monitor("test");

        }

        public void Demo2()
        {

        }

    }
}
