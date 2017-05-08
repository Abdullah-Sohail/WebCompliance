using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Web;
//using Dart.Ssh;

namespace Compliance.Audio.API.Code
{
    public class OpenPriSFTP
    {
        public static string GetTempFile(string fromLoc)
        {

            string server = fromLoc.Split('#')[0];
            string username = "root";
            string pass = "Vt1s@gdch0ic3";
            string newFile = Path.Combine(ConfigurationManager.AppSettings["ffmpeg:TempPath"], Guid.NewGuid().ToString() + ".ogg");

            fromLoc = fromLoc.Split('#')[1];

            //using (Sftp ftp = new Sftp())
            //{
            //    try
            //    {
            //        ftp.Connect(server);
            //        ftp.Authenticate(username, pass);
            //        ftp.Get(fromLoc, newFile, CopyMode.Copy);
            //        ftp.Close();
            //    }
            //    catch (Exception ex)
            //    {
            //        throw;
            //    }
            //}

            return newFile;
        }
    }
}