using System;
using System.Globalization;

using Novinichka.Services.Mapping;

namespace Novinichka.Web.ViewModels.News;

public class NewsViewModel : IMapFrom<Data.Models.News>
{
    public int Id { get; set; }

    public string Title { get; set; }

    public string Content { get; set; }

    public string SourceShortName { get; set; }

    public string SourceName { get; set; }

    public string SourceUrl { get; set; }

    public string OriginalUrl { get; set; }

    public DateTime CreatedOn { get; set; }

    public string CreatedOnAsString
        => this.CreatedOn.Hour == 0 && this.CreatedOn.Minute == 0
            ? this.CreatedOn.ToString("ddd, dd MMM yyyy", new CultureInfo("bg-BG"))
            : this.CreatedOn.ToString("ddd, dd MMM yyyy HH:mm", new CultureInfo("bg-BG"));

    public string ImageUrl { get; set; }
}
