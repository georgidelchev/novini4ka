﻿using System.Collections.Generic;
using System.Threading.Tasks;

namespace Novinichka.Services.Data.Interfaces;

public interface INewsService
{
    Task<int?> AddAsync(NewsModel model, int sourceId);

    Task<IEnumerable<T>> GetAll<T>();

    Task<T> GetDetails<T>(int newsId);

    bool IsExisting(int sourceId, string originalSourceId);

    bool IsExisting(string originalSourceId, string originalSourceUrl);
}
