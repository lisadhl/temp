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
    internal class Program2
    {
        static void Main(string[] args)
        {
            using (HttpClient client = new HttpClient())
            {
                string url = "http://localhost/api/filetool/UploadFile2";
                HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Post, url);
                using (MultipartFormDataContent FormData = new MultipartFormDataContent())
                {
                    NameValueCollection parameters = CreateQueryParameters();

                    foreach (string  key in parameters.Keys.Cast<string>())
                    {
                        FormData.Add(new StringContent(parameters[key]),key);
                    }
                    StreamContent content = new StreamContent(ZipMyLogs());
                    FormData.Add(content, "test2", Guid.NewGuid().ToString() + ".zip");
                    
                    request.Content = FormData;
                    HttpResponseMessage response = client.SendAsync(request).Result;
                    Console.WriteLine(response);
                }

                Console.ReadKey();
            }
        }

        static NameValueCollection CreateQueryParameters()
        {
            return new NameValueCollection
            {
                { "CompanyCode", "0001" },
                { "StoreCode", "00088" }
            };
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
