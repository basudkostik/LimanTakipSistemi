using System.ComponentModel.DataAnnotations;

namespace LimanTakipSistemi.API.Models.DTOs.Port
{
    public class UpdatePortRequestDto
    {
        [Required]
        [StringLength(100)]
        public string Name { get; set; }

        [Required]
        [StringLength(50)]
        public string Country { get; set; }

        [Required]
        [StringLength(50)]
        public string City { get; set; }
    }
}
