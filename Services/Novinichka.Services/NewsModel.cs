using System;

namespace Novinichka.Services
{
    public class NewsModel
    {
        public NewsModel(string title, string content, DateTime createdOn, string imageUrl, string originalUrl, string originalSourceId)
        {
            this.Title = title;
            this.Content = content;
            this.CreatedOn = createdOn;
            this.ImageUrl = imageUrl;
            this.OriginalUrl = originalUrl;
            this.OriginalSourceId = originalSourceId;
        }

        public string Title { get; set; }

        public string Content { get; set; }

        public string ImageUrl { get; set; }

        public string OriginalUrl { get; set; }

        public DateTime CreatedOn { get; set; }

        public string OriginalSourceId { get; set; }
    }
}
