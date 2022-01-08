using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using Novinichka.Web.ViewModels.Administration.Sources;

namespace Novinichka.Services.Data.Interfaces
{
    public interface ISourcesService
    {
        Task CreateAsync(CreateSourceInputModel inputModel);

        Task<IEnumerable<T>> GetAllWithDeleted<T>();

        Task Delete(int sourceId);

        bool IsExisting(string typeName);

        bool IsExisting(int id);

        string GetName(int id);

        string GetBigImageUrl(int sourceId);
    }
}
