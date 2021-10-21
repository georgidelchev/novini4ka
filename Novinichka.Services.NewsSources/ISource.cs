using System.Collections.Generic;

namespace Novinichka.Services.NewsSources
{
    public interface ISource
    {
        string BaseUrl { get; set; }

        IEnumerable<NewsModel> GetAllNewsUrls(string url, string anchorTagSelector, string urlShouldContain = "", int count = 0, bool throwIfNoUrls = true);

        public IEnumerable<NewsModel> GetRecentNews();

        public IEnumerable<NewsModel> GetAllNews();

        string GetOriginalIdFromSourceUrl(string url);
    }
}