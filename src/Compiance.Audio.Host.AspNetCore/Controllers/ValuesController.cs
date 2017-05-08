using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Mime;
using System.Threading.Tasks;
using Compiance.Audio.Host.AspNetCore.Code;
using Microsoft.AspNetCore.Mvc;

namespace Compiance.Audio.Host.AspNetCore.Controllers
{
	public class ValuesController : Controller
	{
		private readonly OpenPriSFTP _openPriSftp;
		private readonly Converter _converter;

		public ValuesController(OpenPriSFTP openPriSftp, Converter converter)
		{
			_openPriSftp = openPriSftp;
			_converter = converter;
		}

		// GET api/values
		public IEnumerable<string> Get()
		{
			return new string[] { "value1", "value2" };
		}

		// GET api/values/5
		public string Get(string id)
		{
			return id;
		}

		// POST api/values
		public IActionResult Post([FromBody]string value)
		{
			var path = value;
			bool deleteInput = false;
			bool needConversion = true;

			if (path.Contains('#'))
			{
				path = _openPriSftp.GetTempFile(value);
				deleteInput = true;
			}

			if (path.StartsWith("\\\\"))
			{
				if (Path.GetExtension(path).ToLower() == ".mp3")
				{
					needConversion = false;
				}
				else
				{
					var newname = "C:\\Temp\\" + Guid.NewGuid().ToString() + Path.GetExtension(path);
					System.IO.File.Copy(path, newname);
					deleteInput = true;
					path = newname;
				}
			}


			MemoryStream stream;

			HttpResponseMessage result = new HttpResponseMessage(HttpStatusCode.OK);

			try
			{
				if (needConversion)
					stream = _converter.Convert(path, deleteInput);
				else
					stream = new MemoryStream(System.IO.File.ReadAllBytes(path));

				return new StreamResult(stream, "application/octet-stream");
			}
			catch (Exception ex)
			{
				return BadRequest(ex.Message);
			}


		}

		// PUT api/values/5
		public void Put(int id, [FromBody]string value)
		{
		}

		// DELETE api/values/5
		public void Delete(int id)
		{
		}
	}

	public class StreamResult : ActionResult
	{
		private readonly Stream _stream;
		private readonly string _contentType;

		public StreamResult(Stream stream, string contentType)
		{
			_stream = stream;
			_contentType = contentType;
		}


		public override async Task ExecuteResultAsync(ActionContext context)
		{
			var response = context.HttpContext.Response;
			response.ContentType = _contentType;
			_stream.Seek(0, SeekOrigin.Begin);
			await _stream.CopyToAsync(context.HttpContext.Response.Body);
		}
	}
}