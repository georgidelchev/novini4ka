using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using AngleSharp.Dom;
using AngleSharp.Html.Dom;
using AngleSharp.Html.Parser;
using Novinichka.Common;

namespace Novinichka.Services.NewsSources;

public abstract class BaseSource : ISource
{
    public virtual string BaseUrl { get; set; }

    protected HtmlParser Parser { get; set; } = new();

    public virtual IEnumerable<NewsModel> GetAllNews()
        => new List<NewsModel>();

    public abstract IEnumerable<NewsModel> GetRecentNews();

    public IEnumerable<NewsModel> GetAllNewsUrls(string url, string anchorTagSelector, string urlShouldContain = "", int count = 0, bool throwIfNoUrls = true)
    {
        var webClient = new WebClient();

        var html = webClient.DownloadString($"{this.BaseUrl}{url}");
        var document = this.Parser.ParseDocument(html);

        var links = document
            .QuerySelectorAll(anchorTagSelector)
            .Where(l => !string.IsNullOrWhiteSpace(l.Attributes["href"]?.Value))
            .Select(l => this.RebuildGivenUrl(l?.Attributes["href"]?.Value))
            .Where(l => l?.Contains(urlShouldContain) == true)
            .Distinct();

        if (count > 0)
        {
            links = links.Take(count);
        }

        if (!links.Any() && throwIfNoUrls)
        {
            throw new Exception("No news has been found.");
        }

        return links
            .Select(this.GetNews)
            .Where(x => x != null);
    }

    public virtual string GetOriginalIdFromSourceUrl(string url)
    {
        var trimmedUrl = url.Trim().Trim('/');
        var uri = new Uri(trimmedUrl);
        var lastSegment = uri.Segments[^1];

        return WebUtility.UrlDecode(lastSegment);
    }

    protected abstract NewsModel ParseDocument(IDocument document, string url);

    protected NewsModel GetNews(string url)
    {
        var webClient = new WebClient();
        webClient.Headers.Add(HttpRequestHeader.UserAgent, GlobalConstants.UserAgentValue);

        IHtmlDocument document;

        try
        {
            var html = webClient.DownloadString(url);
            document = this.Parser.ParseDocument(html);
        }
        catch (WebException ex)
        {
            if (ex.Status == WebExceptionStatus.ProtocolError)
            {
                switch ((ex.Response as HttpWebResponse)?.StatusCode)
                {
                    case HttpStatusCode.NotFound:
                    case HttpStatusCode.InternalServerError:
                        return null;
                }
            }

            throw;
        }

        var news = this.ParseDocument(document, url);
        if (news == null)
        {
            return null;
        }

        news.Title = news.Title?.Trim().Replace("  ", " ");
        news.Content = news.Content?.Trim();

        if (news.CreatedOn > DateTime.Now)
        {
            news.CreatedOn = DateTime.Now;
        }

        if (news.CreatedOn.Date == DateTime.UtcNow.Date &&
            news.CreatedOn.Hour == 0 &&
            news.CreatedOn.Minute == 0)
        {
            news.CreatedOn = DateTime.Now;
        }

        news.OriginalUrl = url.Trim();

        if (news.ImageUrl != null)
        {
            news.ImageUrl = news.ImageUrl.Trim();
            news.ImageUrl = this.RebuildGivenUrl(news.ImageUrl)?.Trim();
        }

        news.OriginalSourceId = news.OriginalSourceId.Trim();

        return news;
    }

    private string RebuildGivenUrl(string url)
    {
        if (string.IsNullOrWhiteSpace(url))
        {
            return null;
        }

        if (!Uri.TryCreate(new Uri(this.BaseUrl), url, out var result))
        {
            return url;
        }

        return result.ToString();
    }
}
