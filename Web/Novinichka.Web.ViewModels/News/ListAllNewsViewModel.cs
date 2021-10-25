using System.Collections.Generic;

namespace Novinichka.Web.ViewModels.News
{
    public class ListAllNewsViewModel : PagingViewModel
    {
        public IEnumerable<GetNewsViewModel> News { get; set; }
    }
}
