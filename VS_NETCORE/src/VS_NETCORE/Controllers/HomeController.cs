using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Xml;
using HtmlAgilityPack;
using Microsoft.AspNetCore.Mvc;
using VS_NETCORE.Models.Curation;

namespace VS_NETCORE.Controllers
{
    public class HomeController : Controller
    {
        public async Task<IActionResult> Index()
        {
            var curationTitleList = new List<CurationViewModel>();
            curationTitleList.Add(new CurationViewModel
            {
                HPTitle = "RETRIP",
                RssURL = "https://retrip.jp/feeds/",
                CurationTitleViewModel = await GetCurationTitleViewModel("RETRIP", "https://retrip.jp/feeds/")
            });


            curationTitleList.Add(new CurationViewModel
            {
                HPTitle = "SocialGame",
                RssURL = "http://gamebiz.jp/?feed=rss",
                CurationTitleViewModel = await GetCurationTitleViewModel("SocialGame", "http://gamebiz.jp/?feed=rss")
            });

            curationTitleList.Add(new CurationViewModel
            {
                HPTitle = "痛いニュース(ﾉ∀`)",
                RssURL = "http://rssblog.ameba.jp/maokobayashi0721/rss20.xml",
                CurationTitleViewModel = await GetCurationTitleViewModel("痛いニュース(ﾉ∀`)", "http://rssblog.ameba.jp/maokobayashi0721/rss20.xml")
            });

            return View(new ViewModel
            {
                CurationViewModel = curationTitleList.ToArray()
            });
        }

        private async Task<CurationTitleViewModel[]> GetCurationTitleViewModel(string title, string rssURL)
        {
            var TestUrl = new Uri(rssURL);
            var serviceCategoryDom = await GetXmlAsync(TestUrl);

            var serviceCategoryDoaaaa =
                serviceCategoryDom.DocumentElement.SelectNodes("//item");

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
