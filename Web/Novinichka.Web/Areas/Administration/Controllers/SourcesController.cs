using System.Linq;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Mvc;
using Novinichka.Services.Data.Interfaces;
using Novinichka.Web.ViewModels.Administration.Sources;

namespace Novinichka.Web.Areas.Administration.Controllers
{
    public class SourcesController : AdministrationController
    {
        private readonly ISourcesService sourcesService;

        public SourcesController(
            ISourcesService sourcesService)
        {
            this.sourcesService = sourcesService;
        }

        [HttpGet]
        public IActionResult Create()
        {
            return this.View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(CreateSourceInputModel inputModel)
        {
            if (!this.ModelState.IsValid)
            {
                return this.View(inputModel);
            }

            if (this.sourcesService.IsExisting(inputModel.TypeName))
            {
                this.ModelState.AddModelError(string.Empty, "This source is already existing!");
                return this.View(inputModel);
            }

            var typeName = $"Novinichka.Services.NewsSources.Sources.{inputModel.TypeName}";
            inputModel.TypeName = typeName;

            await this.sourcesService.CreateAsync(inputModel);

            return this.Redirect("/");
        }

        [HttpGet]
        public async Task<IActionResult> All(int id = 1)
        {
            var sources = await this.sourcesService
                .GetAllWithDeleted<GetAllSourcesViewModel>();
            var viewModel = new ListAllSourcesViewModel()
            {
                ItemsPerPage = 12,
                PageNumber = id,
                Sources = sources,
                ItemsCount = sources.Count(),
            };

            return this.View(viewModel);
        }

        [HttpGet]
        public async Task<IActionResult> Delete(int id)
        {
            if (!this.sourcesService.IsExisting(id))
            {
                this.TempData["ErrorMessage"] = "This source is not existing!";
            }
            else
            {
                var sourceName = this.sourcesService.GetName(id);
                this.TempData["SuccessMessage"] = $"Successfully deleted {sourceName} source.";
                await this.sourcesService.Delete(id);
            }

            return this.RedirectToAction(nameof(this.All));
        }
    }
}
