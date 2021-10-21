﻿using System.IO;
using System.Linq;
using System.Threading.Tasks;

using Novinichka.Data.Common.Repositories;
using Novinichka.Data.Models;
using Novinichka.Services.Data.Interfaces;
using Novinichka.Web.ViewModels.Administration.Sources;

namespace Novinichka.Services.Data.Implementations
{
    public class SourcesService : ISourcesService
    {
        private readonly IDeletableEntityRepository<Source> sourcesRepository;
        private readonly ICloudinaryService cloudinaryService;

        public SourcesService(
            IDeletableEntityRepository<Source> sourcesRepository,
            ICloudinaryService cloudinaryService)
        {
            this.sourcesRepository = sourcesRepository;
            this.cloudinaryService = cloudinaryService;
        }

        public async Task CreateAsync(CreateSourceInputModel inputModel)
        {
            await using var ms = new MemoryStream();

            await inputModel.DefaultImage.CopyToAsync(ms);
            var destinationData = ms.ToArray();

            var defaultImage = await this.cloudinaryService
                .UploadPictureAsync(destinationData, $"{inputModel.ShortName}", "SourcesDefaultImages", 100, 80, "scale");

            //var bigBannerUrl = await this.cloudinaryService
            //    .UploadPictureAsync(destinationData, $"{inputModel.ShortName}_Big", "SourcesBanners", 730, 500, "fit");

            var source = new Source
            {
                DefaultImageUrl = defaultImage,
                Description = inputModel.Description,
                Name = inputModel.FullName,
                ShortName = inputModel.ShortName,
                Url = inputModel.Url,
                TypeName = inputModel.TypeName,
            };

            await this.sourcesRepository.AddAsync(source);
            await this.sourcesRepository.SaveChangesAsync();
        }

        public bool IsExisting(string typeName)
            => this.sourcesRepository.AllWithDeleted().Any(s => s.TypeName == typeName);
    }
}