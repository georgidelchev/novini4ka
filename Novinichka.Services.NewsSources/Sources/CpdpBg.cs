using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net;
using AngleSharp.Dom;
using Novinichka.Services.Helpers;

namespace Novinichka.Services.NewsSources.Sources;

public class CpdpBg : BaseSource
{
    private const string Url = "index.php";
    private const string UrlPaginationFragment = "?p=news_view&aid=";

    private const int CountOfNews = 5;
    private const int StartNews = 1;
    private const int EndNews = 2000;

    public override string BaseUrl { get; set; } = "https://www.cpdp.bg/";

    public override IEnumerable<NewsModel> GetRecentNews()
        => GetNewsUrls($"{this.BaseUrl}{Url}{UrlPaginationFragment}")
            .Select(this.GetNews)
            .Where(x => x != null)
            .Take(CountOfNews)
            .ToList();

    public override IEnumerable<NewsModel> GetAllNews()
    {
        for (var i = StartNews; i <= EndNews; i++)
        {
            var counter = 1;
            NewsModel news;

            try
            {
                news = this.GetNews($"{BaseUrl}{Url}{UrlPaginationFragment}{i}");
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                continue;
            }

            if (news is not null)
            {
                Console.WriteLine($"{counter++} => {news.OriginalUrl}");
                yield return news;
            }
        }
    }

    private IEnumerable<string> GetNewsUrls(string url)
    {
        var webClient = new WebClient();

        var html = webClient.DownloadString(url);

        var document = this.Parser
            .ParseDocument(html);
        var a = document
            .QuerySelectorAll("div.news-content h6 a")
            .Select(x => $"{this.BaseUrl}{x?.Attributes["href"]?.Value}")
            .ToList();

        return document
            .QuerySelectorAll("div.news-content h6 a")
            .Select(x => $"{this.BaseUrl}{x?.Attributes["href"]?.Value}")
            .ToList();
    }

    protected override NewsModel ParseDocument(IDocument document, string url)
    {
        var newsTitle = document
            .QuerySelector("div.center-part h1")
            ?.InnerHtml
            .Trim();

        if (string.IsNullOrWhiteSpace(newsTitle) || newsTitle == "Новини")
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

        var createdOn = DateTime.ParseExact(createdOnAsString, "dd.MM.yyyy", CultureInfo.InvariantCulture);

        var content = document.QuerySelector(".center-part");

        var imageElement = document?.QuerySelector("div.titleImage img");

        var originalSourceId = newsTitle.Replace(' ', '-').ToLower();

        content.RemoveChildNodes(imageElement);
        content.RemoveGivenTag("img");
        content.RemoveElement("div.path");
        content.RemoveElement("h1");
        content.RemoveElement("div.date");

        string image = null;
        if (imageElement is not null)
        {
            image = this.BaseUrl.Remove(this.BaseUrl.Length - 1, 1) + imageElement?.GetAttribute("src");
        }

        return new NewsModel(newsTitle, content?.InnerHtml.Trim(), createdOn, image, url, originalSourceId);
    }
}
