namespace Application.DTOs.Videos
{
    /// <summary>
    /// Response DTO returned to the client after signature generation.
    /// 
    /// SECURITY CRITICAL: This response contains everything the client needs
    /// to upload directly to Cloudinary EXCEPT the ApiSecret.
    /// The ApiSecret is used server-side only to compute the Signature field
    /// and is NEVER sent to the client.
    /// </summary>
    public class DirectVideoUploadSignatureResponse
    {
        /// <summary>
        /// HMAC-SHA256 hex signature computed from the upload parameters + api_secret.
        /// This proves to Cloudinary that our backend authorized this specific upload
        /// with these specific parameters.
        /// </summary>
        public string Signature { get; set; } = string.Empty;

        /// <summary>
        /// Unix timestamp (seconds since epoch). Cloudinary rejects signatures
        /// older than 1 hour to prevent replay attacks.
        /// </summary>
        public long Timestamp { get; set; }

        /// <summary>
        /// Cloudinary cloud name — needed by the client to construct the upload URL:
        /// https://api.cloudinary.com/v1_1/{CloudName}/video/upload
        /// </summary>
        public string CloudName { get; set; } = string.Empty;

        /// <summary>
        /// Cloudinary API Key — this is PUBLIC and safe to expose to the client.
        /// It identifies the account but cannot authorize actions without the signature.
        /// </summary>
        public string ApiKey { get; set; } = string.Empty;

        /// <summary>
        /// The folder path where the video will be stored in Cloudinary.
        /// Must match the value used when computing the signature.
        /// </summary>
        public string Folder { get; set; } = string.Empty;

        /// <summary>
        /// The Cloudinary resource type (always "video" for this endpoint).
        /// </summary>
        public string ResourceType { get; set; } = "video";
    }
}
