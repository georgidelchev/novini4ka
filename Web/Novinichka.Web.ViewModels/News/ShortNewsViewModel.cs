using System.Net;
using System.Text.RegularExpressions;

using Ganss.XSS;

namespace Novinichka.Web.ViewModels.News
{
    public class ShortNewsViewModel : NewsViewModel
    {
        public string ShortContent
            => this.GetShortContent(230);

        private string GetShortContent(int maxLength)
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
