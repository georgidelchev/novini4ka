using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Novinichka.Services.Data.Interfaces;
using Novinichka.Web.ViewModels;
using Novinichka.Web.ViewModels.News;

namespace Novinichka.Web.Controllers
{
    public class HomeController : BaseController
    {
        private readonly INewsService newsService;

        public HomeController(
            INewsService newsService)
        {
            this.newsService = newsService;
        }

        public async Task<IActionResult> Index(int id = 1)
        {
            var news = await this.newsService.GetAll<GetNewsViewModel>();

            var viewModel = new ListAllNewsViewModel()
            {
                ItemsPerPage = 12,
                News = news.Skip((id - 1) * 12).Take(12),
                PageNumber = id,
                ItemsCount = news.Count(),
            };

            return this.View(viewModel);
        }

        public IActionResult Privacy()
        {
            return this.View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return this.View(
                new ErrorViewModel { RequestId = Activity.Current?.Id ?? this.HttpContext.TraceIdentifier });
        }
    }
}
