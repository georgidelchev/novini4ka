using System;
using System.Collections.Generic;
using System.Globalization;
using AngleSharp.Dom;
using Novinichka.Services.Helpers;

namespace Novinichka.Services.NewsSources.Sources;

public class ApiBg : BaseSource
{
    private const string Url = "bg/novini";
    private const string UrlPaginationFragment = "?&p=";
    private const string UrlShouldContain = "/bg/novini";
    private const string AnchorTagSelector = ".news-panel a";

    private const int CountOfNews = 5;
    private const int StartPage = 0;
    private const int EndPage = 10000;
    private const int PaginationSkipCount = 20;

    public override string BaseUrl { get; set; } = "http://www.api.bg/";

    public override IEnumerable<NewsModel> GetRecentNews()
        => this.GetAllNewsUrls(Url, AnchorTagSelector, UrlShouldContain, CountOfNews);

    public override IEnumerable<NewsModel> GetAllNews()
    {
        var counter = 1;
        IEnumerable<NewsModel> news;

        for (var i = StartPage; i <= EndPage; i += PaginationSkipCount)
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
        var newsTitleElement = document.QuerySelector("section#single-news h1");
        var contentElement = document.QuerySelector("section#single-news");
        var imageElement = document.QuerySelector("section#single-news a img.img-thumbnail");
        var createdOnElement = document.QuerySelector("section#single-news div.date");

        contentElement?.RemoveChildNodes(imageElement);
        contentElement?.RemoveChildNodes(newsTitleElement);
        contentElement?.RemoveChildNodes(createdOnElement);

        contentElement?.RemoveGivenTag("script");

        var newsTitle = newsTitleElement?.InnerHtml.Trim();
        var content = contentElement?.InnerHtml.Trim();
        var image = imageElement?.GetAttribute("src");
        var createdOn = createdOnElement != null ? DateTime.ParseExact(createdOnElement?.TextContent, "dd.MM.yyyy HH:mm", CultureInfo.InvariantCulture) : DateTime.UtcNow;

        if (newsTitle == null || content == null)
        {
            return null;
        }

        return new NewsModel(newsTitle, content, createdOn, image, url, this.GetOriginalIdFromSourceUrl(url));
    }
}
