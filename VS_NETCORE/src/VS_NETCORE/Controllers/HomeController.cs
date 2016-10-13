using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Xml;
using HtmlAgilityPack;
using Microsoft.AspNetCore.Mvc;
using VS_NETCORE.Models.Curation;
using MediaTypeHeaderValue = System.Net.Http.Headers.MediaTypeHeaderValue;

namespace VS_NETCORE.Controllers
{
    public class HomeController : Controller
    {
        public async Task<IActionResult> Index()
        {
            var curationTitleList = new List<CurationViewModel>();
            //curationTitleList.Add(new CurationViewModel
            //{
            //    HPTitle = "RETRIP",
            //    RssURL = "https://retrip.jp/feeds/",
            //    CurationTitleViewModel = await GetCurationTitleViewModel("RETRIP", "https://retrip.jp/feeds/")
            //});


            //curationTitleList.Add(new CurationViewModel
            //{
            //    HPTitle = "SocialGame",
            //    RssURL = "http://gamebiz.jp/?feed=rss",
            //    CurationTitleViewModel = await GetCurationTitleViewModel("SocialGame", "http://gamebiz.jp/?feed=rss")
            //});

            //curationTitleList.Add(new CurationViewModel
            //{
            //    HPTitle = "痛いニュース(ﾉ∀`)",
            //    RssURL = "http://rssblog.ameba.jp/maokobayashi0721/rss20.xml",
            //    CurationTitleViewModel = await GetCurationTitleViewModel("痛いニュース(ﾉ∀`)", "http://rssblog.ameba.jp/maokobayashi0721/rss20.xml")
            //});

            curationTitleList.Add(new CurationViewModel
            {
                HPTitle = "ふるさとチョイス",
                RssURL = "http://www.furusato-tax.jp/notification-list.html",
                CurationTitleViewModel = await GetCurationFurusatoTitleViewModel("ふるさとチョイス", "http://www.furusato-tax.jp/notification-list.html")
            });

            curationTitleList.Add(new CurationViewModel
            {
                HPTitle = "さとふる",
                RssURL = "https://www.satofull.jp/products/list.php?&sort=number3,number8,Number19,number6&cnt=30&p=1",
                CurationTitleViewModel = await GetCurationSatofuruTitleViewModel("さとふる", "https://www.satofull.jp/products/list.php?&sort=number3,number8,Number19,number6&cnt=30&p=1")
            });




            return View(new ViewModel
            {
                CurationViewModel = curationTitleList.ToArray()
            });
        }

        [HttpGet, Route("api/events")]
        public async Task<IActionResult> SendMessageRequest(string message, string color, bool notify)
        { 
            try
            {
                if (message.Length > 15)
                {
                    message = message.Substring(0, 15);
                }

                var andMore = "(and more...)";
                if (message.Length + andMore.Length > 5000)
                {
                    message = (message + andMore).Substring(0, 5000);
                }

                var url = $"https://api.hipchat.com/v2/room/3206474/notification?auth_token=8MxdsvAp2dsPfpEHP7ObrzOsEUoCTopwD8FZdnfh";

                HttpClient httpClient = new HttpClient();
                var query_string = $"notify={(notify ? '1' : '0')}&" +
                              $"message={WebUtility.UrlEncode(message)}&" +
                              $"color={color}&" +
                              $"message_format={"text"}";
                HttpContent contentTest = new StringContent(query_string);
                contentTest.Headers.ContentType = new MediaTypeHeaderValue("application/x-www-form-urlencoded");
                var responsetest = await httpClient.PostAsync(url, contentTest);
                String text = await responsetest.Content.ReadAsStringAsync();

            }
            catch (Exception e)
            {

            }
            return Json("OK");
        }

        private async Task<CurationTitleViewModel[]> GetCurationTitleViewModel(string title, string rssURL)
        {
            var TestUrl = new Uri(rssURL);
            var serviceCategoryDom = await GetXmlAsync(TestUrl);

            var serviceCategoryDoaaaa =
                serviceCategoryDom.DocumentElement.SelectNodes("//item");
            var serviceCategoryentry =
                serviceCategoryDom.DocumentElement.SelectNodes("//entry");

            var titleList = serviceCategoryDom.DocumentElement.SelectNodes("//title");
            var linkList = serviceCategoryDom.DocumentElement.SelectNodes("//link");
            var data = new List<CurationTitleViewModel>();
            for (var x = 1; x <= serviceCategoryDoaaaa.Count; x++)
            {
                if (titleList[x].InnerText.Contains(title))
                    continue;

                var rss = new CurationTitleViewModel
                {
                    Title = ShrotText(titleList[x].InnerText),
                    Link = linkList[x].InnerText
                };
                data.Add(rss);
            }
            return data.ToArray();
        }

        private async Task<CurationTitleViewModel[]> GetCurationFurusatoTitleViewModel(string title, string rssURL)
        {
            var TestUrl = new Uri(rssURL);
            var serviceCategoryDom = await GetHtmlAsync(TestUrl);

            HtmlAgilityPack.HtmlNode contents = serviceCategoryDom.DocumentNode.SelectSingleNode("//div[@class='newbox_spmargin']");
            Console.WriteLine(contents.InnerHtml);

            var li = contents.SelectNodes("//li[@class='clearfix']");
            var clearfixshPrefectureSpWrapper = contents.SelectNodes("//div[@class='prefectureSpWrapper']");
            var clearfixsh4 = clearfixshPrefectureSpWrapper.SelectMany(x => x.Descendants("h4"));

            var clearfixsh4linkdetail = clearfixsh4.SelectMany(x => x.Descendants("a")).Select(x => x.Attributes["href"].Value.Contains("detail") ? x : null);
            var data = new List<CurationTitleViewModel>();
            foreach (var h4data in clearfixsh4linkdetail.Where(x => x != null))
            {
                var rss = new CurationTitleViewModel
                {
                    Title = h4data.InnerText,
                    Link  = h4data.Attributes["href"].Value
                };
                data.Add(rss);
            }

            return data.ToArray();
        }

        private async Task<CurationTitleViewModel[]> GetCurationSatofuruTitleViewModel(string title, string rssURL)
        {
            var TestUrl = new Uri(rssURL);
            var serviceCategoryDom = await GetHtmlAsync(TestUrl);

            HtmlAgilityPack.HtmlNode contents = serviceCategoryDom.DocumentNode.SelectSingleNode("//div[@class='articleborder']");
            Console.WriteLine(contents.InnerHtml);
            var li = contents.SelectNodes("//li[@class='pitem']");
            var clearfixsh4 = li.SelectMany(x => x.Descendants("a"));
            var clearfixsh4linkdetail = clearfixsh4.Select(x => x.Attributes["href"].Value.Contains("detail") ? x : null);
            var data = new List<CurationTitleViewModel>();
            foreach (var h4data in clearfixsh4linkdetail.Where(x => !x.InnerHtml.Contains("valuation clearfix")))
            {
                var rss = new CurationTitleViewModel
                {
                    Title =  h4data.InnerText,
                    Link = "https://www.satofull.jp" + h4data.Attributes["href"].Value
            };
                data.Add(rss);
            }
            return data.ToArray();
        }

        private async Task<HtmlDocument> GetHtmlAsync(Uri url)
        {
            using (var client = new HttpClient())
            {
                using (System.IO.Stream stream = await client.GetStreamAsync(url))
                {
                    //var sr = new StreamReader(stream, Encoding.UTF8);
                    var htmlDoc = new HtmlAgilityPack.HtmlDocument();
                    htmlDoc.Load(stream);

                    return htmlDoc;
                }
            }
        }

        private async Task<XmlDocument> GetXmlAsync(Uri url)
        {
            using (var client = new HttpClient())
            {
                using (System.IO.Stream stream = await client.GetStreamAsync(url))
                {
                    //var sr = new StreamReader(stream, Encoding.UTF8);
                    var xmlDoc = new XmlDocument();
                    xmlDoc.Load(stream);

                    return xmlDoc;
                }
            }
        }

        public IActionResult About()
        {
            ViewData["Message"] = "Your application description page.";

            return View();
        }

        public IActionResult Contact()
        {
            ViewData["Message"] = "Your contact page.";

            return View();
        }

        public IActionResult Error()
        {
            return View();
        }

        public string ShrotText(string text)
        {
            if (text.Length > 20) return text.Substring(0, 20);
            return text;
        }
    }

}
