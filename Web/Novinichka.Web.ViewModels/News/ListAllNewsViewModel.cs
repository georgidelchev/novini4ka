using System.Collections.Generic;

namespace Novinichka.Web.ViewModels.News;

public class ListAllNewsViewModel : PagingViewModel
{
    public IEnumerable<ShortNewsViewModel> News { get; set; }
}
