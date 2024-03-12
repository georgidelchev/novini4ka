using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Text;
using AngleSharp.Dom;

namespace Novinichka.Services.NewsSources.Sources;

public class VksBg : BaseSource
{
    private const string Url = "novini.html";
    private const string AnchorTagSelector = "div#Content p a";

    private const int CountOfNews = 5;

    public override string BaseUrl { get; set; } = "http://www.vks.bg/";

    public override IEnumerable<NewsModel> GetRecentNews()
        => GetNewsUrls($"{this.BaseUrl}{Url}")
            .Select(this.GetNews)
            .Where(x => x != null)
            .Take(CountOfNews)
            .ToList();

    public override IEnumerable<NewsModel> GetAllNews()
    {
        var counter = 1;
        IEnumerable<NewsModel> news = null;

        try
        {
            news = GetNewsUrls($"{this.BaseUrl}{Url}")
                .Select(this.GetNews)
                .Where(x => x != null)
                .ToList();
        }
        catch (Exception e)
        {
            Console.WriteLine(e.Message);
        }

        foreach (var element in news)
        {
            Console.WriteLine($"{counter++} => {element.OriginalUrl}");
            yield return element;
        }
    }

    private IEnumerable<string> GetNewsUrls(string url)
    {
        var webClient = new WebClient();

        var html = webClient.DownloadString(url);

        var document = this.Parser
            .ParseDocument(html);

        return document
            .QuerySelectorAll(AnchorTagSelector)
            .Select(x => $"{this.BaseUrl}{x?.Attributes["href"]?.Value.Substring(1, x.Attributes["href"].Value.Length - 1)}")
            .ToList();
    }

    protected override NewsModel ParseDocument(IDocument document, string url)
    {
        var newsTitle = document
            .QuerySelector("div#Content div h2")
            ?.InnerHtml
            .Trim();

        if (string.IsNullOrWhiteSpace(newsTitle))
        {
            return null;
        }

        var createdOnAsString = document
            .QuerySelector("time")
            ?.TextContent;

        if (createdOnAsString is null)
        {
            return null;
        }

        var createdOn = DateTime.ParseExact(createdOnAsString, "dd.MM.yyyy", CultureInfo.InvariantCulture);

        var content = document.QuerySelectorAll("div#Content div p");

        var sb = new StringBuilder();
        foreach (var element in content)
        {
            sb.AppendLine(element.OuterHtml);
        }

        var originalSourceId = this.GetOriginalIdFromSourceUrl(url);

        return new NewsModel(newsTitle, sb.ToString(), createdOn, null, url, originalSourceId);
    }

    public override string GetOriginalIdFromSourceUrl(string url)
    {
        var uri = new Uri(url.Trim().Trim('/'));
        var lastSegment = uri.Segments[^1].Substring(0, uri.Segments[^1].IndexOf('.'));

        return WebUtility.UrlDecode(lastSegment);
    }
}