using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Reflection.Metadata;
using System.Threading.Tasks;
using System.Xml;
using HtmlAgilityPack;
using Microsoft.AspNetCore.Mvc;
using VS_NETCORE.Models;
using VS_NETCORE.Models.Curation;

// For more information on enabling MVC for empty projects, visit http://go.microsoft.com/fwlink/?LinkID=397860

namespace VS_NETCORE.Controllers
{
    public class CurationController : Controller
    {
        private readonly static Uri RootUrl = new Uri("https://as.its-kenpo.or.jp/service_category/index");

        [HttpGet, Route("api/service_groups")]
        public async Task<IActionResult> Index()
        {

            // service_category
            var serviceCategoryDom = await GetHtmlAsync(RootUrl);
            var serviceGroupUrl = serviceCategoryDom.DocumentNode.SelectNodes("//a")
                .Where(x => x.InnerText == "直営・通年・夏季保養施設(空き照会)")
                .Select(x => x.Attributes["href"].Value)
                .Select(x => $"{RootUrl.Scheme}://{RootUrl.Host}" + x)
                .Select(x => new Uri(x))
                .First();

            // service_group
            var serviceGroupDom = await GetHtmlAsync(serviceGroupUrl);
            var serviceApplyUrls = serviceGroupDom.DocumentNode.SelectNodes("//li")
                .SelectMany(x => x.Descendants("a"))
                .Select(x => x.Attributes["href"].Value)
                .Select(x => $"{RootUrl.Scheme}://{RootUrl.Host}" + x);

            return Json(serviceApplyUrls);
        }

        // GET: /<controller>/
        [HttpGet, Route("api/service_apply")]
        public async Task<IActionResult> Index(string url)
        {
            var availabilities = new List<Availability>();

            // service_apply
            var serviceApplyDom = await GetHtmlAsync(new Uri(url));
            var applyUrls = serviceApplyDom.DocumentNode.SelectNodes("//li")
                .SelectMany(x => x.Descendants("a"))
                .Select(x => x.Attributes["href"].Value)
                .Select(x => $"{RootUrl.Scheme}://{RootUrl.Host}" + x)
                .Select(x => new Uri(x));

            // apply
            var applyDoms = await Task.WhenAll(applyUrls.Select(async x => new { Url = x, Dom = await GetHtmlAsync(x) }));
            foreach (var applyDom in applyDoms)
            {
                var title = applyDom.Dom.DocumentNode.SelectNodes("//table")
                    .Where(x => x.Attributes["class"] != null)
                    .Where(x => x.Attributes["class"].Value == "tform_new")
                    .Select(x => x.Descendants("tr").First())
                    .Select(x => x.Descendants("td").Last().InnerText)
                    .First();
                var dates = applyDom.Dom.DocumentNode.SelectNodes("//select")
                    .Where(x => x.Attributes["id"] != null)
                    .Where(x => x.Attributes["id"].Value == "apply_join_time")
                    .SelectMany(x => x.Descendants("option"))
                    .Select(x => x.Attributes["value"].Value)
                    .Where(x => !string.IsNullOrEmpty(x))
                    .Select(x => new AvailabilityDateTime(Convert.ToDateTime(x)));

                var availability = new Availability();
                availability.Title = title;
                availability.Url = applyDom.Url;
                availability.Dates = dates;
                availabilities.Add(availability);
            }

            return Json(availabilities.OrderBy(x => x.Title));
        }

        public async Task<IActionResult> Test()
        {
            var TestUrl = new Uri("https://retrip.jp/feeds/");
            var serviceCategoryDom = await GetHtmlAsync(TestUrl);

            var serviceCategoryDoaaaa =
                serviceCategoryDom.DocumentNode.SelectNodes("//item");
            var titleList = serviceCategoryDom.DocumentNode.SelectNodes("//title");
            var linkList = serviceCategoryDom.DocumentNode.SelectNodes("//link");
            var data = new List<Rss>();
            for (var x = 1; x <= serviceCategoryDoaaaa.Count; x++)
            {
                var rss = new Rss
                {
                    Title = titleList[x].ToString(),
                    Link = linkList[x].ToString()
                };
                data.Add(rss);
            }

            return Json(serviceCategoryDom);
        }


        private async Task<HtmlDocument> GetHtmlAsync(Uri url)
        {
            using (var client = new HttpClient())
            {
                using (System.IO.Stream stream = await client.GetStreamAsync(url))
                {
                    var htmlDoc = new HtmlAgilityPack.HtmlDocument();
                    htmlDoc.Load(stream);
                    return htmlDoc;
                }
            }
        }

        private async Task<XmlReader> GetHtmlAsync2(Uri url)
        {
            using (var client = new HttpClient())
            {
                using (var stream = await client.GetStreamAsync(url))
                {
                    var htmlDoc = new HtmlAgilityPack.HtmlDocument();
                    var reader = XmlReader.Create(stream);
                    return reader;
                }

            }
        }
    }

    public class Rss
    {
        public string Title;
        public string Link;
    }
}
