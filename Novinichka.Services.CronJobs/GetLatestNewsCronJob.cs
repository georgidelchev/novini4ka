﻿using System;
using System.Linq;
using System.Threading.Tasks;
using Hangfire;
using Hangfire.Console;
using Hangfire.Server;
using Novinichka.Common;
using Novinichka.Data.Common.Repositories;
using Novinichka.Data.Models;
using Novinichka.Services.Data.Interfaces;
using Novinichka.Services.NewsSources;

namespace Novinichka.Services.CronJobs
{
    public class GetLatestNewsCronJob
    {
        private readonly IDeletableEntityRepository<Source> sourcesRepository;
        private readonly INewsService newsService;

        public GetLatestNewsCronJob(
            IDeletableEntityRepository<Source> sourcesRepository,
            INewsService newsService)
        {
            this.sourcesRepository = sourcesRepository;
            this.newsService = newsService;
        }

        [AutomaticRetry(Attempts = 3)]
        public async Task StartWorking(string typeName, PerformContext context)
        {
            var source = this.sourcesRepository
                .AllWithDeleted()
                .FirstOrDefault(x => x.TypeName == typeName);

            if (source == null)
            {
                throw new Exception("Source type not found in the database");
            }

            var instance = ReflectionHelpers.GetInstance<BaseSource>(typeName);

            var news = instance
                .GetRecentNews()
                .ToList();
            var bar = context.WriteProgressBar();

            foreach (var currentNews in news.WithProgress(bar))
            {
                var newsId = await this.newsService.AddAsync(currentNews, source.Id);

                if (newsId.HasValue && currentNews != null)
                {
                    context.WriteLine($"[ID:{newsId}] Successfully imported news with title: {currentNews.Title}");

                    //await this.newsService.SaveImageLocallyAsync(
                    //    remoteNews.ImageUrl,
                    //    newsId.Value,
                    //    this.webHostEnvironment.WebRootPath,
                    //    instance.UseProxy);
                }
            }
        }
    }
}
