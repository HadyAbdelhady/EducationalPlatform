using Application.Interfaces;
using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using Domain.enums;
using Microsoft.AspNetCore.Http;

namespace Infrastructure.Services
{
    public class CloudinaryService(Cloudinary cloudinary) : ICloudinaryCore
    {
        private readonly Cloudinary _cloudinary = cloudinary;
        private readonly string _noImageUrl = "https://img.freepik.com/premium-vector/default-image-icon-vector-missing-picture-page-website-design-mobile-app-no-photo-available_87543-11093.jpg";

        public Task<Cloudinary> GetClientAsync() => Task.FromResult(_cloudinary);

        public async Task<bool> DeleteMediaAsync(IEnumerable<string> Url)
        {

            foreach (var mediaUrl in Url)
            {
                var result = await _cloudinary.DestroyAsync(new DeletionParams(mediaUrl));
                if (result.Result != "ok")
                    return false;
            }
            return true;
        }

        public async Task<bool> DeleteSingleMediaAsync(string id)
        {
            var result = await _cloudinary.DestroyAsync(new DeletionParams(id));
            if (result.Result != "ok")
                return false;

            return true;
        }
      
        public async Task<string> UploadMediaAsync(IFormFile file, UsageCategory usageCategory, string? folder = null)
        {
            if (file == null || file.Length == 0)
                throw new ArgumentException("File is required and cannot be empty", nameof(file));

            // Validate file type
            var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif", ".webp", ".bmp" };
            var fileExtension = Path.GetExtension(file.FileName).ToLowerInvariant();

            if (!allowedExtensions.Contains(fileExtension))
                throw new ArgumentException($"Invalid file type. Allowed types: {string.Join(", ", allowedExtensions)}");

            // Validate file size (max 10MB)
            const long maxFileSize = 10 * 1024 * 1024;
            if (file.Length > maxFileSize)
                throw new ArgumentException($"File size exceeds maximum allowed size of {maxFileSize / (1024 * 1024)}MB");

            using var stream = file.OpenReadStream();
            return await UploadMediaAsync(stream, file.FileName, usageCategory, folder);
        }


        public async Task<string> UploadVideoAsync(IFormFile file, string? folder = null)
        {
            if (file == null || file.Length == 0)
                throw new ArgumentException("File is required and cannot be empty", nameof(file));

            // Validate file type for video
            var allowedExtensions = new[] { ".mp4", ".mov", ".avi", ".mkv" };
            var fileExtension = Path.GetExtension(file.FileName).ToLowerInvariant();

            if (!allowedExtensions.Contains(fileExtension))
                throw new ArgumentException($"Invalid file type. Allowed types: {string.Join(", ", allowedExtensions)}");

            // Validate file size (max 1000MB for videos)
            const long maxFileSize = 1000 * 1024 * 1024;
            if (file.Length > maxFileSize)
                throw new ArgumentException($"File size exceeds maximum allowed size of {maxFileSize / (1024 * 1024)}MB");

            using var stream = file.OpenReadStream();
            var uploadParams = new VideoUploadParams
            {
                File = new FileDescription(file.FileName, stream),
                Folder = folder ?? "educational_platform/videos",
                UseFilename = true,
                UniqueFilename = true,
                Overwrite = false
            };

            var result = await _cloudinary.UploadAsync(uploadParams);

            if (result.Error != null)
                throw new Exception($"Cloudinary upload failed: {result.Error.Message}");

            return result.SecureUrl?.ToString() ?? string.Empty;
        }


        public async Task<string> EditMediaAsync(string publicId, string filePath, UsageCategory usageCategory, string? folder = null)
        {
            if (string.IsNullOrWhiteSpace(publicId))
                throw new ArgumentException("Public ID is required", nameof(publicId));

            try
            {
                // Upload new file
                var newMediaUrl = await UploadMediaAsync(filePath, usageCategory, folder);

                // Delete old file only after successful upload
                var deleted = await DeleteSingleMediaAsync(publicId);
                if (!deleted)
                {
                    // Log warning but don't fail - new file is already uploaded
                    Console.WriteLine($"Warning: Could not delete old media with ID: {publicId}");
                }

                return newMediaUrl;
            }
            catch (Exception ex)
            {
                throw new Exception($"Failed to edit media: {ex.Message}", ex);
            }
        }


        /// <summary>
        /// Edit media from IFormFile (for Flutter/mobile app uploads)
        /// </summary>
        public async Task<string> EditMediaAsync(string publicId, IFormFile file, UsageCategory usageCategory, string? folder = null)
        {
            if (string.IsNullOrWhiteSpace(publicId))
                throw new ArgumentException("Public ID is required", nameof(publicId));

            try
            {
                // Upload new file
                var newMediaUrl = await UploadMediaAsync(file, usageCategory, folder);

                // Delete old file only after successful upload
                var deleted = await DeleteSingleMediaAsync(publicId);
                if (!deleted)
                {
                    Console.WriteLine($"Warning: Could not delete old media with ID: {publicId}");
                }

                return newMediaUrl;
            }
            catch (Exception ex)
            {
                throw new Exception($"Failed to edit media: {ex.Message}", ex);
            }
        }


        /// <summary>
        /// Upload image from Stream (flexible method for various sources)
        /// </summary>
        private async Task<string> UploadMediaAsync(Stream fileStream, string fileName, UsageCategory usageCategory, string? folder = null)
        {
            if (fileStream == null || fileStream.Length == 0)
                throw new ArgumentException("File stream is required and cannot be empty", nameof(fileStream));

            if (string.IsNullOrWhiteSpace(fileName))
                throw new ArgumentException("File name is required", nameof(fileName));

            var uploadParams = new ImageUploadParams
            {
                File = new FileDescription(fileName, fileStream),
                Folder = GetFolderPath(usageCategory, folder),
                UseFilename = true,
                UniqueFilename = true,
                Overwrite = false,
                Transformation = GetTransformation(usageCategory)
            };

            var result = await _cloudinary.UploadAsync(uploadParams);

            if (result.Error != null)
                throw new Exception($"Cloudinary upload failed: {result.Error.Message}");

            return result.SecureUrl?.ToString() ?? _noImageUrl;
        }


        private async Task<string> UploadMediaAsync(string filePath, UsageCategory usageCategory, string? folder = null)
        {
            if (string.IsNullOrWhiteSpace(filePath))
                throw new ArgumentException("File path cannot be null or empty", nameof(filePath));

            if (!File.Exists(filePath))
                throw new FileNotFoundException($"File not found at path: {filePath}");

            var uploadParams = new ImageUploadParams
            {
                File = new FileDescription(filePath),
                Folder = GetFolderPath(usageCategory, folder),
                UseFilename = true,
                UniqueFilename = true,
                Overwrite = false,
                Transformation = GetTransformation(usageCategory)
            };

            var result = await _cloudinary.UploadAsync(uploadParams);

            if (result.Error != null)
                throw new Exception($"Cloudinary upload failed: {result.Error.Message}");

            return result.SecureUrl?.ToString() ?? _noImageUrl;
        }


        /// <summary>
        /// Get folder path based on usage category
        /// </summary>
        private static string GetFolderPath(UsageCategory usageCategory, string? customFolder = null)
        {
            if (!string.IsNullOrWhiteSpace(customFolder))
                return customFolder;

            return usageCategory switch
            {
                UsageCategory.ProfilePicture => "educational_platform/profile_pictures",
                UsageCategory.Thumbnail => "educational_platform/video_thumbnails",
                _ => "educational_platform/uploads"
            };
        }


        /// <summary>
        /// Get optimized transformation based on usage category
        /// </summary>
        private static Transformation GetTransformation(UsageCategory imageType)
        {
            return imageType switch
            {
                // Profile Picture: Square crop with face detection, optimized for avatars
                UsageCategory.ProfilePicture => new Transformation()
                    .Width(400).Height(400)
                    .Crop("fill")
                    .Gravity("face")
                    .Quality("auto:good")
                    .FetchFormat("auto")
                    .Chain()
                    .Radius("max") // Optional: makes it circular
                    .Border("2px_solid_white"),

                // Video Thumbnail: 16:9 aspect ratio, optimized for video previews
                UsageCategory.Thumbnail => new Transformation()
                    .Width(1280).Height(720)
                    .Crop("fill")
                    .Gravity("center")
                    .Quality("auto:good")
                    .FetchFormat("auto")
                    .Chain()
                    .Effect("sharpen:100"),

                _ => throw new ArgumentException($"Invalid usage category: {imageType}", nameof(imageType))
            };
        }
    }
}
