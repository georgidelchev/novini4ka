using System;
using System.Collections.Generic;
using System.Globalization;
using AngleSharp.Dom;
using Novinichka.Services.Helpers;

namespace Novinichka.Services.NewsSources.Sources
{
    public class BfuBg : BaseSource
    {
        private const string UrlShouldContain = "bfunion";
        private const string AnchorTagSelector = "h2.entry-title a";

        private const int CountOfNews = 5;
        private const int StartNews = 40000;
        private const int EndNews = 50000;

        private readonly string url = $"archive/{DateTime.UtcNow.Year}/{DateTime.UtcNow.Month}/0";

        public override string BaseUrl { get; set; } = "https://bfunion.bg/";

        public override IEnumerable<NewsModel> GetRecentNews()
            => this.GetAllNewsUrls($"{this.url}", AnchorTagSelector, UrlShouldContain, CountOfNews);

        public override IEnumerable<NewsModel> GetAllNews()
        {
            var counter = 1;

            for (var i = StartNews; i <= EndNews; i++)
            {
                NewsModel news;

                try
                {
                    news = this.GetNews($"{this.BaseUrl}news/{i}/0");
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                    continue;
                }

                if (news is not null &&
                    news.CreatedOn.Date != DateTime.UtcNow.Date &&
                    news.CreatedOn.Date != DateTime.UtcNow.AddDays(-1).Date)
                {
                    Console.WriteLine($"{counter++} => {news.OriginalUrl}");
                    yield return news;
                }
            }
        }

        protected override NewsModel ParseDocument(IDocument document, string url)
        {
            var newsTitle = document
                .QuerySelector("div.tr-post h2.entry-title")
                ?.TextContent
                .Trim();

            if (string.IsNullOrWhiteSpace(newsTitle))
            {
                return null;
            }

            var createdOnAsString = document
                .QuerySelector("div.date")
                ?.InnerHtml;

            if (createdOnAsString is null)
            {
                return null;
            }

            var createdOn = DateTime.Parse(createdOnAsString, new CultureInfo("bg-BG"));

            var content = document.QuerySelector("div.tr-details");

            var imageElement = document?.QuerySelector("div.entry-thumbnail img.img-responsive");

            var originalSourceId = newsTitle.Replace(' ', '-').ToLower();

            content.RemoveChildNodes(imageElement);
            content.RemoveGivenTag("img");

            string image = null;
            if (imageElement is not null)
            {
                image = this.BaseUrl.Remove(this.BaseUrl.Length - 1, 1) + imageElement?.GetAttribute("src");
            }

            return new NewsModel(newsTitle, content?.InnerHtml.Trim(), createdOn, image, url, originalSourceId);
        }
    }
}
