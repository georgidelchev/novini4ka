using System.Collections.Generic;

namespace Novinichka.Web.ViewModels.Administration.Sources
{
    public class ListAllSourcesViewModel : PagingViewModel
    {
        public IEnumerable<GetAllSourcesViewModel> Sources { get; set; }
    }
}
