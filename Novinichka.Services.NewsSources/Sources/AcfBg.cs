using System;
using System.Collections.Generic;
using System.Globalization;
using AngleSharp.Dom;
using Novinichka.Services.Helpers;

namespace Novinichka.Services.NewsSources.Sources
{
    public class AcfBg : BaseSource
    {
        private const string Url = "bg/pres-syobshteniya/";
        private const string UrlPaginationFragment = "page/";
        private const string UrlShouldContain = "acf.bg/bg/";
        private const string AnchorTagSelector = "div.padding-bottom-80 div a.white-red-btn";

        private const int CountOfNews = 5;
        private const int StartPage = 1;
        private const int EndPage = 100;

        public override string BaseUrl { get; set; } = "https://acf.bg/";

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
                .QuerySelector(".fs-28")
                ?.InnerHtml
                .Trim();

            if (string.IsNullOrWhiteSpace(newsTitle))
            {
                return null;
            }

            var createdOnAsString = document
                .QuerySelector("time")
                ?.TextContent
                .Trim();

            var createdOn = DateTime.ParseExact(createdOnAsString, "dd.MM.yyyy", CultureInfo.InvariantCulture);

            var content = document.QuerySelector("div .line-height-26");

            var originalSourceId = this.GetOriginalIdFromSourceUrl(url);

            var image = document?.QuerySelector("figure.padding-bottom-30 img");

            content.RemoveGivenTag("img");
            content.RemoveGivenTag("script");
            content.RemoveGivenTag("form");
            content.RemoveElement("div .awac-wrapper");

            return new NewsModel(newsTitle, content?.InnerHtml, createdOn, image?.GetAttribute("src"), url, originalSourceId);
        }
    }
}
