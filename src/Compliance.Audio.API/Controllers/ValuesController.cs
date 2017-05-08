using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Web.Http;
using Compliance.Audio.API.Code;

namespace Compliance.Audio.API.Controllers
{
    public class ValuesController : ApiController
    {
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
        public HttpResponseMessage Post([FromBody]string value)
        {
            var path = value;
            bool deleteInput = false;
            bool needConversion = true;

            if (path.Contains('#'))
            {
                path = OpenPriSFTP.GetTempFile(value);
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
                    File.Copy(path, newname);
                    deleteInput = true;
                    path = newname;
                }
            }

            var converter = new Converter();
            MemoryStream stream;

            HttpResponseMessage result = new HttpResponseMessage(HttpStatusCode.OK);

            try
            {
                if (needConversion)
                    stream = converter.Convert(path, deleteInput);
                else
                    stream = new MemoryStream(File.ReadAllBytes(path));

                result.Content = new StreamContent(stream);
                result.Content.Headers.ContentType =
                    new MediaTypeHeaderValue("application/octet-stream");
            }
            catch (Exception ex)
            {
                result = Request.CreateErrorResponse(HttpStatusCode.BadRequest, ex.Message);
            }

            return result;

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
}