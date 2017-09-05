using InfoWeb.Areas.Etc.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace InfoWeb.Areas.Etc.Controllers
{
    public class PacController : Controller
    {
        // GET: Etc/Pac
        public const string PAC_ORIGIN = @"http://yo.uku.im/proxy.pac";
        private const string Matching_Regex = @"_proxy_str\s?=.*PROXY (\S*).*";
        public async Task<ActionResult> Index(string server)
        {
            WebRequest request = HttpWebRequest.Create(PAC_ORIGIN);
            string result = string.Empty;
            string pac = string.Empty;
            using (StreamReader reader = new StreamReader((await request.GetResponseAsync()).GetResponseStream()))
            {
                pac = await reader.ReadToEndAsync();
                Match match = Regex.Match(pac, Matching_Regex);
                if (server == null)
                {
                    //result = pac.Replace(match.Groups[1].Value, "cnproxy.funkygeek.me:443;");
                }
                else if (server == "auto")
                {
                    ProxyTester tester = new ProxyTester();
                    ProxyFinder proxyFinder = new ProxyFinder();
                    var endpoints = (await proxyFinder.FindAsync().ConfigureAwait(false)).Where(ep => ep.ProxyType == ProxyType.Elite);
                    CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();
                    var proxy = await tester.PickFastestAsync(endpoints, ProxyTestFunc, cancellationTokenSource.Token).ConfigureAwait(false);
                    result = pac.Replace(match.Groups[1].Value, $"{proxy.ProxyEndpoint.Hostname}:{proxy.ProxyEndpoint.Port}" + ";");
                }
                else
                {
                    result = pac.Replace(match.Groups[1].Value, server + ";");
                }
            }

            Response.AppendHeader("Content-Disposition", "proxy.pac");
            Stream resultStream = GenerateStreamFromString(result);

            return File(resultStream, "application/x-ns-proxy-autoconfig");
        }

        private static bool ProxyTestFunc(ProxyEndpoint proxyEndpoint)
        {
            return Task.Run(async () =>
            {
                string neteaseCheckUrl = "http://ipservice.163.com/isFromMainland";
                WebRequest request = HttpWebRequest.Create(neteaseCheckUrl);
                WebProxy myproxy = new WebProxy(proxyEndpoint.Hostname, proxyEndpoint.Port);
                myproxy.BypassProxyOnLocal = false;
                request.Proxy = myproxy;
                request.Method = "GET";
                string responseText = string.Empty;
                using (StreamReader reader = new StreamReader((await request.GetResponseAsync()).GetResponseStream()))
                {
                    responseText = await reader.ReadToEndAsync();
                    return responseText.ToLower().Contains("true");
                }
            }).Result;
        }

        private Stream GenerateStreamFromString(string s)
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
