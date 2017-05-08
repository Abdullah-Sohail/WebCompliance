using System;
using System.Diagnostics;
using System.IO;

namespace Compiance.Audio.Host.AspNetCore.Code
{
	public class Converter
	{
		private string _ffExe;
		private readonly string _tempPath;

		public Converter(string ffmpegExePath, string tempPath)
		{
			_ffExe = ffmpegExePath;
			_tempPath = tempPath;
			if (string.IsNullOrWhiteSpace(ffmpegExePath) || !File.Exists(ffmpegExePath)) throw new Exception("Could not find the location of the ffmpeg exe file.  The path for ffmpeg.exe " +
			  "can be passed in via a constructor of the ffmpeg class (this class) or by setting in the app.config or web.config file.  " +
			  "in the appsettings section, the correct property name is: ffmpeg:ExeLocation");

			Initialize();
		}

		//Make sure we have valid ffMpeg.exe file and working directory to do our dirty work.
		private void Initialize()
		{
			//Now see if ffmpeg.exe exists
			string workingpath = GetWorkingFile();
			if (string.IsNullOrEmpty(workingpath))
			{
				//ffmpeg doesn't exist at the location stated.
				throw new Exception("Could not find a copy of ffmpeg.exe");
			}
			_ffExe = workingpath;

		}

		private string GetWorkingFile()
		{
			//try the stated directory
			if (File.Exists(_ffExe))
			{
				return _ffExe;
			}

			//oops, that didn't work, try the base directory
			if (File.Exists(Path.GetFileName(_ffExe)))
			{
				return Path.GetFileName(_ffExe);
			}

			//well, now we are really unlucky, let's just return null
			return null;
		}

		public static System.Drawing.Image LoadImageFromFile(string fileName)
		{
			System.Drawing.Image theImage = null;
			using (FileStream fileStream = new FileStream(fileName, FileMode.Open,
			FileAccess.Read))
			{
				byte[] img;
				img = new byte[fileStream.Length];
				fileStream.Read(img, 0, img.Length);
				fileStream.Close();
				theImage = System.Drawing.Image.FromStream(new MemoryStream(img));
				img = null;
			}
			GC.Collect();
			return theImage;
		}

		public static MemoryStream LoadMemoryStreamFromFile(string fileName)
		{
			MemoryStream ms = null;
			using (FileStream fileStream = new FileStream(fileName, FileMode.Open,
			FileAccess.Read))
			{
				byte[] fil;
				fil = new byte[fileStream.Length];
				fileStream.Read(fil, 0, fil.Length);
				fileStream.Close();
				ms = new MemoryStream(fil);
			}
			GC.Collect();
			return ms;
		}


		//Note the private call here and the argument for Parameters.  The private call is
		//being made here because, in this class, we don't really want to have this method
		//called from outside of the class -- this, however flies in the face of allowing the
		//parameters argument (why not just allow out the public call so that a developer can
		//put in the parameters from their own code?  I guess one could do it and it would probably
		//work fine but, for this implementation, I chose to leave it private.
		private string RunProcess(string Parameters)
		{
			//create a process info object so we can run our app
			ProcessStartInfo oInfo = new ProcessStartInfo(this._ffExe, Parameters);

			//Set identity
			//var ssPwd = new System.Security.SecureString();
			//var password = "ChangeMe22";

			//for (int x = 0; x < password.Length; x++)
			//{
			//    ssPwd.AppendChar(password[x]);
			//}

			//oInfo.Domain = "ccscollect.com";
			//oInfo.UserName = "svc_asp";
			//oInfo.Password = ssPwd;

			oInfo.UseShellExecute = false;
			oInfo.CreateNoWindow = true;

			//so we are going to redirect the output and error so that we can parse the return
			oInfo.RedirectStandardOutput = true;
			oInfo.RedirectStandardError = true;


			//Create the output and streamreader to get the output
			string output = null; StreamReader srOutput = null;

			//try the process
			try
			{
				//run the process
				Process proc = System.Diagnostics.Process.Start(oInfo);

				proc.WaitForExit();

				//get the output
				srOutput = proc.StandardError;

				//now put it in a string
				output = srOutput.ReadToEnd();

				proc.Close();
			}
			catch (Exception)
			{
				output = string.Empty;
			}
			finally
			{
				//now, if we succeded, close out the streamreader
				if (srOutput != null)
				{
					srOutput.Close();
					srOutput.Dispose();
				}
			}
			return output;
		}

		public MemoryStream Convert(string inFile, bool deleteInput)
		{
			string tmpfile = Path.Combine(_tempPath, System.Guid.NewGuid().ToString() + ".mp3");
			var Params = string.Format("-i \"{0}\" -ac 2 \"{1}\"", inFile, tmpfile);
			var output = RunProcess(Params);
			MemoryStream ret = null;

			if (File.Exists(tmpfile))
			{
				ret = LoadMemoryStreamFromFile(tmpfile);
				try
				{
					File.Delete(tmpfile);
					if (deleteInput)
						File.Delete(inFile);
				}
				catch (Exception) { }
			}
			return ret;
		}
	}

	#region "This is some cool shit"

	//    //We are going to take in memory stream for this file to allow for different input options.
	//    //Unfortunately, this means that we also need the file extension to pass it out to ffMpeg.
	//    public VideoFile GetVideoInfo(MemoryStream inputFile, string Filename)
	//    {
	//        //Create a temporary file for our use in ffMpeg
	//        string tempfile = Path.Combine(this.WorkingPath, System.Guid.NewGuid().ToString() + Path.GetExtension(Filename));
	//        FileStream fs = File.Create(tempfile);

	//        //write the memory stream to a file and close our the stream so it can be used again.
	//        inputFile.WriteTo(fs);
	//        fs.Flush();
	//        fs.Close();
	//        GC.Collect();

	//        //Video File is a class you will see further down this post.  It has some basic information about the video
	//        VideoFile vf = null;
	//        try
	//        {
	//            vf = new VideoFile(tempfile);
	//        }
	//        catch (Exception ex)
	//        {
	//            throw ex;
	//        }

	//        //And, without adieu, a call to our main method for this functionality.
	//        GetVideoInfo(vf);

	//        try
	//        {
	//            File.Delete(tempfile);
	//        }
	//        catch (Exception)
	//        {

	//        }

	//        return vf;
	//    }

	//    //This sub is just another overload to allow input of a direct path, we are just
	//    //using the videofile class to send in.  More on the video file class further down
	//    //the article.
	//    public VideoFile GetVideoInfo(string inputPath)
	//    {
	//        VideoFile vf = null;
	//        try
	//        {
	//            vf = new VideoFile(inputPath);
	//        }
	//        catch (Exception ex)
	//        {
	//            throw ex;
	//        }
	//        GetVideoInfo(vf);
	//        return vf;
	//    }

	//    //And now the important code for the GetVideoInfo
	//    public void GetVideoInfo(VideoFile input)
	//    {
	//        //set up the parameters for video info -- these will be passed into ffMpeg.exe
	//        string Params = string.Format("-i {0}", input.Path);
	//        string output = RunProcess(Params);
	//        input.RawInfo = output;

	//        //Use a regular expression to get the different properties from the video parsed out.
	//        Regex re = new Regex("[D|d]uration:.((\\d|:|\\.)*)");
	//        Match m = re.Match(input.RawInfo);

	//        if (m.Success)
	//        {
	//            string duration = m.Groups[1].Value;
	//            string[] timepieces = duration.Split(new char[] { ':', '.' });
	//            if (timepieces.Length == 4)
	//            {
	//                input.Duration = new TimeSpan(0, Convert.ToInt16(timepieces[0]), Convert.ToInt16(timepieces[1]), Convert.ToInt16(timepieces[2]), Convert.ToInt16(timepieces[3]));
	//            }
	//        }

	//        //get audio bit rate
	//        re = new Regex("[B|b]itrate:.((\\d|:)*)");
	//        m = re.Match(input.RawInfo);
	//        double kb = 0.0;
	//        if (m.Success)
	//        {
	//            Double.TryParse(m.Groups[1].Value, out kb);
	//        }
	//        input.BitRate = kb;

	//        //get the audio format
	//        re = new Regex("[A|a]udio:.*");
	//        m = re.Match(input.RawInfo);
	//        if (m.Success)
	//        {
	//            input.AudioFormat = m.Value;
	//        }

	//        //get the video format
	//        re = new Regex("[V|v]ideo:.*");
	//        m = re.Match(input.RawInfo);
	//        if (m.Success)
	//        {
	//            input.VideoFormat = m.Value;
	//        }

	//        //get the video format
	//        re = new Regex("(\\d{2,3})x(\\d{2,3})");
	//        m = re.Match(input.RawInfo);
	//        if (m.Success)
	//        {
	//            int width = 0; int height = 0;
	//            int.TryParse(m.Groups[1].Value, out width);
	//            int.TryParse(m.Groups[2].Value, out height);
	//            input.Width = width;
	//            input.Height = height;
	//        }
	//        input.infoGathered = true;
	//    }

	//    //Note the ouputpackage object output.  The output package class is detailed below.
	//    //this overload does much the same as the one above does, in that, it offers the ability
	//    //to input a memory stream vs. a filename or our videofile input class.
	//    public OutputPackage ConvertToFLV(MemoryStream inputFile, string Filename)
	//    {
	//        string tempfile = Path.Combine(this.WorkingPath, System.Guid.NewGuid().ToString() + Path.GetExtension(Filename));
	//        FileStream fs = File.Create(tempfile);
	//        inputFile.WriteTo(fs);
	//        fs.Flush();
	//        fs.Close();
	//        GC.Collect();

	//        VideoFile vf = null;
	//        try
	//        {
	//            vf = new VideoFile(tempfile);
	//        }
	//        catch (Exception ex)
	//        {
	//            throw ex;
	//        }

	//        OutputPackage oo = ConvertToFLV(vf);

	//        try
	//        {
	//            File.Delete(tempfile);
	//        }
	//        catch (Exception)
	//        {

	//        }

	//        return oo;
	//    }
	//    public OutputPackage ConvertToFLV(string inputPath)
	//    {
	//        VideoFile vf = null;
	//        try
	//        {
	//            vf = new VideoFile(inputPath);
	//        }
	//        catch (Exception ex)
	//        {
	//            throw ex;
	//        }

	//        OutputPackage oo = ConvertToFLV(vf);
	//        return oo;
	//    }

	//    //The actually important code, rather than an overload.
	//    public OutputPackage ConvertToFLV(VideoFile input)
	//    {
	//        //check to see if we have already gathered our info so we know
	//        //where to get our preview image from.
	//        if (!input.infoGathered)
	//        {
	//            GetVideoInfo(input);
	//        }

	//        //Create our output object
	//        OutputPackage ou = new OutputPackage();

	//        //set up the parameters for getting a previewimage
	//        string filename = System.Guid.NewGuid().ToString() + ".jpg";
	//        int secs;

	//        //divide the duration in 3 to get a preview image in the middle of the clip
	//        //instead of a black image from the beginning.
	//        secs = (int)Math.Round(TimeSpan.FromTicks(input.Duration.Ticks / 3).TotalSeconds, 0);

	//        string finalpath = Path.Combine(this.WorkingPath, filename);

	//        //These are the parameters for setting up a preview image that must be passed to ffmpeg.
	//        //Note that we are asking for a jpeg image at our specified seconds.
	//        string Params = string.Format("-i {0} {1} -vcodec mjpeg -ss {2} -vframes 1 -an -f rawvideo", input.Path, finalpath, secs);
	//        string output = RunProcess(Params);

	//        ou.RawOutput = output;

	//        //Ok, so hopefully we now have a preview file.  If the file
	//        //did not get created properly, try again at the first frame.
	//        if (File.Exists(finalpath))
	//        {
	//            //load that file into our output package and attempt to delete the file
	//            //since we no longer need it.
	//            ou.PreviewImage = LoadImageFromFile(finalpath);
	//            try
	//            {
	//                File.Delete(finalpath);
	//            }
	//            catch (Exception) { }
	//        }
	//        else
	//        { //try running again at frame 1 to get something
	//            Params = string.Format("-i {0} {1} -vcodec mjpeg -ss {2} -vframes 1 -an -f rawvideo", input.Path, finalpath, 1);
	//            output = RunProcess(Params);

	//            ou.RawOutput = output;

	//            if (File.Exists(finalpath))
	//            {
	//                ou.PreviewImage = LoadImageFromFile(finalpath);
	//                try
	//                {
	//                    File.Delete(finalpath);
	//                }
	//                catch (Exception) { }
	//            }
	//        }

	//        finalpath = Path.Combine(this.WorkingPath, filename);
	//        filename = System.Guid.NewGuid().ToString() + ".flv";

	//        //Now we are going to actually create the converted file.  Note that we are asking for
	//        //a video at 22khz 64bit.  This can be changed by a couple quick alterations to this line,
	//        //or by extending out this class to offer multiple different conversions.
	//        Params = string.Format("-i {0} -y -ar 22050 -ab 64 -f flv {1}", input.Path, finalpath);
	//        output = RunProcess(Params);

	//        //Check to see if our conversion file exists and then load the converted
	//        //file into our output package.  If the file does exist and we are able to
	//        //load it into our output package, we can delete the work file.
	//        if (File.Exists(finalpath))
	//        {
	//            ou.VideoStream = LoadMemoryStreamFromFile(finalpath);
	//            try
	//            {
	//                File.Delete(finalpath);
	//            }
	//            catch (Exception) { }
	//        }
	//        return ou;
	//    }



	//}

	//public class VideoFile
	//{
	//    #region Properties
	//    private string _Path;
	//    public string Path
	//    {
	//        get
	//        {
	//            return _Path;
	//        }
	//        set
	//        {
	//            _Path = value;
	//        }
	//    }

	//    public TimeSpan Duration { get; set; }
	//    public double BitRate { get; set; }
	//    public string AudioFormat { get; set; }
	//    public string VideoFormat { get; set; }
	//    public int Height { get; set; }
	//    public int Width { get; set; }
	//    public string RawInfo { get; set; }
	//    public bool infoGathered { get; set; }
	//    #endregion

	//    #region Constructors
	//    public VideoFile(string path)
	//    {
	//        _Path = path;
	//        Initialize();
	//    }
	//    #endregion

	//    #region Initialization
	//    private void Initialize()
	//    {
	//        this.infoGathered = false;
	//        //first make sure we have a value for the video file setting
	//        if (string.IsNullOrEmpty(_Path))
	//        {
	//            throw new Exception("Could not find the location of the video file");
	//        }

	//        //Now see if the video file exists
	//        if (!File.Exists(_Path))
	//        {
	//            throw new Exception("The video file " + _Path + " does not exist.");
	//        }
	//    }
	//    #endregion
	//}

	//public class OutputPackage
	//{
	//    public MemoryStream VideoStream { get; set; }
	//    public System.Drawing.Image PreviewImage { get; set; }
	//    public string RawOutput { get; set; }
	//    public bool Success { get; set; }
	//}
	#endregion
}