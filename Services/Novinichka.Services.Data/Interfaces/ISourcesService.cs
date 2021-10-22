using System.Collections.Generic;
using System.Threading.Tasks;

using Novinichka.Web.ViewModels.Administration.Sources;

namespace Novinichka.Services.Data.Interfaces
{
    public interface ISourcesService
    {
        Task CreateAsync(CreateSourceInputModel inputModel);

        Task<IEnumerable<T>> GetAll<T>();

        bool IsExisting(string typeName);
    }
}
