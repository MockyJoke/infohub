using System.IO;
using System.Net;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace InfoWeb.Areas.Etc.Controllers
{
    public class PacController : Controller
    {
        // GET: Etc/Pac
        public const string PAC_ORIGIN = @"http://yo.uku.im/proxy.pac";
        private const string Matching_Regex = @"_proxy_str\s?=.*PROXY (\S*).*";
        public async Task<ActionResult> Index()
        {
            WebRequest request = HttpWebRequest.Create(PAC_ORIGIN);
            string result = string.Empty;
            string pac = string.Empty;
            using (StreamReader reader = new StreamReader((await request.GetResponseAsync()).GetResponseStream()))
            {
                pac = await reader.ReadToEndAsync();
                Match match = Regex.Match(pac, Matching_Regex);
                result = pac.Replace(match.Groups[1].Value, "cnproxy.funkygeek.me:443;");
            }

            Response.AppendHeader("Content-Disposition", "proxy.pac");
            Stream resultStream = GenerateStreamFromString(result);

            return File(resultStream, "application/x-ns-proxy-autoconfig");
        }

        public Stream GenerateStreamFromString(string s)
        {
            MemoryStream stream = new MemoryStream();
            StreamWriter writer = new StreamWriter(stream);
            writer.Write(s);
            writer.Flush();
            stream.Position = 0;
            return stream;
        }
    }
}
