using System.Threading.Tasks;

using Microsoft.AspNetCore.Mvc;
using Novinichka.Services.Data.Interfaces;
using Novinichka.Web.ViewModels.News;

namespace Novinichka.Web.Controllers
{
    public class NewsController : BaseController
    {
        private readonly INewsService newsService;

        public NewsController(INewsService newsService)
        {
            this.newsService = newsService;
        }

        [HttpGet]
        public async Task<IActionResult> Details(int id)
        {
            if (!this.newsService.IsExisting(id))
            {
                this.TempData["ErrorMessage"] = "This news is not existing!";
            }

            var viewModel = await this.newsService.GetDetails<NewsViewModel>(id);
            return this.View(viewModel);
        }
    }
}
