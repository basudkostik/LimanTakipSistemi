using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace LimanTakipSistemi.API.Models.Domain

{
    public class Ship
    {

        [Key]
        public int ShipId { get; set; }

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
        public int YearBuilt {  get; set; }

        public ICollection<ShipVisit> ShipVisits{ get; set; } = new List<ShipVisit>();

        public ICollection<Cargo> Cargoes { get ; set; } = new List<Cargo>();

    }
}
