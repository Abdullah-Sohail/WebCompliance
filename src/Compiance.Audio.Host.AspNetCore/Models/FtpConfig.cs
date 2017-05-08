namespace Compiance.Audio.Host.AspNetCore.Models
{
	public  class FtpConfiguration
	{
		public string TempPath { get; set; }
		public string UserName { get; set; }
		public string Password { get; set; }
	}

	public class FfmpegConfiguration
	{
		public string FfmpegExePath { get; set; }
	}
}