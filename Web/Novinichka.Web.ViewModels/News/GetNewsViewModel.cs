using System;
using System.Net;
using System.Text.RegularExpressions;

using Ganss.XSS;
using Novinichka.Services.Mapping;

namespace Novinichka.Web.ViewModels.News
{
    public class GetNewsViewModel : IMapFrom<Data.Models.News>
    {
        public string Title { get; set; }

        public string Content { get; set; }

        public string ShortContent
            => this.GetShortContent(230);

        public DateTime CreatedOn { get; set; }

        public string SourceName { get; set; }

        public string SourceShortName { get; set; }

        public string OriginalUrl { get; set; }

        public string ImageUrl { get; set; }

        public string GetShortContent(int maxLength)
        {
            var htmlSanitizer = new HtmlSanitizer();
            var html = htmlSanitizer.Sanitize(this.Content);
            var strippedContent = WebUtility.HtmlDecode(Regex.Replace(html, "<.*?>", string.Empty));

            strippedContent = strippedContent.Replace("\n", " ");
            strippedContent = strippedContent.Replace("\t", " ");
            strippedContent = Regex.Replace(strippedContent, @"\s+", " ").Trim();

            return strippedContent.Length <= maxLength ? strippedContent : strippedContent.Substring(0, maxLength) + "...";
        }
    }
}
