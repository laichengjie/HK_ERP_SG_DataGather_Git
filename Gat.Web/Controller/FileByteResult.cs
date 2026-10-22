using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;

namespace Gat.Web.Controller
{
    public class FileByteResult : FileStreamResult
    {
        public FileByteResult(byte[] buffer,string mediaType):base(new MemoryStream(buffer), mediaType) { }

        public FileByteResult(byte[] buffer,string mediaType,string fileName):base(new MemoryStream(buffer), mediaType, fileName) { }
    }
}