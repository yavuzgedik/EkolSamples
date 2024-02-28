using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO.Compression;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace SoftNet_DS_ConsoleApp
{
    internal class Program
    {
        static async Task Main(string[] args)
        {
            HttpClient client = new HttpClient();

            string apiUrl = "{api_url}";

            try
            {
                // JSON verisini oluşturun
                var requestData = new
                {
                    Username = "{username}",
                    Password = "{password}",
                    Vkn = "{vkn}",
                    Uuid = "{Uuid}"
                };

                // JSON verisini Serialize edin
                // Newtonsoft kütüphanesi gereklidir.
                string requestBody = JsonConvert.SerializeObject(requestData);

                // İsteği oluşturun
                HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Post, apiUrl);
                request.Content = new StringContent(requestBody, Encoding.UTF8, "application/json");

                // POST isteği gönderin ve yanıtı alın
                HttpResponseMessage response = await client.SendAsync(request);

                // Yanıtı kontrol edin
                if (response.IsSuccessStatusCode)
                {
                    // Yanıt içeriğini okuyun
                    string responseContent = await response.Content.ReadAsStringAsync();
                    var responseModel = JsonConvert.DeserializeObject<ResponseVM>(responseContent);

                    byte[] unzippedPdfData = Decompress(responseModel.Data.PdfData);

                    string unzippedPdfFilePath = "unzipped_example.pdf";
                    File.WriteAllBytes(unzippedPdfFilePath, unzippedPdfData);

                    Console.WriteLine("PDF dosyası başarıyla ziplendi ve tekrar çözüldü.");
                }
                else
                {
                    Console.WriteLine("API isteği başarısız: " + response.StatusCode);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("API bağlantısı sırasında hata oluştu: " + ex.Message);
            }

            Console.ReadLine();
        }
        public static byte[] Decompress(byte[] data)
        {
            using (MemoryStream compressedStream = new MemoryStream(data))
            using (MemoryStream resultStream = new MemoryStream())
            using (DeflateStream deflateStream = new DeflateStream(compressedStream, CompressionMode.Decompress))
            {
                deflateStream.CopyTo(resultStream);
                return resultStream.ToArray();
            }
        }
    }

    public class ResponseDispatch
    {
        public string UUID { get; set; }
        public string XsltData { get; set; }
        public byte[] PdfData { get; set; }
    }

    public class ResponseVM
    {
        public bool State { get; set; }
        public string Message { get; set; }
        public ResponseDispatch Data { get; set; }
    }
}
