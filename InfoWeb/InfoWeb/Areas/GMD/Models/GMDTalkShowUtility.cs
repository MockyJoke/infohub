using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Timers;
using System.Web;

namespace InfoWeb.Areas.GMD.Models
{
    public class GMDTalkShowUtility
    {
        private static GMDTalkShowUtility _instance = null;
        public static GMDTalkShowUtility Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new GMDTalkShowUtility();
                }
                return _instance;
            }
        }
        public List<GMDTalkShow> ShowList { get; private set; }
        private Timer timer;
        private GMDTalkShowUtility()
        {
            Refresh();
            timer = new Timer(3600000);
            timer.Elapsed += (sender, e) =>
            {
                Refresh();
            };
            timer.Start();
        }

        public void Refresh()
        {
            try
            {
                ShowList = GMDTalkShow.LoadAll3().Result;

            }
            catch
            {
                ShowList = new List<GMDTalkShow>();
            }
        }
    }

    public class TalkShowLink
    {
        public string Content;
        public string Url;
    }
    public class GMDTalkShow
    {
        public string Title { get; private set; }
        public string SourceUrl { get; private set; }

        public List<TalkShowLink> Links { get; private set; }

        public GMDTalkShow(string url)
        {
            SourceUrl = url;
        }

        public async Task<bool> LoadFromUrlAsync()
        {
            string html = await GetPageHtmlAsync(SourceUrl).ConfigureAwait(false);
            HtmlDocument htmlDoc = new HtmlDocument();
            htmlDoc.LoadHtml(html);

            Title = ExtractTitle(htmlDoc);
            ExtractLinks(htmlDoc);

            return true;
        }
        private string ExtractTitle(HtmlDocument htmlDoc)
        {
            HtmlNode node = htmlDoc.GetElementbyId("main");
            HtmlNodeCollection nodes = node.SelectNodes("article/div/h1");
            return HttpUtility.HtmlDecode(nodes.FirstOrDefault().InnerText);
        }

        private void ExtractLinks(HtmlDocument htmlDoc)
        {
            Links = new List<TalkShowLink>();
            HtmlNode node = htmlDoc.GetElementbyId("main");
            var nodes = node.SelectNodes("article/div/p/span").Where(n => n.InnerText.Contains("archive")).ToList();
            var linkNode = nodes.FirstOrDefault();
            if (linkNode == null) return;
            if (linkNode.InnerText.Contains(".mp3"))
            {
                TalkShowLink link = new TalkShowLink() { Content = "Link", Url = linkNode.InnerText };
                Links.Add(link);
                string url2 = link.Url.Substring(0, link.Url.LastIndexOf('/'));
                TalkShowLink link2 = new TalkShowLink() { Content = "Link", Url = url2.Replace("download","details") };
                Links.Add(link2);

            }
            else
            {
                TalkShowLink link = new TalkShowLink() { Content = "Link", Url = linkNode.InnerText };
                Links.Add(link);
            }


            //List<HtmlNode> links = nodes.Where(n => n.InnerText.Contains("h。t。t。")).ToList();

            //if (links.Count > 3)
            //{
            //    Links = new List<TalkShowLink>();
            //    for (int i = 0; i < 3; i++)
            //    {
            //        Regex pattern = new Regex(@"[。\s]|[\n]{2}");
            //        string content = HttpUtility.HtmlDecode(pattern.Replace(links[i].InnerText, ""));
            //        int linkStartPos = content.IndexOf("http");
            //        Match url = Regex.Match(content, @"http.*?((?=\s)|(?=\z))");

            //        TalkShowLink link = new TalkShowLink() { Content = content.Substring(0, linkStartPos - 1), Url = url.Value };
            //        Links.Add(link);
            //        //"SD音質1(節目後30分鍾更新)https://archive.org/details/sd20160229s <!--test1-->(adsbygoogle=window.adsbygoogle||[]).push({});"
            //        //"SD音質MP3下載 (節目後30分鍾更新)https://archive.org/download/sd20160229s/sd20160229s.mp3"
            //    }
            //    TalkShowLink tmp = Links[0];
            //    Links[0] = Links[1];
            //    Links[1] = tmp;
            //}
        }
        public static async Task<string> GetPageHtmlAsync(string url)
        {
            HttpWebRequest request = WebRequest.Create(url) as HttpWebRequest;
            HttpWebResponse response = await request.GetResponseAsync().ConfigureAwait(false) as HttpWebResponse;
            string html;
            using (StreamReader reader = new StreamReader(response.GetResponseStream()))
            {
                html = await reader.ReadToEndAsync().ConfigureAwait(false);
            }
            return html;
        }

        public static List<GMDTalkShow> LoadAll1()
        {
            string html = GMDTalkShow.GetPageHtmlAsync("http://gmdwith.us").Result;
            HtmlDocument htmlDoc = new HtmlDocument();
            htmlDoc.LoadHtml(html);
            HtmlNode node = htmlDoc.GetElementbyId("text-4");
            HtmlNodeCollection nodes = node.SelectNodes("div/a");
            List<GMDTalkShow> talkShowList = new List<GMDTalkShow>();
            foreach (HtmlNode n in nodes)
            {
                string url = n.GetAttributeValue("href", "");
                if (url != "")
                {
                    GMDTalkShow talkShow = new GMDTalkShow(url);
                    bool result = talkShow.LoadFromUrlAsync().Result;
                    if (result)
                    {
                        talkShowList.Add(talkShow);
                    }
                }
            }
            return talkShowList;
        }

        public static List<GMDTalkShow> LoadAll2()
        {
            string html = GMDTalkShow.GetPageHtmlAsync("http://gmdwith.us").Result;
            HtmlDocument htmlDoc = new HtmlDocument();
            htmlDoc.LoadHtml(html);
            HtmlNode node = htmlDoc.GetElementbyId("sidebar");
            var nodes = node.SelectNodes("div/ul/li/ul/li/ul/li/a").Where(n => n.InnerText.Contains("月"));
            //var list1 = LoadCatagory(nodes.Last().GetAttributeValue("href", ""));
            //var list2 = LoadCatagory(nodes.ToList()[nodes.Count() - 2].GetAttributeValue("href", ""));
            //return LoadCatagory(nodes.Last().GetAttributeValue("href", ""));
            return new List<Models.GMDTalkShow>();
        }
        private async static Task<List<GMDTalkShow>> LoadCatagory(string catagoryUrl)
        {
            string html = await GMDTalkShow.GetPageHtmlAsync(catagoryUrl).ConfigureAwait(false);
            HtmlDocument htmlDoc = new HtmlDocument();
            htmlDoc.LoadHtml(html);

            HtmlNode node = htmlDoc.GetElementbyId("main");
            var nodes = node.SelectNodes("article/div/h2/a");


            List<GMDTalkShow> talkShowList = new List<GMDTalkShow>();
            foreach (HtmlNode n in nodes)
            {

                string url = n.GetAttributeValue("href", "");
                if (url != "")
                {
                    GMDTalkShow talkShow = new GMDTalkShow(url);
                    bool result = await talkShow.LoadFromUrlAsync().ConfigureAwait(false);
                    if (result)
                    {
                        talkShowList.Add(talkShow);
                    }
                }
            }
            return talkShowList;
        }

        public static async Task<List<GMDTalkShow>> LoadAll3()
        {
            string html = await GMDTalkShow.GetPageHtmlAsync("http://gmdwith.us").ConfigureAwait(false);
            HtmlDocument htmlDoc = new HtmlDocument();
            htmlDoc.LoadHtml(html);
            HtmlNode node = htmlDoc.GetElementbyId("main");
            var nodes = node.SelectNodes("article/div/span/a").Where(n => n.InnerText.Contains("月"));

            var catNode = nodes.ToList().FirstOrDefault();
            if (catNode == null)
            {
                return new List<Models.GMDTalkShow>();
            }
            string catUrl = catNode.GetAttributeValue("href", "");
            var list1 = await LoadCatagory(catUrl);
            var list2 = await LoadPrevCatagoryAsync(catUrl);
            return list1.Concat(list2).ToList();
        }
        public static async Task<List<GMDTalkShow>> LoadPrevCatagoryAsync(string catUrl)
        {
            var list = catUrl.Split('=');
            try
            {
                int catId = Int32.Parse(list[1]);
                return await LoadCatagory(string.Format("{0}={1}", list[0], catId-1));
            }
            catch
            {
                return new List<GMDTalkShow>();
            }
        }
    }

}