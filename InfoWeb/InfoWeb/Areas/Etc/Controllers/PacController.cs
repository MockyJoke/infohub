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
            using (StreamReader reader = new StreamReader((await request.GetResponseAsync()).GetResponseStream()))
            {
                string pac = await reader.ReadToEndAsync();
                Match match = Regex.Match(pac, Matching_Regex);
                result = pac.Replace(match.Groups[1].Value, "139.224.13.191:2737;");
            }

            return Content(result, "application/json");
        }
        public ActionResult Proxy1()
        {
            return View();
        }
        public ActionResult Proxy2()
        {
            return View();
        }
    }
}
