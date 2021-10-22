using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net;
using AngleSharp.Dom;
using Novinichka.Services.Helpers;

namespace Novinichka.Services.NewsSources.Sources
{
    public class EpicenterBg : BaseSource
    {
        private const int CountOfNews = 5;

        private readonly List<string> Urls = new()
        {
            "category/bulgaria/2",
            "category/law/3",
            "category/business/4",
            "category/obstestvo/11",
            "category/culture/6",
            "category/world/7",
            "category/sport/8",
            "category/weekend/10",
        };

        public override string BaseUrl { get; set; } = "http://epicenter.bg";

        public override IEnumerable<NewsModel> GetRecentNews()
        {
            var allNewsUrls = new List<NewsModel>();
            foreach (var url in Urls)
            {
                allNewsUrls.AddRange(GetNewsUrls($"{this.BaseUrl}/{url}/1", url)
                    .Select(this.GetNews)
                    .Where(x => x != null)
                    .Take(CountOfNews)
                    .ToList());
            }

            return allNewsUrls;
        }

        public override IEnumerable<NewsModel> GetAllNews()
        {
            var allNewsUrls = new List<NewsModel>();
            foreach (var url in Urls)
            {
                var webClient = new WebClient();

                var html = webClient.DownloadString(this.BaseUrl + "/" + url + "/1");

                var document = this.Parser
                    .ParseDocument(html);

                var startPage = 1;
                var endPage = int.Parse(document
                    .QuerySelectorAll(".pagination a")[9]
                    .TextContent);

                for (int i = startPage; i < endPage; i++)
                {
                    var news = GetNewsUrls($"{this.BaseUrl}/{url}/{i}", url)
                        .Select(this.GetNews)
                        .Where(x => x != null)
                        .ToList();

                    allNewsUrls.AddRange(news);
                }
            }

            return allNewsUrls;
        }

        protected override NewsModel ParseDocument(IDocument document, string url)
        {
            var newsTitle = document
                .QuerySelector("td[valign=\"top\"] h1[itemprop=\"name headline\"]")
                ?.InnerHtml
                .Trim();

            //3818 | 21 окт. 2021 | 16:09
            var createdOnAsString = document
                .QuerySelector("td[valign=\"bottom\"] p.tinytext-novina")
                ?.TextContent
                .Split(" | ")[1];

            DateTime createdOn;

            try
            {
                createdOn = DateTime.Parse(createdOnAsString, new CultureInfo("bg-BG"));
            }
            catch
            {
                createdOn = DateTime.UtcNow;
                // ignored
            }

            var content = document
                .QuerySelector("div[itemprop=\"articleBody\"] p");

            var originalSourceId = this.GetOriginalIdFromSourceUrl(url);

            var image = document
                .QuerySelector("div.images_h");

            content.RemoveChildNodes(image);

            var imageUrl = image
                .OuterHtml
                .Split()[3]
                .Substring(4)
                .Trim(';')
                .Trim();

            return new NewsModel(newsTitle, content?.OuterHtml, createdOn, imageUrl.Trim(')'), url, originalSourceId);
        }

        public override string GetOriginalIdFromSourceUrl(string url)
        {
            var uri = new Uri(url.Trim().Trim('/'));
            var segment = uri.Segments[2];

            return WebUtility.UrlDecode(segment.Trim('/'));
        }

        private IEnumerable<string> GetNewsUrls(string url, string siteUrl)
        {
            var webClient = new WebClient();

            var html = webClient.DownloadString(url);

            var document = this.Parser
                .ParseDocument(html);

            return document
                .QuerySelectorAll("td.products_list a")
                .Select(x => $"{this.BaseUrl}{x?.Attributes["href"]?.Value}")
                .ToList();
        }
    }
}