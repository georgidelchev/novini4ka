using Novinichka.Data.Models;
using Novinichka.Services.Mapping;

namespace Novinichka.Web.ViewModels.Administration.Sources
{
    public class GetAllSourcesViewModel : IMapFrom<Source>
    {
        public string TypeName { get; set; }

        public string ShortTypeName
            => this.TypeName.Split('.')[4];

        public string ShortName { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }

        public string ShortDescription
            => this.Description.Length >= 47 ? this.Description.Substring(0, 47) + "..." : this.Description;

        public string Url { get; set; }

        public string SmallBannerUrl { get; set; }
    }
}
