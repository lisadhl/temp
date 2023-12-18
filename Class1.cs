using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.IO;

using Windows.Web.Http;
using System.Globalization;
using System.Text.RegularExpressions;
using System.IO.Compression;
using Windows.UI;
using System.Net.Http.Headers;

namespace ClassLibrary1
{
    public class Class1
    {
        public static void Upload()
        {
            // https://learn.microsoft.com/en-us/dotnet/api/system.io.compression.zipfile.createfromdirectory?view=net-8.0#system-io-compression-zipfile-createfromdirectory(system-string-system-io-stream)
            // https://stackoverflow.com/questions/16416601/c-sharp-httpclient-4-5-multipart-form-data-upload

            HttpClient client = new HttpClient();
            HttpRequestMessage request = null;

            MultipartContent contents = new MultipartContent();

            MultipartFormDataContent formData = new MultipartFormDataContent();
            //formData.Add()
            StreamContent contenet = new StreamContent();
        }
        public static async Task<string> Upload(byte[] image)
        {
            using (var client = new System.Net.Http.HttpClient())
            {
                using (var content =
                    new MultipartFormDataContent("Upload----" + DateTime.Now.ToString(CultureInfo.InvariantCulture)))
                {
                    content.Add(new StreamContent(new MemoryStream(image)), "bilddatei", "upload.jpg");

                    using (
                       var message =
                           await client.PostAsync("http://www.directupload.net/index.php?mode=upload", content))
                    {
                        var input = await message.Content.ReadAsStringAsync();

                        return !string.IsNullOrWhiteSpace(input) ? Regex.Match(input, @"http://\w*\.directupload\.net/images/\d*/\w*\.[a-z]{3}").Value : null;
                    }
                }
            }
        }

        public static void Upload2()
        {
            // https://learn.microsoft.com/en-us/dotnet/api/system.io.compression.zipfile.createfromdirectory?view=net-8.0#system-io-compression-zipfile-createfromdirectory(system-string-system-io-stream)
            // https://stackoverflow.com/questions/16416601/c-sharp-httpclient-4-5-multipart-form-data-upload

            // 保存目标
            string webPath = "https://192.168.3.8/0000/00807/0003/";
            // 数据来源
            //string startPath = @".\start";
            //string zipPath = @".\log.zip";
            string startPath = "D://teststudy/log/";
            string zipPath = "D://teststudy/log.zip";
            //压缩
            ZipFile.CreateFromDirectory(startPath, zipPath);
            if (!Directory.Exists(zipPath))
            {
                Directory.CreateDirectory(zipPath);
            }
            // zip转流
            FileStream fs = File.OpenRead(zipPath);
            var streamContent = new StreamContent(fs);

            var FileContent = new ByteArrayContent(streamContent.ReadAsByteArrayAsync().Result);
            FileContent.Headers.ContentType = MediaTypeHeaderValue.Parse("ContentType");

            var handler = new WebRequestHandler();
            handler.ClientCertificateOptions = ClientCertificateOption.Manual;
            handler.ClientCertificates.Add(clientKey1);
            handler.ServerCertificateValidationCallback = (httpRequestMessage, cert, cetChain, policyErrors) =>
            {
                return true;
            };
            using (var client = new HttpClient(handler))
            {
                // Post it
                System.Net.Http.HttpResponseMessage httpResponseMessage = client.PostAsync(webPath, FileContent).Result;

                if (!httpResponseMessage.IsSuccessStatusCode)
                {
                    string ss = httpResponseMessage.StatusCode.ToString();
                }
            }
        }
    }
}
