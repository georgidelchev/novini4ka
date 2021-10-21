using System.Threading.Tasks;

using Novinichka.Web.ViewModels.Administration.Sources;

namespace Novinichka.Services.Data.Interfaces
{
    public interface ISourcesService
    {
        Task CreateAsync(CreateSourceInputModel inputModel);

        bool IsExisting(string typeName);
    }
}
