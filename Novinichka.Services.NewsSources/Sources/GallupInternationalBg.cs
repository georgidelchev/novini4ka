using System;
using System.Collections.Generic;
using System.Globalization;
using AngleSharp.Dom;
using Novinichka.Services.Helpers;

namespace Novinichka.Services.NewsSources.Sources
{
    public class GallupInternationalBg : BaseSource
    {
        private const string Url = "search-results/";
        private const string UrlPaginationFragment = "?_paged=";
        private const string UrlShouldContain = "gallup-international";
        private const string AnchorTagSelector = "div.glp-articles-list a.open-article1";

        private const int CountOfNews = 5;
        private const int StartPage = 1;
        private const int EndPage = 40;

        public override string BaseUrl { get; set; } = "https://www.gallup-international.bg/";

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
                    news = this.GetAllNewsUrls(Url + UrlPaginationFragment + i, AnchorTagSelector);
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
                .QuerySelector("header.entry-header h1.entry-title")
                ?.InnerHtml
                .Trim();

            if (string.IsNullOrWhiteSpace(newsTitle))
            {
                return null;
            }

            var createdOnAsString = document
                .QuerySelector("time.entry-date")
                ?.Attributes["datetime"]
                ?.Value;

            if (createdOnAsString is null)
            {
                return null;
            }

            var createdOn = DateTime.Parse(createdOnAsString, new CultureInfo("bg-BG"));

            var content = document.QuerySelector("div.entry-content");

            var originalSourceId = this.GetOriginalIdFromSourceUrl(url);

            var image = document?.QuerySelector("div.entry-content p img.alignnone");

            content.RemoveChildNodes(image);
            content.RemoveGivenTag("img");

            return new NewsModel(newsTitle, content?.InnerHtml, createdOn, image?.GetAttribute("src"), url, originalSourceId);
        }
    }
}