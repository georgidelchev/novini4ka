using System.Threading.Tasks;

namespace Novinichka.Services.Data.Interfaces
{
    public interface ICloudinaryService
    {
        Task<string> UploadPictureAsync(byte[] data, string fileName, string folderName, int? width, int? height, string cropType);
    }
}
