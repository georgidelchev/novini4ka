using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net;
using AngleSharp.Dom;
using Novinichka.Services.Helpers;

namespace Novinichka.Services.NewsSources.Sources
{
    public class BnbBg : BaseSource
    {
        private const string Url = "AboutUs/PressOffice/POPressReleases/POPRDate/";
        private const string UrlPaginationFragment = "index.htm?forYear=";
        private const string UrlShouldContain = "bnb.bg";
        private const string AnchorTagSelector = "div.content h3 a";

        private const int CountOfNews = 5;
        private const int StartYear = 1998;
        private int endYear = DateTime.UtcNow.Year;

        public override string BaseUrl { get; set; } = "https://bnb.bg/";

        public override IEnumerable<NewsModel> GetRecentNews()
            => GetNewsUrls($"{this.BaseUrl}{Url}{UrlPaginationFragment}")
                .Select(this.GetNews)
                .Where(x => x != null)
                .Take(CountOfNews)
                .ToList();

        public override IEnumerable<NewsModel> GetAllNews()
        {
            for (var i = StartYear; i <= this.endYear; i++)
            {
                var news = GetNewsUrls($"{this.BaseUrl}{Url}{UrlPaginationFragment}{i}")
                    .Select(this.GetNews)
                    .Where(x => x != null)
                    .ToList();

                foreach (var element in news)
                {
                    yield return element;
                }
            }
        }

        private IEnumerable<string> GetNewsUrls(string url)
        {
            var webClient = new WebClient();

            var html = webClient.DownloadString(url);

            var document = this.Parser
                .ParseDocument(html);

            return document
                .QuerySelectorAll(".content h3 a")
                .Select(x => $"{this.BaseUrl}{Url}{x?.Attributes["href"]?.Value}")
                .ToList();
        }

        protected override NewsModel ParseDocument(IDocument document, string url)
        {
            var content = document.QuerySelector("#main");
            content.RemoveChildNodes(content?.QuerySelector("p"));

            var createdOnAsString = content?.QuerySelector("p")?.TextContent.Trim();
            if (!DateTime.TryParseExact(createdOnAsString, "d MMMM yyyy г.", new CultureInfo("bg-BG"), DateTimeStyles.None, out var createdOn))
            {
                return null;
            }

            content.RemoveChildNodes(content.QuerySelector("p"));
            content.RemoveComments();
            content.RemoveGivenTag("comment");

            var newsTitle = content
                .QuerySelector("p")
                ?.TextContent
                .Replace('\n', ' ')
                .Replace('\t', ' ')
                .Replace('\r', ' ')
                .Trim()
                .TrimEnd('.')
                .Trim();

            if (newsTitle != null && newsTitle.Length > 500)
            {
                newsTitle = "ПРЕССЪОБЩЕНИЕ";
            }

            var originalSourceId = this.GetOriginalIdFromSourceUrl(url);

            return new NewsModel(newsTitle, content.InnerHtml.Trim(), createdOn, "NoImage", url, originalSourceId);
        }
    }
}
