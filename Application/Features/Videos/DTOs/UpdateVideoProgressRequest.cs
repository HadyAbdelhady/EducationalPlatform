using System.ComponentModel.DataAnnotations;

namespace Application.Features.Videos.DTOs
{
    public class UpdateVideoProgressRequest
    {
        [Required]
        public Guid VideoId { get; set; }

        [Range(0, 100)]
        public int Progress { get; set; }
    }
}
