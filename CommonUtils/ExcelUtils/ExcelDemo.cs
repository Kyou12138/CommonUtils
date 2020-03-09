using CommonUtils.ExcelUtils.Helper;
using CommonUtils.ExcelUtils.Model;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Web;

namespace CommonUtils.ExcelUtils
{
    public class ExcelDemo
    {
        //导出ExcelDemo
        public HttpResponseMessage ExportExcel()
        {
            //模拟数据源
            List<DemoDto> demoDtos = new List<DemoDto>()
            {
                new DemoDto { UserName = "test0", Gender = "男" },
                new DemoDto { UserName = "test1", Gender = "女" },
                new DemoDto { UserName = "test2", Gender = "男" }
            };

            var model = new ExportModel();
            model.Data = JsonConvert.SerializeObject(demoDtos);
            //设置文件名
            model.FileName = "数据";
            model.Title = model.FileName;
            //设置表头
            model.LstCol = new List<ExportDataColumn>();
            model.LstCol.Add(new ExportDataColumn() { prop = "UserName", label = "用户名" });
            model.LstCol.Add(new ExportDataColumn() { prop = "Gender", label = "性别" });

            //数据转为DataTable格式
            DataTable dt = JsonConvert.DeserializeObject<DataTable>(model.Data);
            string fileName = model.FileName + DateTime.Now.ToString("yyMMddHHmmssfff") + ".xls";

            //设置导出格式
            ExcelConfig config = new ExcelConfig();
            //config.Title = model.Title;
            //config.TitleFont = "微软雅黑";
            //config.TitlePoint = 25;
            config.FileName = fileName;
            config.IsAllSizeColumn = true;//自适应最长列宽
            //每一列的设置,没有设置的列信息，系统将按datatable中的列名导出
            config.ColumnEntity = new List<ColumnEntity>();
            //表头
            foreach (var col in model.LstCol)
            {
                config.ColumnEntity.Add(new ColumnEntity() { Column = col.prop, ExcelColumn = col.label });
            }

            //调用导出方法
            var stream = ExcelHelper.ExportMemoryStream(dt, config); // 通过NPOI形成将数据绘制成Excel文件并形成内存流

            //响应设置
            string browser = string.Empty;
            if (HttpContext.Current.Request.UserAgent != null)
            {
                browser = HttpContext.Current.Request.UserAgent.ToUpper();
            }
            HttpResponseMessage httpResponseMessage = new HttpResponseMessage(HttpStatusCode.OK);
            httpResponseMessage.Content = new StreamContent(stream);
            httpResponseMessage.Content.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream"); // 返回类型必须为文件流 application/octet-stream
            httpResponseMessage.Content.Headers.ContentDisposition = new ContentDispositionHeaderValue("attachment") // 设置头部其他内容特性, 文件名
            {
                FileName = browser.Contains("FIREFOX") ? fileName : HttpUtility.UrlEncode(fileName)
            };
            return httpResponseMessage;
            //return ResponseMessage(httpResponseMessage); //(IHttpActionResult)
        }
    }

    internal class DemoDto
    {
        /// <summary>
        /// 用户名
        /// </summary>
        public string UserName { get; set; }

        /// <summary>
        /// 性别
        /// </summary>
        public string Gender { get; set; }
    }
}