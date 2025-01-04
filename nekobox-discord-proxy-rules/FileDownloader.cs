using System.Drawing;
using System.Net;
using Colorful;
using Console = Colorful.Console;

namespace nekobox_discord_proxy_rules
{
    public class FileDownloader
    {
        public static string DownloadFile(string url, string downloadPath )
        {
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Ssl3 | SecurityProtocolType.Tls | SecurityProtocolType.Tls12 | SecurityProtocolType.Tls11;
            ServicePointManager.ServerCertificateValidationCallback = delegate { return true; };
            
            using (WebClient client = new WebClient())
            {
                Console.WriteLineFormatted($"Downloading file {url}", Color.Blue);
                client.DownloadFile(url, downloadPath);
            }
            Console.WriteLineFormatted("Download complete.", Color.Green);
            return downloadPath;
        }
        
        public static string DownloadString(string url )
        {
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Ssl3 | SecurityProtocolType.Tls | SecurityProtocolType.Tls12 | SecurityProtocolType.Tls11;
            ServicePointManager.ServerCertificateValidationCallback = delegate { return true; };
            
            using (WebClient client = new WebClient())
            {
                client.Encoding = System.Text.Encoding.UTF8;
                Console.WriteLineFormatted($"Downloading string file {url}", Color.Blue);
                return client.DownloadString(url);
            }
        }
    }
}