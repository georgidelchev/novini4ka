using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Hangfire;
using Microsoft.EntityFrameworkCore;
using Novinichka.Data.Common.Repositories;
using Novinichka.Data.Models;
using Novinichka.Services.Data.Interfaces;
using Novinichka.Services.Mapping;
using Novinichka.Web.ViewModels.Administration.Sources;

namespace Novinichka.Services.Data.Implementations;

public class SourcesService : ISourcesService
{
    private readonly IDeletableEntityRepository<Source> sourcesRepository;
    private readonly ICloudinaryService cloudinaryService;
    private readonly IRecurringJobManager recurringJobManager;

    public SourcesService(
        IDeletableEntityRepository<Source> sourcesRepository,
        ICloudinaryService cloudinaryService,
        IRecurringJobManager recurringJobManager)
    {
        this.sourcesRepository = sourcesRepository;
        this.cloudinaryService = cloudinaryService;
        this.recurringJobManager = recurringJobManager;
    }

    public async Task CreateAsync(CreateSourceInputModel inputModel)
    {
        // await using var ms = new MemoryStream();
        // await inputModel.DefaultImage.CopyToAsync(ms);
        // var destinationData = ms.ToArray();
        // var smallBannerUrl = await this.cloudinaryService
        //     .UploadPictureAsync(destinationData, $"{inputModel.ShortName}_Small", "SourcesImages", 100, 80, "scale");
        // var bigBannerUrl = await this.cloudinaryService
        //     .UploadPictureAsync(destinationData, $"{inputModel.ShortName}_Big", "SourcesImages", 730, 500, "fit");
        // var source = new Source
        // {
        //     SmallBannerUrl = smallBannerUrl,
        //     BigBannerUrl = bigBannerUrl,
        //     Description = inputModel.Description,
        //     Name = inputModel.FullName,
        //     ShortName = inputModel.ShortName,
        //     Url = inputModel.Url,
        //     TypeName = inputModel.TypeName,
        // };
        // await this.sourcesRepository.AddAsync(source);
        // await this.sourcesRepository.SaveChangesAsync();
    }

    public async Task<IEnumerable<T>> GetAllWithDeleted<T>()
        => await this.sourcesRepository
            .AllWithDeleted()
            .To<T>()
            .ToListAsync();

    public async Task Delete(int sourceId)
    {
        var source = this.sourcesRepository
            .All()
            .FirstOrDefault(s => s.Id == sourceId);

        if (source != null)
        {
            source.IsDeleted = true;
            source.DeletedOn = DateTime.UtcNow;

            await this.sourcesRepository.SaveChangesAsync();

            this.recurringJobManager.RemoveIfExists($"GetRecentNewsCJ-{source.ShortName}");
        }
    }

    public bool IsExisting(string typeName)
        => this.sourcesRepository
            .AllWithDeleted()
            .Any(s => s.TypeName == typeName);

    public bool IsExisting(int id)
        => this.sourcesRepository
            .AllWithDeleted()
            .Any(s => s.Id == id);

    public string GetName(int id)
        => this.sourcesRepository
            .All()
            .Where(s => s.Id == id)
            .Select(s => s.ShortName + " " + s.Name)
            .FirstOrDefault();

    public string GetBigImageUrl(int sourceId) => null;

    // => this.sourcesRepository
    //     .AllWithDeleted()
    //     .FirstOrDefault(s => s.Id == sourceId)
    //     ?.BigBannerUrl;
}
