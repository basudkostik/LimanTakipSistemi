using System.ComponentModel.DataAnnotations;

namespace LimanTakipSistemi.API.Models.DTOs.Ship
{
    public class UpdateShipRequestDto
    {
        [Required]
        [StringLength(100)]
        public string Name { get; set; }

        [Required]
        [StringLength(10)]
        public string IMO { get; set; }

        [Required]
        [StringLength(50)]
        public string Type { get; set; }

        [Required]
        [StringLength(50)]
        public string Flag { get; set; }

        [Required]
        public int YearBuilt { get; set; }
    }
}
