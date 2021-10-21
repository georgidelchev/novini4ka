using System.Collections.Generic;

namespace Novinichka.Services.Data.Interfaces
{
    public interface ISettingsService
    {
        int GetCount();

        IEnumerable<T> GetAll<T>();
    }
}
