using System.ComponentModel.DataAnnotations;

namespace Application.DTOs.Videos
{
    public class UpdateVideoProgressRequest
    {
        [Required]
        public Guid VideoId { get; set; }

        [Range(0, 100)]
        public int Progress { get; set; }
    }
}
