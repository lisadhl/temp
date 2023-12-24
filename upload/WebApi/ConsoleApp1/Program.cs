using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Http;
using System.IO;
using System.IO.Compression;
using System.Configuration;
using System.Collections.Specialized;
using System.Net;

namespace ConsoleApp1
{
    internal class Program
    {
        static void Main(string[] args)
        {
            using (HttpClient client = new HttpClient())
            {
                string url = $"http://localhost/api/filetool/UploadFile?{CreateQueryParameters()}";
                HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Post, url);
                using (MultipartFormDataContent FormData = new MultipartFormDataContent())
                {
                    StreamContent content = new StreamContent(ZipMyLogs());
                    FormData.Add(content, "test2", Guid.NewGuid().ToString() + ".zip");
                    request.Content = FormData;
                    HttpResponseMessage response = client.SendAsync(request).Result;
                    Console.WriteLine(response);
                }

                Console.ReadKey();
            }
        }

        static string CreateQueryParameters()
        {
            NameValueCollection parameters = new NameValueCollection
            {
                { "CompanyCode", "0000" },
                { "StoreCode", "00087" }
            };

            return string.Join(
                    "&",
                    from key in parameters.Keys.Cast<string>()
                    select $"{key}={WebUtility.UrlEncode(parameters[key])}"                    
                );
        }

        private static Stream ZipMyLogs()
        {
            string zippingDir = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "MyLogs");
            string tempFile = Path.Combine(GetTempDir(), Guid.NewGuid().ToString());

            ZipFile.CreateFromDirectory(zippingDir, tempFile);

            return File.OpenRead(tempFile);
        }

        private static string GetTempDir()
        {
            string tempDir = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "temp");

            if (!Directory.Exists(tempDir))
            {
                Directory.CreateDirectory(tempDir);
            }

            return tempDir;
        }
    }
}
