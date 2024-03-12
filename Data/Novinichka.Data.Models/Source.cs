using System.Collections.Generic;

using Novinichka.Data.Common.Models;

namespace Novinichka.Data.Models;

public class Source : BaseDeletableModel<int>
{
    public string TypeName { get; set; }

    public string ShortName { get; set; }

    public string Name { get; set; }

    public string Description { get; set; }

    public string Url { get; set; }

    public string DefaultImagePath { get; set; }

    public virtual ICollection<News> News { get; set; }
        = new HashSet<News>();
}
