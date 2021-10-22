using System;
using System.IO;
using System.Threading.Tasks;

using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using Novinichka.Services.Data.Interfaces;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats.Png;
using SixLabors.ImageSharp.Processing;

namespace Novinichka.Services.Data.Implementations
{
    public class CloudinaryService : ICloudinaryService
    {
        private readonly Cloudinary cloudinaryUtility;

        public CloudinaryService(
            Cloudinary cloudinaryUtility)
        {
            this.cloudinaryUtility = cloudinaryUtility;
        }

        public async Task<string> UploadPictureAsync(byte[] data, string fileName, string folderName, int? width, int? height, string cropType)
        {
            await using var ms = new MemoryStream(data);

            var transformation = new Transformation()
                .Height(height)
                .Width(width)
                .Crop(cropType);

            var uploadParams = UploadImageParams(fileName, folderName, ms, transformation);

            UploadResult uploadResult = this.cloudinaryUtility.Upload(uploadParams);

            if (uploadResult.Error is not null)
            {
                throw new InvalidOperationException("Error occurred.");
            }

            return uploadResult?.SecureUri.AbsoluteUri;
        }

        private static ImageUploadParams UploadImageParams(string fileName, string folderName, MemoryStream ms, Transformation transformation)
            => new ImageUploadParams
            {
                Folder = folderName,
                File = new FileDescription(fileName, ms),
                AllowedFormats = new[] { "jpg", "png", "jfif", "exif", "gif", "bmp", "ppm", "pgm", "pbm", "pnm", "heif", "bat" },
                Format = "jpg",
                Overwrite = true,
                Transformation = transformation,
            };
    }
}
