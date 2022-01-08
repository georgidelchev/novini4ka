using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using AngleSharp.Dom;
using Novinichka.Services.Helpers;

namespace Novinichka.Services.NewsSources.Sources
{
    public class BivolaBg : BaseSource
    {
        private const string UrlPaginationFragment = "page/";
        private const string UrlShouldContain = "bivol.bg";
        private const string AnchorTagSelector = "h2.entry-title a";

        private const int CountOfNews = 11;
        private const int StartPage = 1;
        private const int EndPage = 300;

        public override string BaseUrl { get; set; } = "https://bivol.bg/";

        public override IEnumerable<NewsModel> GetRecentNews()
            => this.GetAllNewsUrls(UrlPaginationFragment + "/1", AnchorTagSelector, UrlShouldContain, CountOfNews);

        public override IEnumerable<NewsModel> GetAllNews()
        {
            var counter = 1;
            for (var i = StartPage; i <= EndPage; i++)
            {
                IEnumerable<NewsModel> news;

                try
                {
                    news = this.GetAllNewsUrls(UrlPaginationFragment + i, AnchorTagSelector);
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
                .QuerySelectorAll(".header-standard h1.post-title")
                .LastOrDefault()
                ?.TextContent
                .Trim();

            if (string.IsNullOrWhiteSpace(newsTitle))
            {
                return null;
            }

            var createdOnAsString = document
                .QuerySelector("time")
                ?.TextContent
                .Trim();

            var createdOn = DateTime.Parse(createdOnAsString, new CultureInfo("bg-BG"));

            var content = document
                .QuerySelector("div[itemprop=\"articleBody\"]");

            var originalSourceId = this.GetOriginalIdFromSourceUrl(url);

            var image = document
                ?.QuerySelector("div.post-image a");

            content.RemoveGivenTag("script");
            content.RemoveGivenTag("style");
            content.RemoveGivenTag("figure");
            content.RemoveGivenTag("img");
            content.RemoveElement("ul.sigFreeContainer");
            content.RemoveChildNodes(document.QuerySelector("div[itemprop=articleBody] div[role=tabpanel]"));
            content.RemoveChildNodes(document.QuerySelector("div[itemprop=articleBody] div.abh_box_business"));
            content.RemoveChildNodes(document.QuerySelector("div[itemprop=articleBody] form"));
            content.RemoveChildNodes(document.QuerySelector("div[itemprop=articleBody] .dkpdf-button-container"));
            content.RemoveChildNodes(document.QuerySelector("div[itemprop=articleBody] div:has(figure.wp-block-pullquote)"));
            content.RemoveChildNodes(document.QuerySelector("div[itemprop=articleBody] div:has(figure.wp-block-pullquote)"));
            content.RemoveChildNodes(document.QuerySelector("div[itemprop=articleBody] div:has(div.simpay-form-wrap)"));
            content.RemoveChildNodes(document.QuerySelector("div[itemprop=articleBody] figure.wp-block-pullquote"));
            content.RemoveChildNodes(document.QuerySelector("div[itemprop=articleBody] figure.wp-block-pullquote"));
            content.RemoveChildNodes(document.QuerySelector("div[itemprop=articleBody] div:has(script)"));
            content.RemoveChildNodes(document.QuerySelector("div.sharedaddy"));
            content.RemoveChildNodes(document.QuerySelector("div.sharedaddy"));
            content.RemoveChildNodes(document.QuerySelector("p.wpml-ls-statics-post_translations"));

            return new NewsModel(newsTitle, content?.InnerHtml.Trim(), createdOn, image?.GetAttribute("href"), url, originalSourceId);
        }
    }
}