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
    }
}
