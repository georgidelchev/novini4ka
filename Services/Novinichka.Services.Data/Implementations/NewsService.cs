using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

using Microsoft.EntityFrameworkCore;
using Novinichka.Data.Common.Repositories;
using Novinichka.Data.Models;
using Novinichka.Services.Data.Interfaces;
using Novinichka.Services.Mapping;

namespace Novinichka.Services.Data.Implementations
{
    public class NewsService : INewsService
    {
        private readonly IDeletableEntityRepository<News> newsRepository;
        private readonly ICloudinaryService cloudinaryService;
        private readonly ISourcesService sourcesService;

        public NewsService(
            IDeletableEntityRepository<News> newsRepository,
            ICloudinaryService cloudinaryService,
            ISourcesService sourcesService)
        {
            this.newsRepository = newsRepository;
            this.cloudinaryService = cloudinaryService;
            this.sourcesService = sourcesService;
        }

        public async Task<int?> AddAsync(NewsModel model, int sourceId)
        {
            var imageUrl = this.sourcesService.GetBigImageUrl(sourceId);

            if (model.ImageUrl != null)
            {
                var imageBytes = new WebClient().DownloadData(model.ImageUrl);

                imageUrl = await this.cloudinaryService.UploadPictureAsync(imageBytes, $"{model.Title}_Big", "NewsImages", 730, 500, "fit");
            }

            var news = new News
            {
                Title = model.Title,
                Content = model.Content,
                CreatedOn = model.CreatedOn,
                OriginalUrl = model.OriginalUrl,
                SourceId = sourceId,
                OriginalSourceId = model.OriginalSourceId,
                ImageUrl = imageUrl,
            };

            await this.newsRepository.AddAsync(news);
            await this.newsRepository.SaveChangesAsync();

            return news.Id;
        }

        public async Task<IEnumerable<T>> GetAll<T>()
            => await this.newsRepository
                .AllWithDeleted()
                .Include(n => n.Source)
                .OrderByDescending(n => n.CreatedOn)
                .To<T>()
                .ToListAsync();

        public async Task<T> GetDetails<T>(int newsId)
            => await this.newsRepository
                .All()
                .Where(n => n.Id == newsId)
                .Include(n => n.Source)
                .To<T>()
                .FirstOrDefaultAsync();

        public bool IsExisting(int sourceId, string originalSourceId)
            => this.newsRepository
                .AllWithDeleted()
                .Any(n => n.SourceId == sourceId &&
                          n.OriginalSourceId == originalSourceId);

        public bool IsExisting(string originalSourceId, string originalSourceUrl)
            => this.newsRepository
                .AllWithDeleted()
                .Any(n => n.OriginalSourceId == originalSourceId && n.OriginalUrl == originalSourceUrl);
    }
}
