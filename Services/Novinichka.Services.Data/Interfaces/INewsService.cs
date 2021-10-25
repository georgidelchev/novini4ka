using System.Collections.Generic;
using System.Threading.Tasks;

namespace Novinichka.Services.Data.Interfaces
{
    public interface INewsService
    {
        Task<int?> AddAsync(NewsModel model, int sourceId);

        Task<IEnumerable<T>> GetAll<T>();
    }
}
