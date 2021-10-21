using System;
using System.Collections.Generic;
using System.Globalization;
using AngleSharp.Dom;
using Novinichka.Services.Helpers;

namespace Novinichka.Services.NewsSources.Sources
{
    public class StarozagorskiNoviniCom : BaseSource
    {
        private const string Url = "stznews/category/novini/";
        private const string UrlPaginationFragment = "page/";
        private const string UrlShouldContain = "starozagorskinovini.com";
        private const string AnchorTagSelector = "h3.entry-title a";

        private const int CountOfNews = 5;
        private const int StartPage = 1;
        private const int EndPage = 1300;

        public override string BaseUrl { get; set; } = "http://starozagorskinovini.com/";

        public override IEnumerable<NewsModel> GetRecentNews()
            => this.GetAllNewsUrls(Url, AnchorTagSelector, UrlShouldContain, CountOfNews);

        public override IEnumerable<NewsModel> GetAllNews()
        {
            for (var i = StartPage; i <= EndPage; i++)
            {
                var news = this.GetAllNewsUrls(Url + UrlPaginationFragment + i, AnchorTagSelector);

                foreach (var element in news)
                {
                    yield return element;
                }
            }
        }

        protected override NewsModel ParseDocument(IDocument document, string url)
        {
            var newsTitle = document
                .QuerySelector("header.td-post-title h1.entry-title")
                ?.InnerHtml
                .Trim();

            var createdOnAsString = document
                .QuerySelector("time.entry-date")
                ?.TextContent;

            var createdOn = DateTime.ParseExact(createdOnAsString, "dd.MM.yyyy", CultureInfo.InvariantCulture);

            var content = document
                .QuerySelector("div.td-post-content");

            content.RemoveElement("ul.sigFreeContainer");
            content.RemoveGivenTag("script");
            content.RemoveComments();

            var originalSourceId = this.GetOriginalIdFromSourceUrl(url);

            var image = document
                ?.QuerySelector("div.td-post-featured-image a");

            content.RemoveChildNodes(image);

            return new NewsModel(newsTitle, content?.InnerHtml, createdOn, image?.GetAttribute("href"), url, originalSourceId);
        }
    }
}