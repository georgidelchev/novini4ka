using System.Linq;
using System.Threading.Tasks;

using Novinichka.Data.Common.Repositories;
using Novinichka.Data.Models;
using Novinichka.Services.Data.Interfaces;

namespace Novinichka.Services.Data.Implementations
{
    public class NewsService : INewsService
    {
        private readonly IDeletableEntityRepository<News> newsRepository;

        public NewsService(IDeletableEntityRepository<News> newsRepository)
        {
            this.newsRepository = newsRepository;
        }

        public async Task<int?> AddAsync(NewsModel model, int sourceId)
        {
            if (this.newsRepository
                .AllWithDeleted().Any(x => x.SourceId == sourceId && x.OriginalSourceId == model.OriginalSourceId))
            {
                return null;
            }

            var news = new News()
            {
                Title = model.Title,
                Content = model.Content,
                CreatedOn = model.CreatedOn,
                ImageUrl = model.ImageUrl,
                OriginalUrl = model.OriginalUrl,
                SourceId = sourceId,
                OriginalSourceId = model.OriginalSourceId,
            };

            await this.newsRepository.AddAsync(news);
            await this.newsRepository.SaveChangesAsync();

            return news.Id;
        }
    }
}
