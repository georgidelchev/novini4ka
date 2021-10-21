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
        public int SourceId { get; set; }

        public virtual Source Source { get; set; }

        [Required]
        public string OriginalUrl { get; set; }

        [Required]
        public string OriginalSourceId { get; set; }

        [Required]
        public string ImageUrl { get; set; }
    }
}
