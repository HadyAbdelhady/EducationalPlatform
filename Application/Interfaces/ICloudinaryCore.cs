using Domain.enums;
using Microsoft.AspNetCore.Http;

namespace Application.Interfaces
{
    public interface ICloudinaryCore
    {

        // Upload methods
        Task<string> UploadMediaAsync(IFormFile file, UsageCategory usageCategory, string? folder = null);
        Task<string> UploadVideoAsync(IFormFile file, string? folder = null);

        // Edit methods
        Task<string> EditMediaAsync(string publicId, IFormFile file, UsageCategory usageCategory, string? folder = null);

        // Delete methods
        Task<bool> DeleteMediaAsync(IEnumerable<string> id);
        Task<bool> DeleteSingleMediaAsync(string id);
    }
}
