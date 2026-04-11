using System.ComponentModel.DataAnnotations;

namespace Application.DTOs.Videos
{
    /// <summary>
    /// Request DTO sent by the client AFTER a successful direct upload to Cloudinary.
    /// The client sends back the Cloudinary response metadata so the backend can
    /// save the video reference in the database without ever touching the file binary.
    /// </summary>
    public class SaveDirectVideoUploadRequest
    {
        /// <summary>
        /// The Cloudinary public_id returned after a successful upload.
        /// Used as a unique identifier for the asset in Cloudinary.
        /// </summary>
        [Required]
        public string PublicId { get; set; } = string.Empty;

        /// <summary>
        /// The version number returned by Cloudinary. Combined with public_id,
        /// this ensures the URL always points to the correct version of the asset.
        /// </summary>
        [Required]
        public string Version { get; set; } = string.Empty;

        /// <summary>
        /// The HTTPS URL of the uploaded video, returned by Cloudinary.
        /// This is stored in the database as the video's playback URL.
        /// </summary>
        [Required]
        public string SecureUrl { get; set; } = string.Empty;

        /// <summary>
        /// The section this video belongs to in the educational platform.
        /// </summary>
        [Required]
        public Guid SectionId { get; set; }

        /// <summary>
        /// Display name for the video.
        /// </summary>
        [Required]
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// Optional description of the video content.
        /// </summary>
        public string? Description { get; set; }
    }
}
