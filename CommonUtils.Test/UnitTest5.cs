using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace CommonUtils.Test
{
    [TestClass]
    public class UnitTest5
    {
        private Dictionary<int, string> productPoint = new Dictionary<int, string>();

        private string ConfigFile = "Nidec.csv";

        private Dictionary<string, string> svr = new Dictionary<string, string>();

        private string serverName = "";

        private int SaveDataFrequency;

        private ObservableCollection<Dictionary<string, string>> newRecords;

        [TestMethod]
        public void demo1()
        {
            int row = 0;
            int col = 0;
            try
            {
                ParseConfigFlie();
                using (FileStream fs = new FileStream(@"D:\RESULT_LOG_182702-51.csv", FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
                {
                    using (StreamReader sr = new StreamReader(fs, Encoding.Default))
                    {
                        while (!sr.EndOfStream)
                        {
                            var rd = sr.ReadLine();
                            List<string> fields = NidecRecordSplit(rd, ',');
                            if (row != 0 && fields[0].StartsWith("SheetX")) int.TryParse(fields[1], out row);
                            if (col != 0 && fields[0].StartsWith("SheetY")) int.TryParse(fields[1], out col);
                            if (fields[0].StartsWith(string.Format($"{DateTime.Now.Year.ToString()}")))
                            {
                                Dictionary<string, string> record = new Dictionary<string, string>();
                                //                           File.AppendAllText(Environment.CurrentDirectory + "lee.log", string.Format($"文件的第{LineOfFile}行，有{fields.Length}列！\n"));
                                for (int i = 1; i < fields.Count; i++)
                                {
                                    string value;
                                    if (i == 1)
                                    {
                                        value = fields[i - 1] + " " + fields[i];
                                    }
                                    else
                                        value = fields[i];
                                    record[productPoint[i]] = value;

                                }
                                ////判断是否为今天的记录
                                //if (DateTime.Parse(record["DateTime"]).Date.Equals(DateTime.Now.Date))
                                //{
                                //    //ProductInfos.Add(record);
                                //    //DataPostToMes.Enqueue(record);
                                //    if (newRecords == null)
                                //    {
                                //        newRecords = new ObservableCollection<Dictionary<string, string>>();
                                //    }
                                //    newRecords.Add(record);
                                //}
                                if (newRecords == null)
                                {
                                    newRecords = new ObservableCollection<Dictionary<string, string>>();
                                }
                                if (record.Count != 26) Console.WriteLine($"count: {record.Count}");
                                newRecords.Add(record);
                            }
                        }
                    }
                }
                //创建map
                if(newRecords != null)
                {
                    var panelInfos = (from pi in newRecords
                                      let dataStatus = pi["DataStatus"]
                                      let serial = pi["Serial"]
                                      let barcode = pi["2D_ID"]
                                      let item = pi["Item"]
                                      let result = pi["Result"]
                                      let pJudge = pi["PJudge"]
                                      let pieceX = pi["PieceX"]
                                      let pieceY = pi["PieceY"]
                                      let error = pi["Error"]
                                      group new { DataStatus = dataStatus, Result = result, PJudge = pJudge, Serial = serial, Barcode = barcode, Item = item, PieceX = pieceX, PieceY = pieceY, Error = error } by serial into g
                                      select g).ToArray();
                    foreach (var panelInfo in panelInfos)
                    {
                        string[,] mapArr = new string[row, col];
                        string mapName = string.Empty;
                        foreach (var info in panelInfo)
                        {
                            string errorStr = !info.DataStatus.Contains("Test done") ?
                                info.DataStatus : string.IsNullOrWhiteSpace(info.Item) ?
                                string.Empty : $"{info.Item}_{info.PJudge}_{info.Result}_{info.Error}";
                            mapArr[int.Parse(info.PieceX) - 1, int.Parse(info.PieceY) - 1] = NidecHelper.GetNidecBinCode(errorStr);
                            //if (string.IsNullOrEmpty(mapName)) mapName = string.IsNullOrWhiteSpace(info.Barcode) ? $"nidec_{name}_{info.Serial}" : $"nidec_{name}_{info.Barcode}_{info.Serial}";
                            if (string.IsNullOrEmpty(mapName)) mapName = string.IsNullOrWhiteSpace(info.Barcode) ? $"nidec_{name}_{info.Serial}" : $"nidec_{name}_{info.Barcode}_{info.Serial}";
                        }
                        new NidecMap(mapArr, mapName);
                    }
                }

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
            
        }

        public void ParseConfigFlie()
        {
            try
            {
                using (var reader = new StreamReader($"Config/{ConfigFile}", Encoding.Default))
                {
                    while (!reader.EndOfStream)
                    {
                        var cfgs = reader.ReadLine().Split(',');
                        if (cfgs[0].Equals("server"))
                        {
                            for (int i = 1; i < cfgs.Length; i++)
                            {
                                var l = cfgs[i].Split(':');
                                svr[l[0]] = l[1];
                            }
                            serverName = svr["EquipmentName"];
                        }
                        else if (cfgs[0].Equals("SaveDataFrequency"))
                        {
                            SaveDataFrequency = int.Parse(cfgs[1]);
                        }
                        else if (cfgs[0].Equals("Point"))
                        {
                            productPoint[int.Parse(cfgs[1])] = cfgs[2];
                        }

                    }

                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
        }

        /// <summary>
        /// 分割字符串，忽略引号内的分割符
        /// </summary>
        /// <param name="str"></param>
        /// <param name="delimiter"></param>
        /// <returns></returns>
        public static List<string> NidecRecordSplit(string str, char delimiter)
        {
            List<string> temp = new List<string>();
            ArrayList chartemp = new ArrayList();
            Boolean abort = false;
            foreach (char i in str)
            {
                if (i.Equals('"'))
                {
                    abort = !abort;
                }
                if (!i.Equals('"') && !i.Equals(delimiter))
                {
                    chartemp.Add(i);
                }
                if (!i.Equals('"') && i.Equals(delimiter) && abort == true)
                {
                    chartemp.Add(i);
                }
                if (i.Equals(delimiter) && abort == false)
                {
                    string str5 = string.Join(null, (char[])chartemp.ToArray(typeof(char))); //其实这句是核心
                    temp.Add(str5.Trim());
                    chartemp.Clear();
                }
            }
            string str6 = string.Join(null, (char[])chartemp.ToArray(typeof(char)));
            temp.Add(str6.Trim());
            chartemp.Clear();
            return temp;
        }
    }
}
