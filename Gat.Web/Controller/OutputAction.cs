using System;
using System.IO;
using System.Web;
using System.Web.Mvc;

namespace Gat.Web.Controller
{
    public class OutputAction : ActionResult
    {
        /// <summary>
        /// Content-Type
        /// </summary>
        public string ContentType
        {
            get;
            set;
        }

        /// <summary>
        /// 下载文件名
        /// </summary>
        public string DownloadFileName
        {
            get;
            set;
        }

        /// <summary>
        /// 需要供下载的文件完全路径
        /// </summary>
        public string FileName
        {
            get;
            set;
        }

        /// <summary>
        /// 需要供下载的内容
        /// </summary>
        public byte[] Content
        {
            get;
            set;
        }

        /// <summary>
        /// 需要输出的流
        /// </summary>
        public Stream StreamInst
        {
            get;
            set;
        }

        public OutputAction()
        {
        }

        public OutputAction(string oFileName)
        {
            FileName = oFileName;
        }

        public OutputAction(byte[] oContent)
        {
            Content = oContent;
        }

        public OutputAction(Stream oStream)
        {
            StreamInst = oStream;
        }

        public override void ExecuteResult(ControllerContext context)
        {
            if (context == null)
            {
                throw new ArgumentNullException("context");
            }

            HttpResponseBase response = context.HttpContext.Response;
            //response.Clear();

            if (!string.IsNullOrEmpty(FileName))
            {
                OutFile(response);

                if (string.IsNullOrEmpty(DownloadFileName))
                {
                    DownloadFileName = Path.GetFileName(FileName);
                }
            }
            else if (Content != null)
            {
                OutContent(response);
            }
            else if (StreamInst != null)
            {
                OutStream(response);
            }

            //response.Charset
            //response.AddHeader("Content-Length");
            response.ContentType = string.IsNullOrEmpty(ContentType) ? "application/octet-stream" : ContentType;

            if (!string.IsNullOrEmpty(DownloadFileName))
            {
                response.AddHeader("Content-Disposition", "attachment; filename=" + HttpUtility.UrlEncode(DownloadFileName));
            }

            response.End();
        }

        private void OutFile(HttpResponseBase response)
        {
            FileStream s = new FileStream(FileName, FileMode.Open);
            StreamInst = s;
            OutStream(response);
        }

        private void OutContent(HttpResponseBase response)
        {
            response.OutputStream.Write(Content, 0, Content.Length);
        }

        private void OutStream(HttpResponseBase response)
        {
            using (StreamInst)
            {
                byte[] buffer = new byte[8];

                while (true)
                {
                    int count = StreamInst.Read(buffer, 0, 8);

                    if (count == 0)
                    {
                        return;
                    }

                    response.OutputStream.Write(buffer, 0, count);
                }
            }
        }
    }
}