using HtmlAgilityPack;
using System.IO;
using System.Net;
using System.Threading.Tasks;
using System.Linq;
using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using System.Text.RegularExpressions;
using Newtonsoft.Json.Linq;

namespace InfoWeb.Areas.Etc.Models
{
    public class ProxyFinder
    {
        public async Task<List<ProxyEndpoint>> FindAsync()
        {

            WebRequest request = HttpWebRequest.Create("http://www.gatherproxy.com/proxylist/country/?c=China");

            string html = string.Empty;
            using (StreamReader reader = new StreamReader((await request.GetResponseAsync()).GetResponseStream()))
            {
                html = await reader.ReadToEndAsync();
            }

            HtmlDocument htmlDoc = new HtmlDocument();
            htmlDoc.LoadHtml(html);

            HtmlNode node = htmlDoc.GetElementbyId("tblproxy");
            var nodes = node.SelectNodes("script");
            //var noddd = nodes.Where(n => n.GetAttributeValue("class", "").Contains("proxy"));
            List<ProxyEndpoint> proxyEndpoints = new List<ProxyEndpoint>();
            foreach (HtmlNode n in nodes)
            {
                try
                {
                    Match match = Regex.Match(n.InnerText, "{.*}");
                    string json = match.Groups[0].Value;
                    dynamic stuff = JObject.Parse(json);

                    //[0-9].
                    string hostname = stuff.PROXY_IP;
                    string port_hex = stuff.PROXY_PORT;
                    int port = int.Parse(port_hex, System.Globalization.NumberStyles.HexNumber);
                    string type = stuff.PROXY_TYPE;
                    ProxyType proxyType;
                    Enum.TryParse(type, out proxyType);
                    ProxyEndpoint proxyEndpoint = new ProxyEndpoint(hostname, port, proxyType);
                    proxyEndpoints.Add(proxyEndpoint);
                }
                catch
                {

                }

            }
            return proxyEndpoints;
        }
    }
}