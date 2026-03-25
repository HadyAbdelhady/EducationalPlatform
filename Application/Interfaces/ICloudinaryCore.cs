using Application.DTOs.Media;
using Application.DTOs.Videos;
using Domain.enums;
using Microsoft.AspNetCore.Http;

namespace Application.Interfaces
{
    public interface ICloudinaryCore
    {

        // Upload methods
        public Task<CloudinaryMediaResult> UploadPdfAsync(IFormFile file, string? folder = null);
        Task<string> UploadMediaAsync(IFormFile file, UsageCategory usageCategory, string? folder = null);
        Task<string> UploadVideoAsync(IFormFile file, string? folder = null);

        // Direct upload methods (client uploads directly to Cloudinary)
        Task<DirectVideoUploadSignatureResponse> GenerateDirectUploadSignatureAsync(
            DirectVideoUploadSignatureRequest request);

        // Edit methods
        Task<string> EditMediaAsync(string publicId, IFormFile file, UsageCategory usageCategory, string? folder = null);
        public Task<CloudinaryMediaResult> UpdatePdfAsync(string publicId, IFormFile newPdf);

        // Delete methods
        Task<bool> DeleteMediaAsync(IEnumerable<string> id);
        Task<bool> DeleteSingleMediaAsync(string id);
    }
}
