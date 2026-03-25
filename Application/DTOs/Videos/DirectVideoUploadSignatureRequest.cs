namespace Application.DTOs.Videos
{
    /// <summary>
    /// Request DTO for generating a Cloudinary direct upload signature.
    /// The backend validates these fields BEFORE generating a signature,
    /// ensuring we never sign uploads for disallowed file types or oversized files.
    /// </summary>
    public class DirectVideoUploadSignatureRequest
    {
        /// <summary>
        /// Original file name including extension (e.g., "lecture_01.mp4").
        /// Used to validate the file type on the backend before signing.
        /// </summary>
        public string FileName { get; set; } = string.Empty;

        /// <summary>
        /// File size in bytes. Validated against the max allowed size 
        /// BEFORE generating a signature to prevent abuse.
        /// </summary>
        public long FileSize { get; set; }
    }
}
