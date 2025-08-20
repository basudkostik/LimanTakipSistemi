using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LimanTakipSistemi.API.Models.Domain
{
    public class Cargo
    {
        [Key]
        public int CargoId { get; set; }


        [ForeignKey("ShipId")]
        public Ship Ship { get; set; }
        public int ShipId { get; set; }

        
        [StringLength(200)]
        [Required]
        public string Description  { get; set; }

        [Column(TypeName ="decimal(10,2)")]
        public decimal Weight { get; set; }

        [Required]
        [StringLength(50)]
        public string CargoType { get; set; }


    }
}
