using SoftNet_WS_ConsoleApp.SoftNetWS;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace SoftNet_WS_ConsoleApp
{
    internal class Program
    {
        static string username = "{username}";
        static string password = "{password}";
        static void Main(string[] args)
        {
            Integration10SoapClient client = new Integration10SoapClient();

            var token = client.CreateUserToken(username, password, ModuleType.eDispatch);

            Guid docId = Guid.Parse("{uuid}");

            var disPDF = client.GetCommonDocumentPDFByUUIDs(token, new UbltrPDFInput[]
            {
                new UbltrPDFInput()
                {
                    UUID = docId,
                    UseEmbeddedXSLT = true,
                },
            });

            byte[] pdfData = disPDF.First().PDFContent;

            byte[] zippedPdfData = Compress(pdfData);

            string zipFilePath = "example.zip";
            File.WriteAllBytes(zipFilePath, zippedPdfData);

            byte[] unzippedPdfData = Decompress(zippedPdfData);

            string unzippedPdfFilePath = "unzipped_example.pdf";
            File.WriteAllBytes(unzippedPdfFilePath, unzippedPdfData);

            Console.WriteLine("PDF dosyası başarıyla ziplendi ve tekrar çözüldü.");
        }


        public static byte[] Compress(byte[] data)
        {
            using (MemoryStream memoryStream = new MemoryStream())
            {
                using (DeflateStream deflateStream = new DeflateStream(memoryStream, CompressionMode.Compress))
                {
                    deflateStream.Write(data, 0, data.Length);
                }
                return memoryStream.ToArray();
            }
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
}
