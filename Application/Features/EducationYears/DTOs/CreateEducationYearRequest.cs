using System.ComponentModel.DataAnnotations;

namespace Application.Features.EducationYears.DTOs
{
    public class CreateEducationYearRequest
    {
        [Required(ErrorMessage = "Education year name is required.")]
        [StringLength(100, MinimumLength = 2, ErrorMessage = "Education year name must be between 2 and 100 characters.")]
        public string EducationYearName { get; set; } = string.Empty;
    }
}
