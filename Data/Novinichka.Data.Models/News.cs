using System.ComponentModel.DataAnnotations;

using Novinichka.Data.Common.Models;

namespace Novinichka.Data.Models
{
    public class News : BaseDeletableModel<int>
    {
        [Required]
        public string Title { get; set; }

        [Required]
        public string Content { get; set; }

        [Required]
        public string Source { get; set; }

        [Required]
        public string SourceUrl { get; set; }

        [Required]
        public string OriginalUrl { get; set; }

        [Required]
        public string ImageUrl { get; set; }
    }
}
