using System;
using System.Collections.Generic;
using System.Globalization;
using AngleSharp.Dom;
using Novinichka.Services.Helpers;

namespace Novinichka.Services.NewsSources.Sources
{
    public class ApiBg : BaseSource
    {
        private const string Url = "index.php/bg/prescentar/novini";
        private const string UrlPaginationFragment = "?ccm_paging_p_b606=";
        private const string UrlShouldContain = "bg/prescentar/novini";
        private const string AnchorTagSelector = ".ccm-page-list .news-item a.news_more_link";

        private const int CountOfNews = 5;
        private const int StartPage = 1;
        private const int EndPage = 1100;

        public override string BaseUrl { get; set; } = "http://www.api.bg/";

        public override IEnumerable<NewsModel> GetRecentNews()
            => this.GetAllNewsUrls(Url, AnchorTagSelector, UrlShouldContain, CountOfNews);

        public override IEnumerable<NewsModel> GetAllNews()
        {
            var counter = 1;
            IEnumerable<NewsModel> news;

            for (var i = StartPage; i <= EndPage; i++)
            {
                try
                {
                    news = this.GetAllNewsUrls(Url + UrlPaginationFragment + i, AnchorTagSelector, UrlShouldContain);
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                    continue;
                }

                foreach (var element in news)
                {
                    Console.WriteLine($"{counter++} => {element.OriginalUrl}");
                    yield return element;
                }
            }
        }

        protected override NewsModel ParseDocument(IDocument document, string url)
        {
            var newsTitle = document
                .QuerySelector(".box h1")
                ?.InnerHtml
                .Trim();

            var createdOnAsString = document
                .QuerySelector(".news-article")
                ?.ChildNodes[0];

            var createdOn = createdOnAsString == null ? 
                DateTime.UtcNow : 
                DateTime.ParseExact(createdOnAsString?.TextContent, "dd.MM.yyyy HH:mm", CultureInfo.InvariantCulture);

            var content = document.QuerySelector(".news-article");

            var originalSourceId = this.GetOriginalIdFromSourceUrl(url);

            var image = this.BaseUrl.Remove(this.BaseUrl.Length - 1, 1) + document?
                .QuerySelector(".news-article img")
                ?.GetAttribute("src");

            content.RemoveGivenTag("img");
            content.RemoveGivenTag("script");
            content?.RemoveChild(createdOnAsString);

            return new NewsModel(newsTitle, content?.InnerHtml, createdOn, image, url, originalSourceId);
        }
    }
}
