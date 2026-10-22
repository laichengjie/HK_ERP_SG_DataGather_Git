using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.IO;
using System.Web;

namespace Gat.Utils
{
    public class FileHelper
    {
        /// <summary>
        /// 下载文件
        /// </summary>
        /// <param name="fileName"></param>
        public static void DownloadFile(string fileName, bool deleteFile = false)
        {
            FileStream myFile = new FileStream(fileName, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
            BinaryReader br = new BinaryReader(myFile);

            var httpResponse = HttpContext.Current.Response;
            var httpRequest = HttpContext.Current.Request;
            var speed = 40960;

            httpResponse.AddHeader("Accept-Ranges", "bytes");
            httpResponse.Buffer = false;
            long fileLength = myFile.Length;
            long startBytes = 0;

            double pack = 10240; //10K bytes
            //int sleep = 200;   //每秒5次   即5*10K bytes每秒
            int sleep = (int)Math.Floor(1000 * pack / speed) + 1;
            if (httpRequest.Headers["Range"] != null)
            {
                httpResponse.StatusCode = 206;
                string[] range = httpRequest.Headers["Range"].Split(new char[] { '=', '-' });
                startBytes = Convert.ToInt64(range[1]);
            }
            httpResponse.AddHeader("Content-Length", (fileLength - startBytes).ToString());
            if (startBytes != 0)
            {
                //Response.AddHeader("Content-Range", string.Format(" bytes {0}-{1}/{2}", startBytes, fileLength-1, fileLength));
            }
            httpResponse.AddHeader("Connection", "Keep-Alive");
            httpResponse.ContentType = "application/octet-stream";
            httpResponse.AddHeader("Content-Disposition", "attachment;filename=" + HttpUtility.UrlEncode(Path.GetFileName(fileName), System.Text.Encoding.UTF8));

            br.BaseStream.Seek(startBytes, SeekOrigin.Begin);
            int maxCount = (int)Math.Floor((fileLength - startBytes) / pack) + 1;

            for (int i = 0; i < maxCount; i++)
            {
                if (httpResponse.IsClientConnected)
                {
                    httpResponse.BinaryWrite(br.ReadBytes(int.Parse(pack.ToString())));
                    //System.Threading.Thread.Sleep(sleep);
                }
                else
                {
                    i = maxCount;
                }
            }

            httpResponse.Flush();
            httpResponse.Close();

            br.Close();
            myFile.Close();
            if (deleteFile && File.Exists(fileName)) File.Delete(fileName);
            //if (!File.Exists(fileName)) return false;

            //try
            //{

            //    }
            //    catch (Exception ex)
            //    {
            //        //LogHelper _log = new LogHelper();
            //        //_log.SetConfig();
            //        //_log.Log(LogType.Application, ex);

            //        return false;
            //    }
            //    finally
            //    {
            //        br.Close();
            //        myFile.Close();


            //        if (deleteFile && File.Exists(fileName)) File.Delete(fileName);
            //    }
            //}
            //catch
            //{
            //    return false;
            //}
            //return true;
        }

        /// <summary>
        /// 上传文件
        /// </summary>
        /// <param name="file">通过form表达提交的文件</param>
        /// <param name="virpath">文件要保存的虚拟路径</param>
        public static string uploadFile(HttpPostedFile file, string virpath)
        {
            //if (file.ContentLength > 1024 * 1024 * 6)
            //{
            //    throw new Exception("文件不能大于6M");
            //}
            string imgtype = Path.GetExtension(file.FileName);
            //imgtype对上传的文件进行限制
            if (imgtype != ".xls" && imgtype != ".xlsx")
            {
                throw new Exception("只允许上传excel文件");
            }
            string dirFullPath = HttpContext.Current.Server.MapPath(virpath);
            if (!Directory.Exists(dirFullPath))//如果文件夹不存在，则先创建文件夹
            {
                Directory.CreateDirectory(dirFullPath);
            }
            string fileName = file.FileName;
            if (fileName.LastIndexOf('\\') > -1)
            {
                fileName = fileName.Substring(fileName.LastIndexOf('\\') + 1);
            }
            file.SaveAs(dirFullPath + fileName);
            return fileName;
        }
    }
}
