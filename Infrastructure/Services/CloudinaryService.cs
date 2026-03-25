using Application.DTOs.Media;
using Application.DTOs.Videos;
using Application.Interfaces;
using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using Domain.enums;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;

namespace Infrastructure.Services
{
    public class CloudinaryService(Cloudinary cloudinary, IOptions<CloudinarySettings> cloudinarySettings) : ICloudinaryCore
    {
        private readonly Cloudinary _cloudinary = cloudinary;

        // SECURITY CRITICAL: These settings contain the ApiSecret.
        // They are used ONLY server-side for signature computation and are
        // NEVER exposed in any API response.
        private readonly CloudinarySettings _settings = cloudinarySettings.Value;

        private readonly string _noImageUrl = "https://img.freepik.com/premium-vector/default-image-icon-vector-missing-picture-page-website-design-mobile-app-no-photo-available_87543-11093.jpg";

        public Task<Cloudinary> GetClientAsync() => Task.FromResult(_cloudinary);

        #region Direct Upload Signature

        /// <summary>
        /// Generates a time-limited, parameter-bound signature that allows the client
        /// to upload a video DIRECTLY to Cloudinary without the file ever touching our server.
        /// 
        /// SECURITY ARCHITECTURE:
        /// 1. The client sends file metadata (name, size) - NOT the file itself.
        /// 2. We validate the metadata (type, size) server-side before signing.
        /// 3. We compute: signature = SHA1(sorted_params + api_secret)
        /// 4. We return the signature + public params. The api_secret NEVER leaves the server.
        /// 5. Cloudinary verifies the signature on its end using the same api_secret.
        /// 6. The signature expires after 1 hour (Cloudinary enforces this via the timestamp).
        /// </summary>
        public Task<DirectVideoUploadSignatureResponse> GenerateDirectUploadSignatureAsync(
            DirectVideoUploadSignatureRequest request)
        {
            // -- Step 1: Validate file extension --
            // We validate BEFORE signing so we never authorize uploads of non-video files.
            var allowedExtensions = new[] { ".mp4", ".mov", ".avi", ".mkv" };
            var fileExtension = Path.GetExtension(request.FileName)?.ToLowerInvariant();

            if (string.IsNullOrWhiteSpace(fileExtension) || !allowedExtensions.Contains(fileExtension))
                throw new ArgumentException(
                    $"Invalid file type '{fileExtension}'. Allowed types: {string.Join(", ", allowedExtensions)}");

            // -- Step 2: Validate file size --
            // Max 1000 MB - must match the client-side validation.
            const long maxFileSize = 1000L * 1024 * 1024;
            if (request.FileSize <= 0)
                throw new ArgumentException("File size must be greater than 0.");

            if (request.FileSize > maxFileSize)
                throw new ArgumentException(
                    $"File size ({request.FileSize / (1024 * 1024)}MB) exceeds the maximum allowed size of {maxFileSize / (1024 * 1024)}MB.");

            // -- Step 3: Build signature parameters --
            // Cloudinary requires parameters to be sorted alphabetically and
            // concatenated as "key=value&key=value" before appending the api_secret.
            var timestamp = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
            var folder = "educational_platform/videos";

            // These are the parameters that will be signed.
            // The client MUST include the exact same values when uploading,
            // otherwise Cloudinary will reject the signature as invalid.
            var paramsToSign = new SortedDictionary<string, object>
            {
                { "folder", folder },
                { "timestamp", timestamp }
            };

            // -- Step 4: Compute the signature --
            // SECURITY CRITICAL: This is the core of the signed upload pattern.
            // We build "folder=xyz&timestamp=123" then append the api_secret
            // and compute SHA-1 (Cloudinary's required algorithm for signed uploads).
            var paramString = string.Join("&",
                paramsToSign.Select(kvp => $"{kvp.Key}={kvp.Value}"));

            // Append api_secret - this is WHY the signature can only be generated server-side
            var stringToSign = paramString + _settings.ApiSecret;

            string signature;
            using (var sha1 = SHA1.Create())
            {
                var hashBytes = sha1.ComputeHash(Encoding.UTF8.GetBytes(stringToSign));
                signature = BitConverter.ToString(hashBytes).Replace("-", "").ToLowerInvariant();
            }

            // -- Step 5: Return the response --
            // Note: We return ApiKey (public identifier) but NEVER ApiSecret.
            // The client uses these values to construct the multipart form POST to Cloudinary.
            var response = new DirectVideoUploadSignatureResponse
            {
                Signature = signature,
                Timestamp = timestamp,
                CloudName = _settings.CloudName,
                ApiKey = _settings.ApiKey,     // Safe to expose - this is a public identifier
                Folder = folder,
                ResourceType = "video"
                // ApiSecret is intentionally OMITTED - never sent to the client
            };

            return Task.FromResult(response);
        }

        #endregion

        #region PDF Upload

        public async Task<CloudinaryMediaResult> UploadPdfAsync(IFormFile file, string? folder = null)
        {
            if (file == null || file.Length == 0)
                throw new ArgumentException("File is required and cannot be empty", nameof(file));

            // Validate file type
            var allowedExtensions = new[] { ".pdf" };
            var fileExtension = Path.GetExtension(file.FileName).ToLowerInvariant();

            if (!allowedExtensions.Contains(fileExtension))
                throw new ArgumentException("Only PDF files are allowed.");

            // Validate file size (max 20MB)
            const long maxFileSize = 20 * 1024 * 1024;
            if (file.Length > maxFileSize)
                throw new ArgumentException("PDF file size exceeds 20MB limit.");

            using var stream = file.OpenReadStream();

            var uploadParams = new RawUploadParams
            {

                File = new FileDescription(file.FileName, stream),
                Folder = folder ?? "educational_platform/pdfs",
                UseFilename = true,
                UniqueFilename = true,
                Overwrite = false
            };

            var result = await _cloudinary.UploadAsync(uploadParams);

            if (result.Error != null)
                throw new Exception($"Cloudinary PDF upload failed: {result.Error.Message}");

            return new CloudinaryMediaResult
            {
                Url = result.SecureUrl.ToString(),
                PublicId = result.PublicId
            };
        }

        public async Task<CloudinaryMediaResult> UpdatePdfAsync(string publicId, IFormFile newPdf)
        {
            if (string.IsNullOrWhiteSpace(publicId))
                throw new ArgumentException("Public ID is required", nameof(publicId));

            try
            {
                // Upload new pdf
                var newPdfUploadResult = await UploadPdfAsync(newPdf);

                // Delete old pdf only after successful upload
                var deleted = await DeleteSingleMediaAsync(publicId);
                if (!deleted)
                {
                    Console.WriteLine($"Warning: Could not delete old pdf with ID: {publicId}");
                }

                return newPdfUploadResult;
            }
            catch (Exception ex)
            {
                throw new Exception($"Failed to edit pdf: {ex.Message}", ex);
            }
        }

        #endregion

        #region Delete

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

        #endregion

        #region Media Upload (Images)

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

        #endregion

        #region Video Upload (Server-Proxy - Legacy)

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

        #endregion

        #region Edit Media

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

        #endregion

        #region Private Helpers

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
                UsageCategory.CourseThumbnail => "educational_platform/course_thumbnails",
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
                    .Radius("max")
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

                // Course Thumbnail: 16:9 aspect ratio, optimized for course cards
                UsageCategory.CourseThumbnail => new Transformation()
                    .Width(800).Height(450)
                    .Crop("fill")
                    .Gravity("center")
                    .Quality("auto:good")
                    .FetchFormat("auto"),

                _ => throw new ArgumentException($"Invalid usage category: {imageType}", nameof(imageType))
            };
        }

        #endregion
    }
}
