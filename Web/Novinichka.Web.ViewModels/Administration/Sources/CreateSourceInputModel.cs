using System.ComponentModel.DataAnnotations;

using Microsoft.AspNetCore.Http;

namespace Novinichka.Web.ViewModels.Administration.Sources
{
    public class CreateSourceInputModel
    {
        [Required]
        public string TypeName { get; set; }

        [Required]
        [MinLength(2)]
        [MaxLength(10)]
        public string ShortName { get; set; }

        [Required]
        [MinLength(2)]
        [MaxLength(100)]
        public string FullName { get; set; }

        [Required]
        [MinLength(10)]
        [MaxLength(2000)]
        public string Description { get; set; }

        [Required]
        [Url]
        public string Url { get; set; }

        [Required]
        public IFormFile DefaultImage { get; set; }
    }
}
