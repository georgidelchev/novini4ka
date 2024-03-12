using System;
using System.Collections.Generic;
using System.Globalization;
using AngleSharp.Dom;
using Novinichka.Services.Helpers;

namespace Novinichka.Services.NewsSources.Sources;

public class BfuBg : BaseSource
{
    private const string UrlShouldContain = "bfunion";
    private const string AnchorTagSelector = "h2.entry-title a";

    private const int CountOfNews = 5;
    private const int StartPage = 40000;
    private const int EndPage = 50000;

    private readonly string url = $"archive/{DateTime.UtcNow.Year}/{DateTime.UtcNow.Month}";

    public override string BaseUrl { get; set; } = "https://bfunion.bg/";

    public override IEnumerable<NewsModel> GetRecentNews()
        => this.GetAllNewsUrls($"{this.url}", AnchorTagSelector, UrlShouldContain, CountOfNews);

    public override IEnumerable<NewsModel> GetAllNews()
    {
        var counter = 1;
        NewsModel currentNews;

        for (var currentPage = StartPage; currentPage <= EndPage; currentPage++)
        {
            try
            {
                currentNews = this.GetNews($"{this.BaseUrl}news/{currentPage}/0");
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                continue;
            }

            if (currentNews is not null &&
                currentNews.CreatedOn.Date != DateTime.UtcNow.Date &&
                currentNews.CreatedOn.Date != DateTime.UtcNow.AddDays(-1).Date)
            {
                Console.WriteLine($"{counter++} => {currentNews.OriginalUrl}");
                yield return currentNews;
            }
        }
    }

    protected override NewsModel ParseDocument(IDocument document, string url)
    {
        var newsTitleElement = document?.QuerySelector("div.tr-post div.post-content h2.entry-title");
        var createdOnElement = document?.QuerySelector("div.date");
        var contentElement = document?.QuerySelector("div.tr-details");
        var imageElement = document?.QuerySelector("div.entry-thumbnail img.img-responsive");

        contentElement.RemoveChildNodes(imageElement);

        string image = null;
        if (imageElement is not null)
        {
            image = this.BaseUrl.Remove(this.BaseUrl.Length - 1, 1) + imageElement?.GetAttribute("src");
        }

        var newsTitle = newsTitleElement?.TextContent.Trim();

        DateTime? createdOn = null;
        if (createdOnElement != null)
        {
            createdOn = DateTime.Parse(createdOnElement.InnerHtml, new CultureInfo("bg-BG"));
        }

        var originalSourceId = newsTitle?.Replace(' ', '-').ToLower();
        var content = contentElement?.InnerHtml.Trim();

        if (string.IsNullOrWhiteSpace(newsTitle))
        {
            return null;
        }

        return new NewsModel(newsTitle, content, createdOn.Value, image, url, originalSourceId);
    }
}
