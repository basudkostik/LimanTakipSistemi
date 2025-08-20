using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LimanTakipSistemi.API.Models.DTOs.Cargo
{
    public class UpdateCargoRequestDto
    {
        [Required]
        public int ShipId { get; set; }

        [Required]
        [StringLength(200)]
        public string Description { get; set; }

        [Column(TypeName = "decimal(10,2)")]
        [Range(0.01, 9999999.99)]
        public decimal Weight { get; set; }

        [Required]
        [StringLength(50)]
        public string CargoType { get; set; }
    }
}
