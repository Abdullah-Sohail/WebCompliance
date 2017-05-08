using System;
using System.IO;
using Compiance.Audio.Host.AspNetCore.Models;
using Microsoft.Extensions.Options;

//using Dart.Ssh;

namespace Compiance.Audio.Host.AspNetCore.Code
{
    public class OpenPriSFTP
    {
	    private readonly IOptions<FtpConfiguration> _ftpConfig;

	    public OpenPriSFTP(IOptions<FtpConfiguration> ftpConfig)
	    {
		    _ftpConfig = ftpConfig;
	    }
        public string GetTempFile(string fromLoc)
        {

            string server = fromLoc.Split('#')[0];
	        string username = _ftpConfig.Value.UserName;// "root";
	        string pass = _ftpConfig.Value.Password;  //"Vt1s@gdch0ic3";
			string newFile = Path.Combine(_ftpConfig.Value.TempPath, Guid.NewGuid().ToString() + ".ogg");

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