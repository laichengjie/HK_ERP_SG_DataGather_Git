using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using ICSharpCode.SharpZipLib.Zip;


namespace GatNewBll.Common
{
    /// <summary>
    /// 压缩文件类
    /// </summary>
    public class CompressFile
    {
        /// <summary>
        /// 
        /// </summary>
        public CompressFile()
        {
            ZIPFileList = new List<string>();
            ZIPName = string.Empty;
            ZIPDirectoryName = string.Empty;
        }

        /// <summary>
        /// 压缩的文件路径
        /// </summary>
        public string ZIPDirectoryName { get; set; }

        /// <summary>
        /// 压缩后的文件名称
        /// </summary>
        public string ZIPName { get; set; }

        /// <summary>
        ///需要压缩的文件 
        /// </summary>
        public List<string> ZIPFileList { get; set; }

        /// <summary>
        /// 压缩文件返回压缩后的信息
        /// </summary>
        /// <param name="IsDelCompressFile">是否删除压缩的源文件</param>
        /// <returns></returns>
        public string CompressReturnMsg(bool IsDelCompressFile)
        {
            FileStream Zip_File;
            ZipOutputStream ZipStream;
            ZipEntry ZipEntry;
            string rtnMessage = "";//返回的信息

            try
            {
                //循环文件，如果文件不存在就不添加的压缩里面
                for (int i = 0; i < ZIPFileList.Count; i++)
                {
                    if (!File.Exists(ZIPFileList[i]))
                    {
                        ZIPFileList.RemoveAt(i);
                        i--;
                    }
                }

                //没有有文件下面的压缩不执行
                if (ZIPFileList.Count == 0)
                {
                    return " file not find";
                }
                //没有目录进行创建
                if (!Directory.Exists(ZIPDirectoryName))
                {
                    Directory.CreateDirectory(ZIPDirectoryName);
                }

                // 解决文档名称乱码问题,出现乱码就是因为CodePage不对
                Encoding gbk = Encoding.GetEncoding("gbk");
                ICSharpCode.SharpZipLib.Zip.ZipConstants.DefaultCodePage = gbk.CodePage;

                //文件路径，文档路径与文件名称
                string strPath = ZIPDirectoryName + ZIPName;

                Zip_File = File.Create(strPath);
                ZipStream = new ZipOutputStream(Zip_File);
                foreach (string FileToZip in ZIPFileList)
                {
                    Zip_File = File.OpenRead(FileToZip);
                    byte[] buffer = new byte[Zip_File.Length];
                    Zip_File.Read(buffer, 0, buffer.Length);
                    Zip_File.Close();
                    ZipEntry = new ZipEntry(Path.GetFileName(FileToZip));
                    ZipStream.PutNextEntry(ZipEntry);
                    ZipStream.Write(buffer, 0, buffer.Length);
                }
                ZipStream.Finish();
                ZipStream.Close();
                Zip_File.Close();
                if (IsDelCompressFile)
                {
                    for (int i = 0; i < ZIPFileList.Count; i++)
                    {
                        if (File.Exists(ZIPFileList[i]))
                            File.Delete(ZIPFileList[i]);
                    }
                }
                rtnMessage = "success";

            }
            catch (Exception ex)
            {
                rtnMessage = "fail:" + ex.Message;
            }
            finally
            {
                GC.Collect();
                GC.Collect(1);
            }
            return rtnMessage;
        }
    }
}
