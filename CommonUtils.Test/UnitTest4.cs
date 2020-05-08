using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using CommonUtils.FtpUtils;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace CommonUtils.Test
{
    [TestClass]
    public class UnitTest4
    {
        [TestMethod]
        public void TestMethod1()
        {
            string path = "ftp://zkzhang@newpiserver/Web.config";
            FileStream fs = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
            using (StreamReader sr = new StreamReader(fs))
            {
                while (!sr.EndOfStream)
                {
                    Console.WriteLine(sr.ReadLine());
                }
            }
        }

        /// <summary>
        /// 通配符转正则 处理 ? *
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        private static String WildCardToRegex(string rex)
        {
            return "^" + Regex.Escape(rex).Replace("\\?", ".").Replace("\\*", ".*") + "$";
        }

        [TestMethod]
        public void demo1()
        {
            var list = new List<string>()
            {
                @"\data_dll\WangDai.Data.xml",
                @"\data_dll\Attachment.Data.dll",
                @"\data_dll\Attachment.Data.dll.bak",
                @"\data_dll\tAttachment.Data.dll.bak"
            };

            // ^\\data_dll\\.*\.bak$
            var rex = WildCardToRegex(@"\data_dll\t*.bak");
            Console.WriteLine(rex);
            var temp = list.Where(ex => Regex.IsMatch(ex, rex, RegexOptions.IgnoreCase)).ToList();

            foreach (var item in temp)
            {
                Console.WriteLine(item);
            }

        }

        [TestMethod]
        public void demo2()
        {
            FtpHelper ftpHelper = new FtpHelper();
            List<FileStruct> folders = ftpHelper.GetFiles(@"\SAAConfig\B0110-1");
            Console.WriteLine(folders[0].Path);
            Console.WriteLine(folders.Count);
            string date = ftpHelper.GetLastModifyTime(folders[0].Path);
            Console.WriteLine(date);
            Stream s = ftpHelper.GetStream(folders[0].Path);
            using (StreamReader srd = new StreamReader(s, Encoding.Default))
            {
                while (!srd.EndOfStream)
                {
                    Console.WriteLine(srd.ReadLine());
                }
            }
        }

        [TestMethod]
        public void demo3()
        {
            string rd = " 2020/04/16, 07:59:51, 182702-51,3,  , 050317A_2W_4W, P2-050317A-2W+4W, Test done,0,2,4,1,50,2,1, SILVER,1495,1518,277,\" - F1495, S1496+F1518,S1519\", MS_ER, [contact fail(+F)],1.00E+100, ohm,0.00E+00,2.29E-02, [contact fail(+F)]";
            //string[] a = Regex.Split(rd, ",(?=([^\\\"]*\\\"[^\\\"]*\\\")*[^\\\"]*$)", RegexOptions.None);
            //foreach (var item in a)
            //{
            //    Console.WriteLine(item);
            //}
            ArrayList arr = Split(rd, ',');
            Console.WriteLine(arr.Count);
            foreach (var item in arr)
            {
                Console.WriteLine(item);
            }
        }

        /// <summary>
        /// 分割字符串，忽略引号内的分割符
        /// </summary>
        /// <param name="str"></param>
        /// <param name="delimiter"></param>
        /// <returns></returns>
        public static ArrayList Split(string str, char delimiter)
        {
            ArrayList temp = new ArrayList();
            ArrayList chartemp = new ArrayList();
            Boolean abort = false;
            foreach (char i in str)
            {
                if (i.Equals('"'))
                {
                    abort = !abort;
                }
                if (!i.Equals('"') && !i.Equals(delimiter)) {
                    chartemp.Add(i);
                }
                if (!i.Equals('"') && i.Equals(delimiter) && abort == true)
                { 
                    chartemp.Add(i);
                }
                if (i.Equals(delimiter) && abort == false) {
                    string str5 = string.Join(null, (char[])chartemp.ToArray(typeof(char))); //其实这句是核心
                    temp.Add(str5);
                    chartemp.Clear();
                }
            }
            string str6 = string.Join(null, (char[])chartemp.ToArray(typeof(char)));
            temp.Add(str6);
            chartemp.Clear();
            return temp;
        }
    }
}
